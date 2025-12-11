using Confluent.Kafka;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ratings.Application.GetRatingByVacancyIdQuery;
using Ratings.Application.GetVacancyRatingsQuery;
using Ratings.Application.UpdateVacancyRatingCommand;
using Ratings.Contracts.Responses;
using Shared.Abstractions;
using Shared.Options;

namespace Ratings.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<ICommandHandler<Guid, UpdateVacancyRatingCommand.UpdateVacancyRatingCommand>, UpdateVacancyRatingCommandHandler>();
        
        services.AddScoped<IQueryHandler<RatingByVacancyIdResponse, GetRatingByVacancyIdQuery.GetRatingByVacancyIdQuery>, GetRatingByVacancyId>();
        services.AddScoped<IQueryHandler<VacancyRatingsResponse, GetVacancyRatingsQuery.GetVacancyRatingsQuery>, GetVacancyRatings>();
        
        services.AddHostedService<ConsumingService.ConsumingService>();
        
        var kafkaOptions = configuration.GetSection(KafkaOptions.SECTION_NAME).Get<KafkaOptions>();
        var consumerConfig = new ConsumerConfig
        {
            GroupId = "add-review-consumer-group",
            BootstrapServers = kafkaOptions?.BootstrapServers ?? "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };
        services.AddSingleton<IConsumer<Null, string>>(sp => 
            new ConsumerBuilder<Null, string>(consumerConfig).Build());

        return services;
    }
}