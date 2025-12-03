using Shared.Abstractions;
using Vacancies.Contracts.Dto;
using Vacancies.Contracts.Responses;
using Microsoft.EntityFrameworkCore;

namespace Vacancies.Application.GetReviewsByVacancyIdQuery;

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

        var reviewsDto = reviews.Select(r => new ReviewDto(
            r.Id,
            r.Mark,
            r.Comment,
            r.UserName
        ));

        return new ReviewsByVacancyIdResponse(reviewsDto);
    }
}