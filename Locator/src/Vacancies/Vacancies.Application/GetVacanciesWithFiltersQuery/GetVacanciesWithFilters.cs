using HeadHunter.Contracts;
using Ratings.Contracts;
using Shared.Abstractions;
using Shared.Thesauruses;
using Vacancies.Application.Fails.Exceptions;
using Vacancies.Contracts.Dto;
using Vacancies.Contracts.Responses;

namespace Vacancies.Application.GetVacanciesWithFiltersQuery;

public class GetVacanciesWithFilters : IQueryHandler<VacanciesResponse, GetVacanciesWithFiltersQuery>
{
    private readonly IRatingsContract _ratingsContract;
    private readonly IVacanciesContract _vacanciesContract;
    private readonly IAuthContract _authContract;
    
    public GetVacanciesWithFilters(
        IRatingsContract ratingsContract, 
        IVacanciesContract vacanciesContract, 
        IAuthContract authContract)
    {
        _ratingsContract = ratingsContract;
        _vacanciesContract = vacanciesContract;
        _authContract = authContract;
    }  

    public async Task<VacanciesResponse> Handle(
        GetVacanciesWithFiltersQuery query,
        CancellationToken cancellationToken)
    {
        // Get Employee access token
        string? token = await _authContract.GetEmployeeTokenAsync(query.Dto.UserId, cancellationToken);
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
        
        // Get Vacancies that match resume
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
        var ratings = await _ratingsContract.GetVacancyRatingsDtoAsync(cancellationToken);
        if (ratings.Length == 0)
        {
            throw new GetRatingsFailureException();
        }
        
        var ratingsDict = ratings
            .Where(r => vacancyIds.Contains(r.EntityId))
            .ToDictionary(r => r.EntityId, r => r.Value);
        
        // Vacancies with their Ratings
        var vacanciesDto = vacancies.Select(v => new FullVacancyDto(
            long.Parse(v.Id),
            v,
            ratingsDict.TryGetValue(long.Parse(v.Id), out double rating) ? rating : null));
        
        return new VacanciesResponse(count, vacanciesDto, page, pages, perPage);
    }
}