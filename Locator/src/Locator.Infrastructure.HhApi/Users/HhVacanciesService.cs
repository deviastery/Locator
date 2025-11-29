using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using CSharpFunctionalExtensions;
using Locator.Application.Users;
using Locator.Contracts.Users.Responses;
using Locator.Contracts.Vacancies.Dto;
using Locator.Contracts.Vacancies.Responses;
using Shared;
using Shared.Thesauruses;
using Errors = Locator.Infrastructure.HhApi.Users.Fails.Errors;

namespace Locator.Infrastructure.HhApi.Users;

public class HhVacanciesService : IVacanciesService
{
    private readonly HttpClient _httpClient;

    public HhVacanciesService(
        HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<Result<ResumesResponse, Error>> GetResumeIdsAsync(
        string accessToken, 
        CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.hh.ru/resumes/mine");
        request.Headers.Add("User-Agent", "Locator/1.0");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return Errors.General.Failure("Get resumes failed");
        }

        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        var resumes = JsonSerializer.Deserialize<ResumesResponse>(json);
        return resumes?.Count != null
            ? resumes
            : Errors.General.NotFound("Resumes not found");
    }
    
    public async Task<Result<EmployeeVacanciesResponse, Error>> GetVacanciesMatchResumeAsync(
        string resumeId, 
        GetVacanciesDto query,
        string accessToken, 
        CancellationToken cancellationToken)
    {
        if ((query.Experience is not null && !Enum.IsDefined(typeof(ExperienceEnum), query.Experience)) || 
            (query.Employment is not null && !Enum.IsDefined(typeof(EmploymentEnum), query.Employment)) ||
            (query.Schedule is not null && !Enum.IsDefined(typeof(ScheduleEnum), query.Schedule)) ||
            (query.Salary?.Currency is not null && !Enum.IsDefined(typeof(CurrencyEnum), query.Salary.Currency)))
        {
            return Errors.General.Validation("Enum query is not valid.");
        }
        
        var queryParams = new Dictionary<string, string?>
        {
            ["page"] = query.Page is not null ? query.Page.ToString() : null,
            ["pages"] = query.Pages is not null ? query.Pages.ToString() : null,
            ["per_page"] = query.PerPage is not null ? query.PerPage.ToString() : null,
            ["search_field"] = query.SearchField,
            ["experience"] = query.Experience,
            ["employment"] = query.Employment,
            ["schedule"] = query.Schedule,
            ["area"] = query.Area is not null ? query.Area.ToString() : null,
            ["salary"] = query.Salary?.Salary.ToString(),
            ["currency"] = query.Salary?.Currency,
        };

        string queryString = string.Join("&", queryParams
            .Where(kvp => kvp.Value != null)
            .Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value ?? string.Empty)}"));
        string url = queryString != string.Empty ? 
            $"https://api.hh.ru/resumes/{resumeId}/similar_vacancies?{queryString}" : 
            $"https://api.hh.ru/resumes/{resumeId}/similar_vacancies";
        
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("User-Agent", "Locator/1.0");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return Errors.General.Validation("Bad request to get vacancies");
        }

        if (!response.IsSuccessStatusCode)
        {
            return Errors.General.Failure("Get vacancies failed");
        }

        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        var vacancies = JsonSerializer.Deserialize<EmployeeVacanciesResponse>(json);
        return vacancies?.Count != null
            ? vacancies
            : Errors.General.NotFound("Vacancies that match resume not found");
    }
    
    public async Task<Result<VacancyDto, Error>> GetVacancyByIdAsync(string vacancyId, string accessToken, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get, 
            $"https://api.hh.ru/vacancies/{vacancyId}");
        request.Headers.Add("User-Agent", "Locator/1.0");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Errors.General.NotFound($"Vacancy not found by ID={vacancyId}");
        }
        if (!response.IsSuccessStatusCode)
        {
            return Errors.General.Failure("Get vacancies failed");
        }

        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        var vacancy = JsonSerializer.Deserialize<VacancyDto>(json);
        return vacancy != null
            ? vacancy
            : Errors.General.NotFound($"Vacancy not found by ID={vacancyId}");
    }
    
    public async Task<Result<EmployeeNegotiationsResponse, Error>> GetNegotiationsByUserIdAsync(
        GetNegotiationsDto query, 
        string accessToken, 
        CancellationToken cancellationToken)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["page"] = query.Page is not null ? query.Page.ToString() : null,
            ["pages"] = query.Pages is not null ? query.Pages.ToString() : null,
            ["per_page"] = query.PerPage is not null ? query.PerPage.ToString() : null,
        };
        
        string queryString = string.Join("&", queryParams
            .Where(kvp => kvp.Value != null)
            .Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value ?? string.Empty)}"));
        string url = queryString != string.Empty ? 
            $"https://api.hh.ru/negotiations?{queryString}" : 
            "https://api.hh.ru/negotiations";
        
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("User-Agent", "Locator/1.0");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return Errors.General.Validation("Bad request to get negotiations");
        }

        if (!response.IsSuccessStatusCode)
        {
            return Errors.General.Failure("Get negotiations failed");
        }

        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        var negotiations = JsonSerializer.Deserialize<EmployeeNegotiationsResponse>(json);
        return negotiations?.Count != null
            ? negotiations
            : Errors.General.NotFound("Negotiations not found");
    }
    
    public async Task<Result<NegotiationDto, Error>> GetNegotiationByVacancyIdAsync(
        long vacancyId, 
        string accessToken, 
        CancellationToken cancellationToken)
    {
        string url = $"https://api.hh.ru/negotiations?vacancy_id={vacancyId}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("User-Agent", "Locator/1.0");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        
        switch (response.StatusCode)
        {
            case HttpStatusCode.BadRequest:
                return Errors.General.Validation("Bad request to get negotiations");
            case HttpStatusCode.NotFound:
                return Errors.General.NotFound($"Negotiation not found by vacancy ID={vacancyId}");
        }
        if (!response.IsSuccessStatusCode)
        {
            return Errors.General.Failure("Get negotiations failed");
        }

        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        var negotiations = JsonSerializer.Deserialize<EmployeeNegotiationsResponse>(json);
        
        var negotiation = negotiations?.Negotiations?.FirstOrDefault();
        return negotiation != null
            ? negotiation
            : Errors.General.NotFound($"Negotiation not found by vacancy ID={vacancyId}");
    }
    
    public async Task<Result<NegotiationDto, Error>> GetNegotiationByIdAsync(
        long negotiationId, 
        string accessToken, 
        CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get, 
            $"https://api.hh.ru/negotiations/{negotiationId}");
        request.Headers.Add("User-Agent", "Locator/1.0");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);        
        switch (response.StatusCode)
        {
            case HttpStatusCode.BadRequest:
                return Errors.General.Validation("Bad request to get negotiation");
            case HttpStatusCode.NotFound:
                return Errors.General.NotFound($"Negotiation not found by ID={negotiationId}");
        }
        if (!response.IsSuccessStatusCode)
        {
            return Errors.General.Failure("Get negotiation failed");
        }

        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        var negotiation = JsonSerializer.Deserialize<NegotiationDto>(json);
        
        return negotiation != null
            ? negotiation
            : Errors.General.NotFound($"Negotiation not found by ID={negotiationId}");
    }
    
    public async Task<Result<int, Error>> GetDaysAfterApplyingAsync(
        long negotiationId, 
        string accessToken, 
        CancellationToken cancellationToken)
    {
        // Get Negotiation by ID
        (_, bool isFailure, NegotiationDto? negotiation, Error? error) = 
            await GetNegotiationByIdAsync(negotiationId, accessToken, cancellationToken);
        if (isFailure)
        {
            return error;
        }

        if (negotiation == null)
        {
            return Errors.General.NotFound($"Negotiation not found by ID={negotiationId}");
        }
        
        // Calculate count of days after applying
        string negotiationCreatedAtString = negotiation.CreatedAt;
        var negotiationCreatedAt = DateTimeOffset.Parse(negotiationCreatedAtString);
        var nowInSameOffset = DateTimeOffset.UtcNow.ToOffset(negotiationCreatedAt.Offset);
        var diff = nowInSameOffset - negotiationCreatedAt;
        int daysDifference = (int)Math.Floor(diff.TotalDays);
        
        return daysDifference;
    }
}