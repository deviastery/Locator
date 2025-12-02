using CSharpFunctionalExtensions;
using FluentValidation;
using Framework.Extensions;
using Microsoft.Extensions.Logging;
using Ratings.Contracts;
using Ratings.Contracts.Dto;
using Shared;
using Shared.Abstractions;
using Vacancies.Application.Fails;
using Vacancies.Domain;

namespace Vacancies.Application.PrepareToUpdateVacancyRatingCommand;

    public class PrepareToUpdateVacancyRatingCommandHandler : ICommandHandler<PrepareToUpdateVacancyRatingCommand>
{
    private readonly IVacanciesRepository _vacanciesRepository;
    private readonly IRatingsContract _ratingsContract;
    private readonly ILogger<PrepareToUpdateVacancyRatingCommandHandler> _logger;
    private readonly IValidator<UpdateVacancyRatingDto> _validator;
    
    public PrepareToUpdateVacancyRatingCommandHandler(
        IVacanciesRepository vacanciesRepository,
        IRatingsContract ratingsContract, 
        ILogger<PrepareToUpdateVacancyRatingCommandHandler> logger, 
        IValidator<UpdateVacancyRatingDto> validator)
    {
        _vacanciesRepository = vacanciesRepository;
        _ratingsContract = ratingsContract;
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
        var vacancyRatingDto = new UpdateVacancyRatingDto(command.VacancyId, averageMarkResult.Value);
        var validationResult = await _validator.ValidateAsync(vacancyRatingDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors().ToFailure();
        }

        // Create vacancy Rating
        var updateVacancyRatingDto = new UpdateVacancyRatingDto(command.VacancyId, averageMarkResult.Value);
        
        var createVacancyRatingResult = await _ratingsContract
            .UpdateVacancyRatingAsync(updateVacancyRatingDto, cancellationToken);
        if (createVacancyRatingResult.IsFailure)
        {
            return createVacancyRatingResult.Error;
        }

        return createVacancyRatingResult;
    }
}