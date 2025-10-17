using Locator.Application.Abstractions;
using Locator.Application.Ratings;
using Locator.Contracts.Vacancies;
using Microsoft.EntityFrameworkCore;

namespace Locator.Application.Vacancies.GetVacancyByIdQuery;

public class GetVacancyById : IQueryHandler<VacancyResponse, GetVacancyByIdQuery>
{
    private readonly IRatingsReadDbContext _ratingsDbContext;
    private readonly IVacanciesReadDbContext _vacanciesDbContext;
    
    public GetVacancyById(
        IRatingsReadDbContext ratingsDbContext, 
        IVacanciesReadDbContext vacanciesDbContext)
    {
        _ratingsDbContext = ratingsDbContext;
        _vacanciesDbContext = vacanciesDbContext;
    }  
    public async Task<VacancyResponse> Handle(
        GetVacancyByIdQuery query,
        CancellationToken cancellationToken)
    {
        var vacancy = await _vacanciesDbContext.ReadVacancies
            .Include(v => v.Reviews)
            .Where(v => v.Id == query.Dto.VacancyId)
            .FirstOrDefaultAsync(cancellationToken);

        var rating = await _ratingsDbContext.ReadVacancyRatings
            .Where(r => vacancy != null && r.Id == vacancy.RatingId)
            .FirstOrDefaultAsync(cancellationToken);

        var reviewsDto = vacancy?.Reviews?.Select(r => new ReviewDto(
            r.Id,
            r.Mark,
            r.Comment,
            r.UserName
        ));

        var vacancyDto = new VacancyWithReviewsDto(
            vacancy.Id,
            vacancy.Name,
            vacancy.Description,
            vacancy.Salary,
            vacancy.Experience,
            vacancy.RatingId != null ? rating : null,
            reviewsDto ?? null
        );

        return new VacancyResponse(vacancyDto);
    }
}