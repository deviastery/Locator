using Locator.Application.Abstractions;
using Locator.Application.Ratings;
using Locator.Application.Users;
using Locator.Application.Users.Fails.Exceptions;
using Locator.Application.Vacancies.Fails.Exceptions;
using Locator.Contracts.Users;
using Locator.Contracts.Vacancies;
using Microsoft.EntityFrameworkCore;

namespace Locator.Application.Vacancies.GetVacanciesWithFiltersQuery;

public class GetVacanciesWithFilters : IQueryHandler<VacanciesResponse, GetVacanciesWithFiltersQuery>
{
    private readonly IRatingsReadDbContext _ratingsDbContext;
    private readonly IEmployeeVacanciesService _vacanciesService;
    private readonly IAuthService _authService;
    
    public GetVacanciesWithFilters(
        IRatingsReadDbContext ratingsDbContext, 
        IEmployeeVacanciesService vacanciesService, 
        IAuthService authService)
    {
        _ratingsDbContext = ratingsDbContext;
        _vacanciesService = vacanciesService;
        _authService = authService;
    }  

    public async Task<VacanciesResponse> Handle(
        GetVacanciesWithFiltersQuery query,
        CancellationToken cancellationToken)
    {
        var tokenResult = await _authService
            .GetValidEmployeeAccessTokenAsync(query.Dto.UserId, cancellationToken);
        if (tokenResult.IsFailure)
        {
            throw new GetValidEmployeeAccessTokenException();
        }
        var token = tokenResult.Value;
        
        var resumesResult = await _vacanciesService
            .GetResumeIdsAsync(token, cancellationToken);
        if (resumesResult.IsFailure)
        {
            throw new GetResumeException();
        }
        
        var resume = resumesResult.Value?.Resumes?
            .FirstOrDefault(r => r.Status?.Id == ResumeStatusEnum.PUBLISHED.ToString().ToLower());
        if (resume is null)
        {
            throw new GetValidResumeNotFoundException();
        }
        
        var vacanciesResult = await _vacanciesService
            .GetVacanciesMatchResumeAsync(resume.Id, token, cancellationToken);
        if (vacanciesResult.IsFailure)
        {
            throw new GetVacanciesMatchResumeFailureException();
        }
        
        var vacancies = vacanciesResult.Value.Vacancies;
        long count = vacanciesResult.Value.Count;
        var page = vacanciesResult.Value.Page;
        var pages = vacanciesResult.Value.Pages;
        var perPage = vacanciesResult.Value.PerPage;
        
        var vacancyIds = vacancies
            .Select(v => long.Parse(v.Id)).ToList();
        
        var ratingsDict = await _ratingsDbContext.ReadVacancyRatings
            .Where(r => vacancyIds.Contains(r.EntityId))
            .ToDictionaryAsync(r => r.EntityId, r => r.Value, cancellationToken);
        
        var vacanciesDto = vacancies.Select(v => new FullVacancyDto(
            long.Parse(v.Id),
            v,
            ratingsDict.TryGetValue(long.Parse(v.Id), out var rating) ? rating : null
        ));
        
        return new VacanciesResponse(count, vacanciesDto, page, pages, perPage);
    }
}