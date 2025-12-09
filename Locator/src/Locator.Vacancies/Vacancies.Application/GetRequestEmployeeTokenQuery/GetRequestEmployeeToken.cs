using System.Text.Json;
using Shared.Abstractions;
using Vacancies.Application.Fails.Exceptions;
using Vacancies.Contracts.Responses;

namespace Vacancies.Application.GetRequestEmployeeTokenQuery;

public class GetRequestEmployeeToken: IQueryHandler<EmployeeTokenResponse, GetRequestEmployeeTokenQuery>
{
    private readonly HttpClient _httpClient;

    public GetRequestEmployeeToken(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<EmployeeTokenResponse> Handle(GetRequestEmployeeTokenQuery query, CancellationToken cancellationToken)
    {   
        // Get Employee access token
        var tokenRequest = new HttpRequestMessage(
            HttpMethod.Get, 
            $"https://localhost:5000/api/users/auth/employee_token/{query.UserId}");
        tokenRequest.Headers.Add("User-Agent", "Locator/1.0");
        tokenRequest.Headers.Add("Api-Gateway", "Signed");

        var tokenResponse = await _httpClient.SendAsync(tokenRequest, cancellationToken);
        if (!tokenResponse.IsSuccessStatusCode)
        {
            throw new GetValidEmployeeAccessTokenException();
        }

        string tokenJson = await tokenResponse.Content.ReadAsStringAsync(cancellationToken);
        var employeeTokenResponse = JsonSerializer.Deserialize<EmployeeTokenResponse>(tokenJson);
        
        return employeeTokenResponse ?? new EmployeeTokenResponse();
    } 
}