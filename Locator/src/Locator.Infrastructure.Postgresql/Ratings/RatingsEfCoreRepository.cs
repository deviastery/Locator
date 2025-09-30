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

    public async Task<Guid> UpdateVacancyRatingAsync(VacancyRating rating, CancellationToken cancellationToken)
    {
        var vacancyRating = await _ratingsDbContext.VacancyRatings
            .SingleOrDefaultAsync(r => r.EntityId == rating.EntityId, cancellationToken);
        if (vacancyRating != null)
        {
            vacancyRating.Value = rating.Value;
            await _ratingsDbContext.SaveChangesAsync(cancellationToken);
            return rating.Id;
        }
        
        var ratingId = await CreateVacancyRatingAsync(rating, cancellationToken);
        return ratingId;
    }
    public async Task<Guid> CreateVacancyRatingAsync(VacancyRating rating, CancellationToken cancellationToken)
    {
        await _ratingsDbContext.VacancyRatings.AddAsync(rating, cancellationToken);
        await _ratingsDbContext.SaveChangesAsync(cancellationToken);
        return rating.Id;
    }
    
    public async Task<Result<VacancyRating?, Error>> GetVacancyRatingByIdAsync(Guid ratingId, CancellationToken cancellationToken)
    {
        try
        {
            var vacancyRating = await _ratingsDbContext.VacancyRatings
                .SingleOrDefaultAsync(r => r.Id == ratingId, cancellationToken);

            if (vacancyRating == null)
            {
                return Errors.General.NotFound(ratingId);
            }
            return vacancyRating;
        }
        catch (Exception e)
        {
            return Errors.General.Failure(e.Message);
        }
    }
}