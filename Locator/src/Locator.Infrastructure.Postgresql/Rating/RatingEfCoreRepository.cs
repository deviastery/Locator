using Locator.Application.Rating;
using Locator.Domain.Rating;

namespace Locator.Infrastructure.Postgresql.Rating;

public class RatingsEfCoreRepository : IRatingsRepository
{
    private readonly LocatorDbContext _locatorDbContext;

    public RatingsEfCoreRepository(LocatorDbContext locatorDbContext)
    {
        _locatorDbContext = locatorDbContext;
    }

    public async Task<Guid> CreateVacancyRatingAsync(VacancyRating rating, CancellationToken cancellationToken)
    {
        await _locatorDbContext.VacancyRatings.AddAsync(rating, cancellationToken);
        await _locatorDbContext.SaveChangesAsync(cancellationToken);
        return rating.Id;
    }
}