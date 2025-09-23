using Locator.Application.Ratings;
using Locator.Domain.Ratings;
using Microsoft.EntityFrameworkCore;

namespace Locator.Infrastructure.Postgresql.Ratings;

public class RatingsEfCoreRepository : IRatingsRepository
{
    private readonly LocatorDbContext _locatorDbContext;

    public RatingsEfCoreRepository(LocatorDbContext locatorDbContext)
    {
        _locatorDbContext = locatorDbContext;
    }

    public async Task<Guid> CreateVacancyRatingAsync(VacancyRating rating, CancellationToken cancellationToken)
    {
        var vacancyRating = await _locatorDbContext.VacancyRatings
            .SingleOrDefaultAsync(r => r.EntityId == rating.EntityId, cancellationToken);
        if (vacancyRating != null)
        {
            vacancyRating.Value = rating.Value;
            await _locatorDbContext.SaveChangesAsync(cancellationToken);
            return rating.Id;
        }
        
        await _locatorDbContext.VacancyRatings.AddAsync(rating, cancellationToken);
        await _locatorDbContext.SaveChangesAsync(cancellationToken);
        return rating.Id;
    }
}