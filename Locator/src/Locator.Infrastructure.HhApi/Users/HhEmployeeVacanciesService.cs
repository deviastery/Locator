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

namespace Locator.Infrastructure.HhApi.Users;

public class HhEmployeeVacanciesService : IEmployeeVacanciesService
{
    private readonly HttpClient _httpClient;
    private readonly HhApiConfiguration _config;
    private readonly UsersDbContext _usersDbContext;
    private readonly IUsersRepository _usersRepository;

    public HhEmployeeVacanciesService(
        HttpClient httpClient, 
        IOptions<HhApiConfiguration> config, 
        UsersDbContext usersDbContext,
        IUsersRepository usersRepository)
    {
        _httpClient = httpClient;
        _config = config.Value; 
        _usersDbContext = usersDbContext;
        _usersRepository = usersRepository;
    }
    
    public async Task<Result<ResumesResponse, Error>> GetUserResumeIdsAsync(
        string accessToken, 
        CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.hh.ru/resumes/mine");
        request.Headers.Add("User-Agent", "Locator/1.0");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return Errors.GetResumesFailed();

        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        var resumes = JsonSerializer.Deserialize<ResumesResponse>(json);
        return resumes?.Count != null
            ? resumes
            : Errors.MissingResumes();
    }
    
    public async Task<Result<EmployeeVacanciesResponse, Error>> GetUserVacanciesAsync(
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
            return Errors.GetVacanciesFailed();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var vacancies = JsonSerializer.Deserialize<EmployeeVacanciesResponse>(json);
        return vacancies?.Count != null
            ? vacancies
            : Errors.MissingVacancies();
    }
}