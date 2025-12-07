using System.Text.Json;
using CSharpFunctionalExtensions;
using HeadHunter.Contracts;
using HeadHunter.Contracts.Dto;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Abstractions;
using Vacancies.Application.Fails.Exceptions;
using Vacancies.Contracts.Dto;
using Vacancies.Contracts.Responses;

namespace Vacancies.Application.GetVacancyByIdQuery;

public class GetVacancyById : IQueryHandler<VacancyResponse, GetVacancyByIdQuery>
{
    private readonly HttpClient _httpClient;
    private readonly IVacanciesReadDbContext _vacanciesDbContext;
    private readonly IVacanciesContract _vacanciesContract;
    
    public GetVacancyById(
        HttpClient httpClient,
        IVacanciesReadDbContext vacanciesDbContext,
        IVacanciesContract vacanciesContract)
    {
        _vacanciesDbContext = vacanciesDbContext;
        _vacanciesContract = vacanciesContract;
        _httpClient = httpClient;
    }  
    public async Task<VacancyResponse> Handle(
        GetVacancyByIdQuery query,
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

        // Get a Vacancy by ID
        Result<VacancyDto, Error> vacancyByIdResult = await _vacanciesContract
            .GetVacancyByIdAsync(query.Dto.VacancyId.ToString(), token, cancellationToken);
        if (vacancyByIdResult.IsFailure && vacancyByIdResult.Error.Code == "record.not.found")
        {
            throw new GetVacancyByIdNotFoundException($"Vacancy not found by ID={query.Dto.VacancyId}");
        }        
        if (vacancyByIdResult.IsFailure || vacancyByIdResult.Value == null)
        {
            throw new GetVacancyByIdFailureException();
        }

        // Get a Rating of a Vacancy
        var request = new HttpRequestMessage(
            HttpMethod.Get, 
            $"https://localhost:5001/api/ratings/vacancies/{query.Dto.VacancyId}");
        request.Headers.Add("User-Agent", "Locator/1.0");

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new GetRatingByVacancyIdNotFoundException(
                $"Rating not found by Vacancy ID={query.Dto.VacancyId}");
        }

        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        var ratingResponse = JsonSerializer.Deserialize<RatingByVacancyIdResponse>(json);
        if (ratingResponse?.Rating == null)
        {
            throw new GetRatingByVacancyIdNotFoundException(
                $"Rating not found by Vacancy ID={query.Dto.VacancyId}");
        }
        
        var rating = ratingResponse?.Rating;
        if (rating is null)
        {
            throw new GetRatingByVacancyIdNotFoundException(
                $"Rating not found by Vacancy ID={query.Dto.VacancyId}");
        }
        
        // Get Reviews of a Vacancy
        var reviews = await _vacanciesDbContext.ReadReviews
            .Where(r => r.VacancyId == query.Dto.VacancyId)
            .ToListAsync(cancellationToken);

        var reviewsDto = reviews.Select(r => new ReviewDto(
            r.Id,
            r.Mark,
            r.Comment,
            r.UserName));

        // Get Vacancy with Reviews and Rating
        var vacancyDto = new FullVacancyDto(query.Dto.VacancyId, vacancyByIdResult.Value, rating?.Value);

        return new VacancyResponse(vacancyDto, reviewsDto);
    }
}