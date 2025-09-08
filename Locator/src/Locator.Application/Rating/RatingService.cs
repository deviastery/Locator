using FluentValidation;
using Locator.Contracts.Rating;
using Microsoft.Extensions.Logging;

namespace Locator.Application.Rating;

public class RatingService : IRatingService
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IValidator<CreateVacancyRatingDto> _validator;
    private readonly ILogger<RatingService> _logger;

    public RatingService(
        IRatingRepository ratingRepository, 
        IValidator<CreateVacancyRatingDto> validator,
        ILogger<RatingService> logger)
    {
        _ratingRepository = ratingRepository;
        _validator = validator;
        _logger = logger;
    }

    public Task<Guid> CreateVacancyRating(Guid vacancyId, CreateVacancyRatingDto reviewsVacancyId, CancellationToken cancellationToken) => throw new NotImplementedException();
}