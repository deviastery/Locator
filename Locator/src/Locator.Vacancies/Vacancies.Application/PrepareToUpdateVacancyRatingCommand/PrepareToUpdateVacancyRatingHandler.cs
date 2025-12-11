using System.Text.Json;
using Confluent.Kafka;
using CSharpFunctionalExtensions;
using FluentValidation;
using Framework.Extensions;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Abstractions;
using Vacancies.Application.Fails;
using Vacancies.Contracts.Dto;
using Vacancies.Domain;

namespace Vacancies.Application.PrepareToUpdateVacancyRatingCommand;

    public class PrepareToUpdateVacancyRatingCommandHandler : ICommandHandler<PrepareToUpdateVacancyRatingCommand>
{
    private readonly IProducer<Null, string> _producer;
    private readonly IVacanciesRepository _vacanciesRepository;
    private readonly ILogger<PrepareToUpdateVacancyRatingCommandHandler> _logger;
    private readonly IValidator<UpdateVacancyRatingDto> _validator;
    
    public PrepareToUpdateVacancyRatingCommandHandler(
        IProducer<Null, string> producer,
        IVacanciesRepository vacanciesRepository,
        ILogger<PrepareToUpdateVacancyRatingCommandHandler> logger, 
        IValidator<UpdateVacancyRatingDto> validator)
    {
        _producer = producer;
        _vacanciesRepository = vacanciesRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<UnitResult<Failure>> Handle(
        PrepareToUpdateVacancyRatingCommand command, 
        CancellationToken cancellationToken)
    {
        // Get all Reviews of a Vacancy
        var reviewsVacancyId = await _vacanciesRepository.GetReviewsByVacancyIdAsync(
            command.VacancyId, cancellationToken);
        if (reviewsVacancyId.Count == 0)
        {
            return Errors.General.NotFound($"Reviews not found by vacancy ID={command.VacancyId}").ToFailure();
        }

        // Calculate average mark
        var averageMarkResult = Review.CalculateAverageMark(reviewsVacancyId);
        if (averageMarkResult.IsFailure)
        {
            return averageMarkResult.Error.ToFailure();
        }
        
        // Input data validation
        var updateVacancyRatingDto = new UpdateVacancyRatingDto(command.VacancyId, averageMarkResult.Value);
        var validationResult = await _validator.ValidateAsync(updateVacancyRatingDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors().ToFailure();
        }
        
        // Create vacancy Rating
        var dto = new CreateRequestVacancyRatingDto(command.VacancyId, averageMarkResult.Value);
        var result = await _producer.ProduceAsync(
            "add-review",
            new Message<Null, string>
            {
                Value = JsonSerializer.Serialize(dto),
            });
        if (result.Status != PersistenceStatus.Persisted)
        {
            return Errors.SentVacancyRatingFail().ToFailure();
        }
        
        return default;
    }
}