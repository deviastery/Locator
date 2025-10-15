using Locator.Application.Abstractions;
using Locator.Application.Ratings;
using Locator.Application.Users;
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
            throw new Exception();
        }
        var token = tokenResult.Value;
        
        var resumesResult = await _vacanciesService
            .GetUserResumeIdsAsync(token, cancellationToken);
        if (resumesResult.IsFailure)
        {
            throw new Exception();
        }

        var resume = resumesResult.Value.Resumes
            .FirstOrDefault(r => r.Status.Enum == ResumeStatusEnum.PUBLISHED);
        
        var vacanciesResult = await _vacanciesService
            .GetUserVacanciesAsync(resume.Id, token, cancellationToken);
        if (vacanciesResult.IsFailure)
        {
            throw new Exception();
        }

        var vacancies = vacanciesResult.Value.Vacancies;
        long count = vacanciesResult.Value.Count;
        var page = vacanciesResult.Value.Page;
        var pages = vacanciesResult.Value.Pages;
        var perPage = vacanciesResult.Value.PerPage;
        
        var vacancyIds = vacancies
            .Select(r => r.Id).ToList();
        
        var ratingsDict = await _ratingsDbContext.ReadVacancyRatings
            .Where(r => vacancyIds.Contains(r.EntityId))
            .ToDictionaryAsync(r => r.EntityId, r => r.Value, cancellationToken);
        
        var vacanciesDto = vacancies.Select(v => new FullVacancyDto(
            v,
            ratingsDict.TryGetValue(v.Id, out var rating) ? rating : null
        ));

        return new VacanciesResponse(count, vacanciesDto, page, pages, perPage);
    }
}