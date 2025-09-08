using Locator.Application.Rating;
using Locator.Domain.Rating;

namespace Locator.Infrastructure.Postgresql.Rating;

public class RatingEfCoreRepository : IRatingRepository
{
    private readonly DbContext _dbContext;

    public RatingEfCoreRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> CreateVacancyRatingAsync(VacancyRating rating, CancellationToken cancellationToken)
    {
        await _dbContext.Rating.AddAsync(rating, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return rating.Id;
    }
}