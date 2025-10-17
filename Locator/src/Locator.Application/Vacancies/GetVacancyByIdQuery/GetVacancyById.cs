using Locator.Application.Abstractions;
using Locator.Application.Ratings;
using Locator.Application.Users;
using Locator.Application.Users.Fails.Exceptions;
using Locator.Application.Vacancies.Fails.Exceptions;
using Locator.Contracts.Vacancies;
using Microsoft.EntityFrameworkCore;

namespace Locator.Application.Vacancies.GetVacancyByIdQuery;

public class GetVacancyById : IQueryHandler<VacancyResponse, GetVacancyByIdQuery>
{
    private readonly IRatingsReadDbContext _ratingsDbContext;
    private readonly IVacanciesReadDbContext _vacanciesDbContext;
    private readonly IEmployeeVacanciesService _vacanciesService;
    private readonly IAuthService _authService;
    
    public GetVacancyById(
        IRatingsReadDbContext ratingsDbContext, 
        IVacanciesReadDbContext vacanciesDbContext,
        IEmployeeVacanciesService vacanciesService, 
        IAuthService authService)
    {
        _ratingsDbContext = ratingsDbContext;
        _vacanciesDbContext = vacanciesDbContext;
        _vacanciesService = vacanciesService;
        _authService = authService;
    }  
    public async Task<VacancyResponse> Handle(
        GetVacancyByIdQuery query,
        CancellationToken cancellationToken)
    {
        var tokenResult = await _authService
            .GetValidEmployeeAccessTokenAsync(query.Dto.UserId, cancellationToken);
        if (tokenResult.IsFailure)
        {
            throw new GetValidEmployeeAccessTokenException();
        }
        var token = tokenResult.Value;
        
        var vacancyResult = await _vacanciesService
            .GetVacancyByIdAsync(query.Dto.VacancyId.ToString(), token, cancellationToken);
        if (vacancyResult.IsFailure || vacancyResult.Value == null)
        {
            throw new GetVacancyByIdException();
        }
        
        var vacancy = vacancyResult.Value;

        var rating = await _ratingsDbContext.ReadVacancyRatings
            .Where(r => r.EntityId == query.Dto.VacancyId)
            .FirstOrDefaultAsync(cancellationToken);
        
        var reviews = await _vacanciesDbContext.ReadReviews
            .Where(r => r.VacancyId == query.Dto.VacancyId)
            .ToListAsync(cancellationToken);

        var reviewsDto = reviews.Select(r => new ReviewDto(
            r.Id,
            r.Mark,
            r.Comment,
            r.UserName
        ));

        var vacancyDto = new FullVacancyDto(query.Dto.VacancyId, vacancy, rating?.Value);

        return new VacancyResponse(vacancyDto, reviewsDto);
    }
}