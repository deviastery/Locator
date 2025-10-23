using CSharpFunctionalExtensions;
using Locator.Application.Abstractions;
using Locator.Application.Ratings;
using Locator.Application.Users;
using Locator.Application.Vacancies.Fails.Exceptions;
using Locator.Contracts.Vacancies.Dtos;
using Locator.Contracts.Vacancies.Responses;
using Locator.Domain.Thesauruses;
using Microsoft.EntityFrameworkCore;

namespace Locator.Application.Vacancies.GetVacanciesWithFiltersQuery;

public class GetVacanciesWithFilters : IQueryHandler<VacanciesResponse, GetVacanciesWithFiltersQuery>
{
    private readonly IRatingsReadDbContext _ratingsDbContext;
    private readonly IVacanciesService _vacanciesService;
    private readonly IAuthService _authService;
    
    public GetVacanciesWithFilters(
        IRatingsReadDbContext ratingsDbContext, 
        IVacanciesService vacanciesService, 
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
        (_, bool isFailure, string? token) = await _authService
            .GetValidEmployeeAccessTokenAsync(query.Dto.UserId, cancellationToken);
        if (isFailure)
        {
            throw new GetValidEmployeeAccessTokenException();
        }

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
            .GetVacanciesMatchResumeAsync(resume.Id, query.Dto.Query, token, cancellationToken);
        if (vacanciesResult.IsFailure || vacanciesResult.Value.Vacancies == null)
        {
            throw new GetVacanciesMatchResumeFailureException();
        }
        
        var vacancies = vacanciesResult.Value.Vacancies;
        long count = vacanciesResult.Value.Count;
        int page = vacanciesResult.Value.Page;
        int pages = vacanciesResult.Value.Pages;
        int perPage = vacanciesResult.Value.PerPage;
        
        var vacancyIds = vacancies
            .Select(v => long.Parse(v.Id)).ToList();
        
        var ratingsDict = await _ratingsDbContext.ReadVacancyRatings
            .Where(r => vacancyIds.Contains(r.EntityId))
            .ToDictionaryAsync(r => r.EntityId, r => r.Value, cancellationToken);
        
        var vacanciesDto = vacancies.Select(v => new FullVacancyDto(
            long.Parse(v.Id),
            v,
            ratingsDict.TryGetValue(long.Parse(v.Id), out double rating) ? rating : null
        ));
        
        return new VacanciesResponse(count, vacanciesDto, page, pages, perPage);
    }
}