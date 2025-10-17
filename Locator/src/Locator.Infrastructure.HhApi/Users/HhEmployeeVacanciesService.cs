using System.Net.Http.Headers;
using System.Text.Json;
using CSharpFunctionalExtensions;
using Locator.Application.Users;
using Locator.Application.Users.Fails;
using Locator.Contracts.Users;
using Locator.Contracts.Vacancies;
using Locator.Domain.Users;
using Locator.Infrastructure.Postgresql.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared;
using Errors = Locator.Infrastructure.HhApi.Users.Fails.Errors;

namespace Locator.Infrastructure.HhApi.Users;

public class HhEmployeeVacanciesService : IEmployeeVacanciesService
{
    private readonly HttpClient _httpClient;

    public HhEmployeeVacanciesService(
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
            return Errors.GetResumesFailed();
        }

        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        var resumes = JsonSerializer.Deserialize<ResumesResponse>(json);
        return resumes?.Count != null
            ? resumes
            : Errors.MissingResumes();
    }
    
    public async Task<Result<EmployeeVacanciesResponse, Error>> GetVacanciesMatchResumeAsync(
        string resumeId, 
        string accessToken, 
        CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get, 
            $"https://api.hh.ru/resumes/{resumeId}/similar_vacancies");
        request.Headers.Add("User-Agent", "Locator/1.0");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return Errors.GetVacanciesFailed();
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var vacancies = JsonSerializer.Deserialize<EmployeeVacanciesResponse>(json);
        return vacancies?.Count != null
            ? vacancies
            : Errors.MissingVacancies();
    }

    public async Task<Result<VacancyDto, Error>> GetVacancyByIdAsync(string vacancyId, string accessToken, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get, 
            $"https://api.hh.ru/vacancies/{vacancyId}");
        request.Headers.Add("User-Agent", "Locator/1.0");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return Errors.GetVacanciesFailed();
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var vacancy = JsonSerializer.Deserialize<VacancyDto>(json);
        return vacancy != null
            ? vacancy
            : Errors.MissingVacancies();
    }
}