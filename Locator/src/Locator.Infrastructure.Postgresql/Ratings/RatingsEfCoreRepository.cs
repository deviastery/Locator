using CSharpFunctionalExtensions;
using Locator.Application.Ratings;
using Locator.Application.Ratings.Fails;
using Locator.Domain.Ratings;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Locator.Infrastructure.Postgresql.Ratings;

public class RatingsEfCoreRepository : IRatingsRepository
{
    private readonly RatingsDbContext _ratingsDbContext;

    public RatingsEfCoreRepository(RatingsDbContext ratingsDbContext)
    {
        _ratingsDbContext = ratingsDbContext;
    }

    public async Task<Result<Guid, Error>> UpdateVacancyRatingAsync(VacancyRating rating, CancellationToken cancellationToken)
    {
        try
        {
            var ratingRecord = await _ratingsDbContext.VacancyRatings
                .SingleOrDefaultAsync(r => r.EntityId == rating.EntityId, cancellationToken);

            if (ratingRecord == null)
            {
                return Errors.General.NotFound(rating.Id);
            }
            
            ratingRecord.Value = rating.Value;
            await _ratingsDbContext.SaveChangesAsync(cancellationToken);
            return ratingRecord.Id;
        }
        catch (Exception e)
        {
            return Errors.General.Failure(e.Message);
        }
    }

    public async Task<Result<Guid, Error>> CreateVacancyRatingAsync(VacancyRating rating, CancellationToken cancellationToken)
    {
        try
        {
            var newVacancyRating = await _ratingsDbContext.VacancyRatings.AddAsync(rating, cancellationToken);
            await _ratingsDbContext.SaveChangesAsync(cancellationToken);
            return newVacancyRating.Entity.Id;
        }
        catch (Exception e)
        {
            return Errors.General.Failure(e.Message);
        }
    }
}