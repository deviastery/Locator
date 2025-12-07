using System.Text.Json;
using HeadHunter.Contracts;
using Shared.Abstractions;
using Shared.Thesauruses;
using Vacancies.Application.Fails.Exceptions;
using Vacancies.Contracts.Dto;
using Vacancies.Contracts.Responses;

namespace Vacancies.Application.GetVacanciesWithFiltersQuery;

public class GetVacanciesWithFilters : IQueryHandler<VacanciesResponse, GetVacanciesWithFiltersQuery>
{
    private readonly HttpClient _httpClient;
    private readonly IVacanciesContract _vacanciesContract;
    
    public GetVacanciesWithFilters(
        HttpClient httpClient,
        IVacanciesContract vacanciesContract)
    {
        _vacanciesContract = vacanciesContract;
        _httpClient = httpClient;
    }  

    public async Task<VacanciesResponse> Handle(
        GetVacanciesWithFiltersQuery query,
        CancellationToken cancellationToken)
    {
        // Get Employee access token
        var tokenRequest = new HttpRequestMessage(
            HttpMethod.Get, 
            $"https://localhost:7146/api/users/auth/employee_token/{query.Dto.UserId}");
        tokenRequest.Headers.Add("User-Agent", "Locator/1.0");

        var tokenResponse = await _httpClient.SendAsync(tokenRequest, cancellationToken);
        if (!tokenResponse.IsSuccessStatusCode)
        {
            throw new GetValidEmployeeAccessTokenException();
        }

        string tokenJson = await tokenResponse.Content.ReadAsStringAsync(cancellationToken);
        var employeeTokenResponse = JsonSerializer.Deserialize<EmployeeTokenResponse>(tokenJson);
        if (employeeTokenResponse?.EmployeeToken?.Token == null)
        {
            throw new GetValidEmployeeAccessTokenException();
        }
        
        string? token = employeeTokenResponse.EmployeeToken.Token;
        
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
        var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5001/api/ratings");
        request.Headers.Add("User-Agent", "Locator/1.0");

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new GetRatingsFailureException();
        }

        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        var ratingsResponse = JsonSerializer.Deserialize<VacancyRatingsResponse>(json);
        if (ratingsResponse?.VacancyRatings == null || ratingsResponse.VacancyRatings?.Length == 0)
        {
            throw new GetRatingsFailureException();
        }
        
        var ratings = ratingsResponse.VacancyRatings;
        
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