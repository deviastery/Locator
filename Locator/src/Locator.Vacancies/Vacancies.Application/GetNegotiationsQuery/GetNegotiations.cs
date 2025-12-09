using System.Text.Json;
using HeadHunter.Contracts;
using Shared.Abstractions;
using Vacancies.Application.Fails.Exceptions;
using Vacancies.Contracts.Responses;

namespace Vacancies.Application.GetNegotiationsQuery;

public class GetNegotiations : IQueryHandler<NegotiationsResponse, GetNegotiationsQuery>
{
    private readonly HttpClient _httpClient;
    private readonly IVacanciesContract _vacanciesContract;
    
    public GetNegotiations( 
        HttpClient httpClient,
        IVacanciesContract vacanciesContract)
    {
        _vacanciesContract = vacanciesContract;
        _httpClient = httpClient;
    }  

    public async Task<NegotiationsResponse> Handle(
        GetNegotiationsQuery query, 
        CancellationToken cancellationToken)
    {
        // Get Employee access token
        var tokenRequest = new HttpRequestMessage(
            HttpMethod.Get, 
            $"https://localhost:5000/api/users/auth/employee_token/{query.Dto.UserId}");
        tokenRequest.Headers.Add("User-Agent", "Locator/1.0");
        tokenRequest.Headers.Add("Api-Gateway", "Signed");

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

        // Get all Negotiations by user ID
        var negotiationsResult = await _vacanciesContract
            .GetNegotiationsByUserIdAsync(query.Dto.Query, token, cancellationToken);
        
        switch (negotiationsResult.IsFailure)
        {
            case true when negotiationsResult.Error.Code == "value.is.invalid":
                throw new GetNegotiationsValidationException();
            case true when negotiationsResult.Error.Code == "record.not.found":
                throw new GetNegotiationsNotFoundException("Negotiations not found");
        }
        if (negotiationsResult.IsFailure || negotiationsResult.Value.Negotiations is null)
        {
            throw new GetNegotiationsFailureException();
        }
        
        var negotiations = negotiationsResult.Value.Negotiations;
        long count = negotiationsResult.Value.Count;
        int page = negotiationsResult.Value.Page;
        int pages = negotiationsResult.Value.Pages;
        int perPage = negotiationsResult.Value.PerPage;
        
        return new NegotiationsResponse(count, negotiations, page, pages, perPage);
    }
}