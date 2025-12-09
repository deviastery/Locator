using System.Net;
using System.Text.Json;
using Shared.Abstractions;
using Vacancies.Application.Fails.Exceptions;
using Vacancies.Contracts.Responses;

namespace Vacancies.Application.GetRequestRatingByVacancyIdQuery;

public class GetRequestRatingByVacancyId: IQueryHandler<RatingByVacancyIdResponse, GetRequestRatingByVacancyIdQuery>
{
    private readonly HttpClient _httpClient;

    public GetRequestRatingByVacancyId(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<RatingByVacancyIdResponse> Handle(GetRequestRatingByVacancyIdQuery query, CancellationToken cancellationToken)
    {   
        // Get a Rating of a Vacancy
        var request = new HttpRequestMessage(
            HttpMethod.Get, 
            $"https://localhost:5001/api/ratings/vacancies/{query.VacancyId}");
        request.Headers.Add("User-Agent", "Locator/1.0");
        request.Headers.Add("Api-Gateway", "Signed");

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new RatingByVacancyIdResponse();
            }

            throw new GetRatingByVacancyIdNotFoundException(
                $"Rating not found by Vacancy ID={query.VacancyId}");
        }

        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        var ratingResponse = JsonSerializer.Deserialize<RatingByVacancyIdResponse>(json);
        
        return ratingResponse ?? new RatingByVacancyIdResponse();
    } 
}