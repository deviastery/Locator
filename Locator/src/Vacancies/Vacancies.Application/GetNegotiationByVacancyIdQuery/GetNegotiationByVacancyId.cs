using CSharpFunctionalExtensions;
using HeadHunter.Contracts;
using Shared;
using Shared.Abstractions;
using Vacancies.Application.Fails.Exceptions;
using Vacancies.Contracts.Dto;
using Vacancies.Contracts.Responses;

namespace Vacancies.Application.GetNegotiationByVacancyIdQuery;

public class GetNegotiationByVacancyId : IQueryHandler<NegotiationResponse, GetNegotiationByVacancyIdQuery>
{
    private readonly IVacanciesContract _vacanciesContract;
    private readonly IAuthContract _authContract;
    
    public GetNegotiationByVacancyId(
        IVacanciesContract vacanciesContract, 
        IAuthContract authContract)
    {
        _vacanciesContract = vacanciesContract;
        _authContract = authContract;
    }  
    public async Task<NegotiationResponse> Handle(
        GetNegotiationByVacancyIdQuery query,
        CancellationToken cancellationToken)
    {
        // Get Employee access token
        string? token = await _authContract.GetEmployeeTokenAsync(query.Dto.UserId, cancellationToken);
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