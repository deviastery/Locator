using Microsoft.EntityFrameworkCore;
using Ratings.Contracts.Dto;
using Ratings.Contracts.Responses;
using Shared.Abstractions;

namespace Ratings.Application.GetVacancyRatingsQuery;

public class GetVacancyRatings : IQueryHandler<VacancyRatingsResponse, GetVacancyRatingsQuery>
{
    private readonly IRatingsReadDbContext _ratingsDbContext;
    
    public GetVacancyRatings(
        IRatingsReadDbContext ratingsDbContext)
    {
        _ratingsDbContext = ratingsDbContext;
    }  
    public async Task<VacancyRatingsResponse> Handle(
        GetVacancyRatingsQuery query,
        CancellationToken cancellationToken)
    {
        var ratings = await _ratingsDbContext.ReadVacancyRatings
            .ToListAsync(cancellationToken);
        
        if (ratings.Count == 0)
        {
            return new VacancyRatingsResponse(null);
        }

        var ratingsDto = ratings.Select(r => 
            new VacancyRatingDto(
                r.Id,
                r.Value,
                r.EntityId));

        return new VacancyRatingsResponse(ratingsDto.ToArray());
    }
}