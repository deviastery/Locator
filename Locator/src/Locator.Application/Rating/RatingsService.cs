using Locator.Domain.Rating;
using Microsoft.Extensions.Logging;

namespace Locator.Application.Rating;

public class RatingsService : IRatingsService
{
    private readonly IRatingsRepository _ratingsRepository;
    private readonly ILogger<RatingsService> _logger;

    public RatingsService(
        IRatingsRepository ratingsRepository, 
        ILogger<RatingsService> logger)
    {
        _ratingsRepository = ratingsRepository;
        _logger = logger;
    }

    public async Task<Guid> CreateVacancyRating(Guid vacancyId, double averageMark,
        CancellationToken cancellationToken)
    {
        // Валидация входных данных
        if (averageMark < 0 || averageMark > 5)
        {
            throw new Exception("Оценка должна быть в пределах от 0.0 до 5.0");
        }
        
        var rating = new VacancyRating(averageMark, vacancyId);
        await _ratingsRepository.CreateVacancyRatingAsync(rating, cancellationToken);
        _logger.LogInformation("Rating created with id={ReviewId} on vacancy with id={VacancyId}", rating.Id, vacancyId);

        return rating.Id;
    }
}