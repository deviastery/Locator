using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Ratings.Application;
using Ratings.Application.Fails;
using Ratings.Domain;
using Shared;

namespace Ratings.Infrastructure.Postgresql;

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
                return Errors.General.NotFound($"Rating not found by ID={rating.Id}");
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
    
    public async Task<Result<VacancyRating, Error>> GetRatingByVacancyIdAsync(
        long vacancyId, CancellationToken cancellationToken)
    {
        try
        {
            var ratingRecord = await _ratingsDbContext.ReadVacancyRatings
                .SingleOrDefaultAsync(
                    r => r.EntityId == vacancyId, 
                    cancellationToken);

            if (ratingRecord == null)
            {
                return Errors.General.NotFound($"Rating not found by Vacancy ID={vacancyId}");
            }
            
            return ratingRecord;
        }
        catch (Exception e)
        {
            return Errors.General.Failure(e.Message);
        }
    }
    
    public async Task<Result<VacancyRating[], Error>> GetRatingsAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            var ratingsRecord = await _ratingsDbContext.ReadVacancyRatings
                .ToListAsync(cancellationToken);
            
            return ratingsRecord.ToArray();
        }
        catch (Exception e)
        {
            return Errors.General.Failure(e.Message);
        }
    }
}