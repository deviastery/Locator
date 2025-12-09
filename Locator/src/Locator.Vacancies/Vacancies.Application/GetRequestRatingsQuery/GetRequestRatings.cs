using System.Text.Json;
using Shared.Abstractions;
using Vacancies.Application.Fails.Exceptions;
using Vacancies.Contracts.Responses;

namespace Vacancies.Application.GetRequestRatingsQuery;

public class GetRequestRatings: IQueryHandler<VacancyRatingsResponse, GetRequestRatingsQuery>
{
    private readonly HttpClient _httpClient;

    public GetRequestRatings(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<VacancyRatingsResponse> Handle(GetRequestRatingsQuery query, CancellationToken cancellationToken)
    {   
        // Get Ratings of all Vacancies
        var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5001/api/ratings");
        request.Headers.Add("User-Agent", "Locator/1.0");
        request.Headers.Add("Api-Gateway", "Signed");

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new GetRatingsFailureException();
        }

        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        var ratingsResponse = JsonSerializer.Deserialize<VacancyRatingsResponse>(json);
        
        return ratingsResponse ?? new VacancyRatingsResponse();
    } 
}