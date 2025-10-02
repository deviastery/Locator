using Locator.Application.Abstractions;
using Locator.Application.Vacancies.GetReviewsByVacancyIdQuery;
using Locator.Contracts.Ratings;
using Microsoft.EntityFrameworkCore;
using VacancyRatingDto = Locator.Contracts.Ratings.VacancyRatingDto;

namespace Locator.Application.Ratings.GetRatingByVacancyIdQuery;

public class GetRatingByVacancyId : IQueryHandler<RatingByVacancyIdResponse, GetRatingByVacancyIdQuery>
{
    private readonly IRatingsReadDbContext _ratingsDbContext;
    
    public GetRatingByVacancyId(
        IRatingsReadDbContext ratingsDbContext)
    {
        _ratingsDbContext = ratingsDbContext;
    }  
    public async Task<RatingByVacancyIdResponse> Handle(
        GetRatingByVacancyIdQuery query,
        CancellationToken cancellationToken)
    {
        var rating = await _ratingsDbContext.ReadVacancyRatings
            .Where(r => r.EntityId == query.Dto.VacancyId)
            .FirstOrDefaultAsync(cancellationToken);

        var ratingDto = new VacancyRatingDto(
            rating.Id,
            rating.Value,
            rating.EntityId,
            rating.EntityType
        );

        return new RatingByVacancyIdResponse(ratingDto);
    }
}