using Locator.Application.Abstractions;
using Locator.Contracts.Vacancies;
using Microsoft.EntityFrameworkCore;

namespace Locator.Application.Vacancies.GetReviewsByVacancyIdQuery;

public class GetReviewsByVacancyId : IQueryHandler<ReviewsByVacancyIdResponse, GetReviewsByVacancyIdQuery>
{
    private readonly IVacanciesReadDbContext _vacanciesDbContext;
    
    public GetReviewsByVacancyId(
        IVacanciesReadDbContext vacanciesDbContext)
    {
        _vacanciesDbContext = vacanciesDbContext;
    }  
    public async Task<ReviewsByVacancyIdResponse> Handle(
        GetReviewsByVacancyIdQuery query,
        CancellationToken cancellationToken)
    {
        var reviews = await _vacanciesDbContext.ReadReviews
            .Where(r => r.VacancyId == query.VacancyId)
            .ToListAsync(cancellationToken);

        var reviewsDto = reviews?.Select(r => new ReviewDto(
            r.Id,
            r.Mark,
            r.Comment,
            r.UserName
        ));

        return new ReviewsByVacancyIdResponse(reviewsDto ?? new List<ReviewDto>());
    }
}