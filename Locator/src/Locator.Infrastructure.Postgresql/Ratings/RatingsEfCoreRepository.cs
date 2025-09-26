using CSharpFunctionalExtensions;
using Locator.Application.Ratings;
using Locator.Application.Vacancies.Fails;
using Locator.Domain.Ratings;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Locator.Infrastructure.Postgresql.Ratings;

public class RatingsEfCoreRepository : IRatingsRepository
{
    private readonly LocatorDbContext _locatorDbContext;

    public RatingsEfCoreRepository(LocatorDbContext locatorDbContext)
    {
        _locatorDbContext = locatorDbContext;
    }

    public async Task<Guid> UpdateVacancyRatingAsync(VacancyRating rating, CancellationToken cancellationToken)
    {
        var vacancyRating = await _locatorDbContext.VacancyRatings
            .SingleOrDefaultAsync(r => r.EntityId == rating.EntityId, cancellationToken);
        if (vacancyRating != null)
        {
            vacancyRating.Value = rating.Value;
            await _locatorDbContext.SaveChangesAsync(cancellationToken);
            return rating.Id;
        }
        
        var ratingId = await CreateVacancyRatingAsync(rating, cancellationToken);
        return ratingId;
    }
    public async Task<Guid> CreateVacancyRatingAsync(VacancyRating rating, CancellationToken cancellationToken)
    {
        await _locatorDbContext.VacancyRatings.AddAsync(rating, cancellationToken);
        await _locatorDbContext.SaveChangesAsync(cancellationToken);
        return rating.Id;
    }
    
    public async Task<Result<VacancyRating?, Error>> GetVacancyRatingByIdAsync(Guid ratingId, CancellationToken cancellationToken)
    {
        try
        {
            var vacancyRating = await _locatorDbContext.VacancyRatings
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

    public async Task<Result<Dictionary<Guid, double?>, Error>> GetVacancyRatingsByIdsAsync(IEnumerable<Guid> ratingIds,
        CancellationToken cancellationToken)
    {
        try
        {
            var vacancyRatings = await _locatorDbContext.VacancyRatings
                .ToListAsync(cancellationToken);

            if (vacancyRatings.Count == 0)
            {
                return Errors.FailGetVacancyRatings();
            }
            
            var ratingValuesDict = new Dictionary<Guid, double?>();
            
            foreach (var id in ratingIds)
            {
                ratingValuesDict[id] = null;
            }
            foreach (var rating in vacancyRatings)
            {
                ratingValuesDict[rating.Id] = rating.Value;
            }

            return ratingValuesDict;
        }
        catch (Exception e)
        {
            return Errors.General.Failure(e.Message);
        }
    }
}