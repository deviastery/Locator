using Locator.Application.Abstractions;
using Locator.Application.Ratings;
using Locator.Contracts.Vacancies;
using Microsoft.EntityFrameworkCore;

namespace Locator.Application.Vacancies.GetVacanciesWithFiltersQuery;

public class GetVacanciesWithFilters : IQueryHandler<VacancyResponse, GetVacanciesWithFiltersQuery>
{
    private readonly IRatingsReadDbContext _ratingsDbContext;
    private readonly IVacanciesReadDbContext _vacanciesDbContext;
    
    public GetVacanciesWithFilters(
        IRatingsReadDbContext ratingsDbContext, 
        IVacanciesReadDbContext vacanciesDbContext)
    {
        _ratingsDbContext = ratingsDbContext;
        _vacanciesDbContext = vacanciesDbContext;
    }  

    public async Task<VacancyResponse> Handle(
        GetVacanciesWithFiltersQuery query,
        CancellationToken cancellationToken)
    {
        var vacancies = await _vacanciesDbContext.ReadVacancies
            .ToListAsync(cancellationToken);
        long count = vacancies.Count;

        var vacancyRatingIds = vacancies.Select(v => v.RatingId).ToList();
        var ratingsDict = await _ratingsDbContext.ReadVacancyRatings
            .Where(r => vacancyRatingIds.Contains(r.Id))
            .ToDictionaryAsync(r => r.Id, r => r.Value, cancellationToken);
        
        var vacanciesDto = vacancies.Select(v => new VacancyDto(
            v.Id,
            v.Name,
            v.Description,
            v.Salary,
            v.Experience,
            v.RatingId != null && ratingsDict.TryGetValue(v.RatingId.Value, out var r) ? r : null
        ));

        return new VacancyResponse(vacanciesDto, count);
    }
}