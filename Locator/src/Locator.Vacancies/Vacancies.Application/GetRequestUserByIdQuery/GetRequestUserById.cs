using System.Text.Json;
using Shared.Abstractions;
using Vacancies.Application.Fails.Exceptions;
using Vacancies.Contracts.Responses;

namespace Vacancies.Application.GetRequestUserByIdQuery;

public class GetRequestUserById: IQueryHandler<UserResponse, GetRequestUserByIdQuery>
{
    private readonly HttpClient _httpClient;

    public GetRequestUserById(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UserResponse> Handle(GetRequestUserByIdQuery query, CancellationToken cancellationToken)
    {   
        // Get User
        var request = new HttpRequestMessage(
            HttpMethod.Get, 
            $"https://localhost:5000/api/users/{query.UserId}");
        request.Headers.Add("User-Agent", "Locator/1.0");
        request.Headers.Add("Api-Gateway", "Signed");

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new GetUserByIdFailureException();
        }

        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        var userResponse = JsonSerializer.Deserialize<UserResponse>(json);
        
        return userResponse ?? new UserResponse();
    } 
}