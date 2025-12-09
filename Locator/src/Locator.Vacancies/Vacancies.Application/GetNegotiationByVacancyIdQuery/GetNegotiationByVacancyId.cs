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
    private readonly IQueryHandler<EmployeeTokenResponse, GetRequestEmployeeTokenQuery.GetRequestEmployeeTokenQuery> 
        _getRequestEmployeeTokenQuery;
    private readonly IVacanciesContract _vacanciesContract;
    
    public GetNegotiationByVacancyId(
        IQueryHandler<EmployeeTokenResponse, GetRequestEmployeeTokenQuery.GetRequestEmployeeTokenQuery> getRequestEmployeeTokenQuery,
        IVacanciesContract vacanciesContract)
    {
        _getRequestEmployeeTokenQuery = getRequestEmployeeTokenQuery;
        _vacanciesContract = vacanciesContract;
    }  
    public async Task<NegotiationResponse> Handle(
        GetNegotiationByVacancyIdQuery query,
        CancellationToken cancellationToken)
    {
        // Get Employee access token
        var getRequestEmployeeTokenQuery = new GetRequestEmployeeTokenQuery.GetRequestEmployeeTokenQuery(
            query.Dto.UserId);
        var employeeTokenResponse = await _getRequestEmployeeTokenQuery.Handle(
            getRequestEmployeeTokenQuery,
            cancellationToken);
        if (employeeTokenResponse?.EmployeeToken?.Token == null)
        {
            throw new GetValidEmployeeAccessTokenException();
        }
        
        string token = employeeTokenResponse.EmployeeToken.Token;
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