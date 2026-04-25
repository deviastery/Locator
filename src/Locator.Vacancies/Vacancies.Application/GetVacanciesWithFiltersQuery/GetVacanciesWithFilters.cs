using HeadHunter.Contracts;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions;
using Shared.Thesauruses;
using Vacancies.Application.Fails.Exceptions;
using Vacancies.Contracts.Dto;
using Vacancies.Contracts.Responses;

namespace Vacancies.Application.GetVacanciesWithFiltersQuery;

public class GetVacanciesWithFilters : IQueryHandler<VacanciesResponse, GetVacanciesWithFiltersQuery>
{
    private readonly IQueryHandler<EmployeeTokenResponse, GetRequestEmployeeTokenQuery.GetRequestEmployeeTokenQuery> 
        _getRequestEmployeeTokenQuery;    
    private readonly IQueryHandler<VacancyRatingsResponse, GetRequestRatingsQuery.GetRequestRatingsQuery> 
        _getRequestRatingsQuery;
    private readonly IVacanciesContract _vacanciesContract;
    private readonly IVacanciesReadDbContext _vacanciesReadDbContext;
    
    public GetVacanciesWithFilters(
        IQueryHandler<EmployeeTokenResponse, GetRequestEmployeeTokenQuery.GetRequestEmployeeTokenQuery> 
            getRequestEmployeeTokenQuery,
        IQueryHandler<VacancyRatingsResponse, GetRequestRatingsQuery.GetRequestRatingsQuery> 
            getRequestRatingsQuery,
        IVacanciesContract vacanciesContract,
        IVacanciesReadDbContext vacanciesReadDbContext)
    {
        _getRequestRatingsQuery = getRequestRatingsQuery;
        _getRequestEmployeeTokenQuery = getRequestEmployeeTokenQuery;
        _vacanciesContract = vacanciesContract;
        _vacanciesReadDbContext = vacanciesReadDbContext;
    }  

    public async Task<VacanciesResponse> Handle(
        GetVacanciesWithFiltersQuery query,
        CancellationToken cancellationToken)
    {
        // Get Employee access token
        var getRequestEmployeeTokenQuery = new GetRequestEmployeeTokenQuery.GetRequestEmployeeTokenQuery(
            query.Dto.UserId);
        var employeeTokenResponse = await _getRequestEmployeeTokenQuery.Handle(
            getRequestEmployeeTokenQuery,
            cancellationToken);
        if (employeeTokenResponse.EmployeeToken?.Token == null)
        {
            throw new GetValidEmployeeAccessTokenException();
        }
        
        string token = employeeTokenResponse.EmployeeToken.Token;
        
        if (token is null)
        {
            throw new GetValidEmployeeAccessTokenException();
        }

        // Get all resume IDs of user
        var resumesResult = await _vacanciesContract
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
        
        // Get Vacancies that match resume and filters
        var vacanciesResult = await _vacanciesContract
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
        var ratingsResponse = await _getRequestRatingsQuery.Handle(
            new GetRequestRatingsQuery.GetRequestRatingsQuery(),
            cancellationToken);
        
        if (ratingsResponse.VacancyRatings == null || ratingsResponse.VacancyRatings?.Length == 0)
        {
            throw new GetRatingsFailureException();
        }
        
        var ratings = ratingsResponse.VacancyRatings;
        
        var ratingsDict = ratings
            ?.Where(r => vacancyIds.Contains(r.EntityId))
            .ToDictionary(r => r.EntityId, r => r.Value);
        
        // Vacancies with their Ratings
        var vacanciesDto = vacancies.Select(v => new FullVacancyDto(
            long.Parse(v.Id),
            v,
            ratingsDict != null && ratingsDict.TryGetValue(long.Parse(v.Id), out double rating) ? rating : null));
        
        // Rating and Review filters
        if (query.Dto.Query.OnlyWithReviews == true)
        {
            var vacanciesWithReviewsIds = await _vacanciesReadDbContext.ReadReviews
                .Where(r => vacancyIds.Contains(r.VacancyId))
                .Select(r => r.VacancyId)
                .Distinct()
                .ToListAsync(cancellationToken);

            vacanciesDto = vacanciesDto.Where(v => vacanciesWithReviewsIds.Contains(v.Id));
        }
        if (query.Dto.Query.MinRating.HasValue)
        {
            vacanciesDto = vacanciesDto.Where(v => v.Rating >= query.Dto.Query.MinRating.Value);
        }
        if (query.Dto.Query.MaxRating.HasValue)
        {
            vacanciesDto = vacanciesDto.Where(v => v.Rating <= query.Dto.Query.MaxRating.Value);
        }

        var filteredVacancies = vacanciesDto.ToList();
        
        bool isLocalFilterApplied = query.Dto.Query.OnlyWithReviews == true || 
                                    query.Dto.Query.MinRating.HasValue || 
                                    query.Dto.Query.MaxRating.HasValue;

        if (isLocalFilterApplied)
        {
            count = filteredVacancies.Count; 
            pages = 1;
            page = 1;
        }
        
        return new VacanciesResponse(count, filteredVacancies, page, pages, perPage);
    }
}