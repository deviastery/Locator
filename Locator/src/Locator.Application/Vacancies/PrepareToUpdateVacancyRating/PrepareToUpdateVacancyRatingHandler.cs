using CSharpFunctionalExtensions;
using FluentValidation;
using Locator.Application.Abstractions;
using Locator.Application.Extensions;
using Locator.Application.Ratings.UpdateVacancyRating;
using Locator.Contracts.Ratings;
using Locator.Domain.Vacancies;
using Microsoft.Extensions.Logging;
using Shared;

namespace Locator.Application.Vacancies.PrepareToUpdateVacancyRating;

public class PrepareToUpdateVacancyRatingHandler : ICommandHandler<PrepareToUpdateVacancyRatingCommand>
{
    private readonly IVacanciesRepository _vacanciesRepository;
    private readonly ICommandHandler<Guid, UpdateVacancyRatingCommand> _updateVacancyRatingCommandHandler;
    private readonly ILogger<PrepareToUpdateVacancyRatingHandler> _logger;
    private readonly IValidator<UpdateVacancyRatingDto> _validator;
    
    public PrepareToUpdateVacancyRatingHandler(
        IVacanciesRepository vacanciesRepository,
        ICommandHandler<Guid, UpdateVacancyRatingCommand> updateVacancyRatingCommandHandler, 
        ILogger<PrepareToUpdateVacancyRatingHandler> logger, 
        IValidator<UpdateVacancyRatingDto> validator)
    {
        _vacanciesRepository = vacanciesRepository;
        _updateVacancyRatingCommandHandler = updateVacancyRatingCommandHandler;
        _logger = logger;
        _validator = validator;
    }

    public async Task<UnitResult<Failure>> Handle(
        PrepareToUpdateVacancyRatingCommand command, 
        CancellationToken cancellationToken)
    {
        // Get all reviews of a vacancy
        var reviewsVacancyId = await _vacanciesRepository.GetReviewsByVacancyIdAsync(
            command.vacancyId, cancellationToken);

        // Calculate average mark
        var averageMarkResult = Review.CalculateAverageMark(reviewsVacancyId);
        if (averageMarkResult.IsFailure)
        {
            return averageMarkResult.Error.ToFailure();
        }
        
        // Input data validation
        var vacancyRatingDto = new UpdateVacancyRatingDto(command.vacancyId, averageMarkResult.Value);
        var validationResult = await _validator.ValidateAsync(vacancyRatingDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors().ToFailure();
        }

        // Create VacancyRating
        var updateVacancyRatingDto = new UpdateVacancyRatingDto(command.vacancyId, averageMarkResult.Value);
        var updateVacancyRatingCommand = new UpdateVacancyRatingCommand(updateVacancyRatingDto);
        var createVacancyRatingResult = await _updateVacancyRatingCommandHandler
            .Handle(updateVacancyRatingCommand, cancellationToken);
        if (createVacancyRatingResult.IsFailure)
        {
            return createVacancyRatingResult.Error;
        }

        return createVacancyRatingResult;
    }
}