using System.Text.Json;
using CSharpFunctionalExtensions;
using HeadHunter.Contracts;
using HeadHunter.Contracts.Dto;
using Shared;
using Shared.Abstractions;
using Vacancies.Application.Fails.Exceptions;
using Vacancies.Contracts.Responses;

namespace Vacancies.Application.GetNegotiationByVacancyIdQuery;

public class GetNegotiationByVacancyId : IQueryHandler<NegotiationResponse, GetNegotiationByVacancyIdQuery>
{
    private readonly HttpClient _httpClient;
    private readonly IVacanciesContract _vacanciesContract;
    
    public GetNegotiationByVacancyId(
        HttpClient httpClient,
        IVacanciesContract vacanciesContract)
    {
        _vacanciesContract = vacanciesContract;
        _httpClient = httpClient;
    }  
    public async Task<NegotiationResponse> Handle(
        GetNegotiationByVacancyIdQuery query,
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

        // Get Negotiation by Vacancy ID
        Result<NegotiationDto, Error> negotiationResult = await _vacanciesContract
            .GetNegotiationByVacancyIdAsync(query.Dto.VacancyId, token, cancellationToken);
        
        switch (negotiationResult.IsFailure)
        {
            case true when negotiationResult.Error.Code == "value.is.invalid":
                throw new GetNegotiationsValidationException();
            case true when negotiationResult.Error.Code == "record.not.found":
                throw new GetNegotiationsNotFoundException(
                    $"Negotiations not found by vacancy ID={query.Dto.VacancyId}");
        }
        if (negotiationResult.IsFailure || negotiationResult.Value is null)
        {
            throw new GetNegotiationsFailureException();
        }

        return new NegotiationResponse(negotiationResult.Value);
    }
}