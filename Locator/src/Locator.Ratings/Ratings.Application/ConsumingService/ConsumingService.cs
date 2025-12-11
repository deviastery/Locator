using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ratings.Contracts.Dto;
using Shared.Abstractions;

namespace Ratings.Application.ConsumingService;

public class ConsumingService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConsumer<Null, string> _consumer;
    private readonly ILogger<ConsumingService> _logger;
    
    private const string AddReviewTopic = "add-review";

    public ConsumingService(
        IServiceScopeFactory scopeFactory,
        IConsumer<Null, string> consumer, 
        ILogger<ConsumingService> logger)
    {
        _scopeFactory = scopeFactory;
        _consumer = consumer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(5000);
        
        _consumer.Subscribe([AddReviewTopic]);
        _logger.LogInformation("Started consuming from topic: {Topic}", AddReviewTopic);
        
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = _consumer.Consume(cancellationToken);
                if (consumeResult?.Message == null) continue;

                if (consumeResult.Topic == AddReviewTopic)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var commandHandler = scope.ServiceProvider.GetRequiredService<
                        ICommandHandler<Guid, UpdateVacancyRatingCommand.UpdateVacancyRatingCommand>>();
                    
                    var dto = JsonSerializer.Deserialize<UpdateVacancyRatingDto>(
                        consumeResult.Message.Value);
                    if (dto == null)
                    {
                        _logger.LogWarning("Invalid message format: {Value}", consumeResult.Message.Value);
                        continue;
                    }

                    var command = new UpdateVacancyRatingCommand.UpdateVacancyRatingCommand(dto);
                    var ratingIdResponse = await commandHandler.Handle(command, cancellationToken);
                    if (ratingIdResponse.IsFailure)
                    {
                        _logger.LogError("Failed to handle message: {Error}", ratingIdResponse.Error);
                    }
                    else
                    {
                        _consumer.Commit(consumeResult);
                        _logger.LogInformation("Successfully processed rating for vacancy ID: {VacancyId}", dto.VacancyId);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Consuming service is stopping.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consuming message.");
            }
        }
    }
    
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _consumer.Close();
        await base.StopAsync(cancellationToken);
    }
}