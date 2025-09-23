using CSharpFunctionalExtensions;
using FluentValidation;
using Locator.Application.Extensions;
using Locator.Contracts.Ratings;
using Locator.Domain.Ratings;
using Microsoft.Extensions.Logging;
using Shared;

namespace Locator.Application.Ratings;

public class RatingsService : IRatingsService
{
    private readonly IRatingsRepository _ratingsRepository;
    private readonly IValidator<CreateVacancyRatingDto> _validator;
    private readonly ILogger<RatingsService> _logger;

    public RatingsService(
        IRatingsRepository ratingsRepository,
        IValidator<CreateVacancyRatingDto> validator,
        ILogger<RatingsService> logger)
    {
        _ratingsRepository = ratingsRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, Failure>> CreateVacancyRating(
        CreateVacancyRatingDto vacancyRatingDto,
        CancellationToken cancellationToken)
    {
        // Валидация входных данных
        var validationResult = await _validator.ValidateAsync(vacancyRatingDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors().ToFailure();
        }
        
        (double averageMark, Guid vacancyId) = (vacancyRatingDto.AverageMark, vacancyRatingDto.VacancyId);
        var rating = new VacancyRating(averageMark, vacancyId);
        
        await _ratingsRepository.CreateVacancyRatingAsync(rating, cancellationToken);
        _logger.LogInformation("Rating created with id={ReviewId} on vacancy with id={VacancyId}", rating.Id, vacancyId);

        return rating.Id;
    }
}