using CSharpFunctionalExtensions;
using Locator.Application.Abstractions;
using Locator.Application.Ratings;
using Locator.Application.Users;
using Locator.Application.Vacancies.Fails.Exceptions;
using Locator.Contracts.Vacancies.Dto;
using Locator.Contracts.Vacancies.Responses;
using Microsoft.EntityFrameworkCore;
using Shared.Thesauruses;

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
        // Get Employee access token
        string? token = await _authService.GetEmployeeTokenAsync(query.Dto.UserId, cancellationToken);
        if (token is null)
        {
            throw new GetValidEmployeeAccessTokenException();
        }

        // Get all resume IDs of user
        var resumesResult = await _vacanciesService
            .GetResumeIdsAsync(token, cancellationToken);
        if (resumesResult.IsFailure && resumesResult.Error.Code == "record.not.found")
        {
            throw new GetValidResumeNotFoundException("Resumes not found");
        }
        if (resumesResult.IsFailure)
        {
            throw new GetResumeFailureException();
        }
        
        var resume = resumesResult.Value?.Resumes?
            .FirstOrDefault(r => r.Status?.Id == nameof(ResumeStatusEnum.PUBLISHED).ToLower());
        if (resume is null)
        {
            throw new GetValidResumeNotFoundException("Resumes not found");
        }
        
        // Get Vacancies that match resume
        var vacanciesResult = await _vacanciesService
            .GetVacanciesMatchResumeAsync(resume.Id, query.Dto.Query, token, cancellationToken);

        switch (vacanciesResult.IsFailure)
        {
            case true when vacanciesResult.Error.Code == "value.is.invalid":
                throw new GetVacanciesMatchResumeValidationException(vacanciesResult.Error.Message);
            case true when vacanciesResult.Error.Code == "record.not.found":
                throw new GetVacanciesMatchResumeNotFoundException("Vacancies that match resume not found");
        }

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
        
        // Get Ratings of all Vacancies
        var ratingsDict = await _ratingsDbContext.ReadVacancyRatings
            .Where(r => vacancyIds.Contains(r.EntityId))
            .ToDictionaryAsync(r => r.EntityId, r => r.Value, cancellationToken);
        
        // Vacancies with their Ratings
        var vacanciesDto = vacancies.Select(v => new FullVacancyDto(
            long.Parse(v.Id),
            v,
            ratingsDict.TryGetValue(long.Parse(v.Id), out double rating) ? rating : null));
        
        return new VacanciesResponse(count, vacanciesDto, page, pages, perPage);
    }
}