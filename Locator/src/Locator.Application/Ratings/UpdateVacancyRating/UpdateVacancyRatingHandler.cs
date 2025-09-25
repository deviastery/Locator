using CSharpFunctionalExtensions;
using FluentValidation;
using Locator.Application.Abstractions;
using Locator.Application.Extensions;
using Locator.Contracts.Ratings;
using Locator.Domain.Ratings;
using Microsoft.Extensions.Logging;
using Shared;

namespace Locator.Application.Ratings.UpdateVacancyRating;

public class UpdateVacancyRatingHandler : ICommandHandler<Guid, UpdateVacancyRatingCommand>
{
    private readonly IRatingsRepository _ratingsRepository;
    private readonly IValidator<UpdateVacancyRatingDto> _validator;
    private readonly ILogger<UpdateVacancyRatingHandler> _logger;
    
    public UpdateVacancyRatingHandler(
        ILogger<UpdateVacancyRatingHandler> logger, 
        IValidator<UpdateVacancyRatingDto> validator, 
        IRatingsRepository ratingsRepository)
    {
        _logger = logger;
        _validator = validator;
        _ratingsRepository = ratingsRepository;
    }

    public async Task<Result<Guid, Failure>> Handle(
        UpdateVacancyRatingCommand command,
        CancellationToken cancellationToken)
    {
        // Input data validation
        var validationResult = await _validator.ValidateAsync(command.vacancyRatingDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors().ToFailure();
        }
        
        // Create VacancyRating
        (double averageMark, Guid vacancyId) = (command.vacancyRatingDto.AverageMark, command.vacancyRatingDto.VacancyId);
        var rating = new VacancyRating(averageMark, vacancyId);
        var ratingId = await _ratingsRepository.CreateVacancyRatingAsync(rating, cancellationToken);
        _logger.LogInformation("Rating created or updated with id={RatingId} on vacancy with id={VacancyId}", ratingId, vacancyId);

        return rating.Id;
    }
}