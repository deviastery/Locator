using CSharpFunctionalExtensions;
using Locator.Application.Abstractions;
using Locator.Application.Users;
using Locator.Application.Vacancies.Fails.Exceptions;
using Locator.Contracts.Vacancies.Dto;
using Locator.Contracts.Vacancies.Responses;
using Shared;

namespace Locator.Application.Vacancies.GetNegotiationByVacancyIdQuery;

public class GetNegotiationByVacancyId : IQueryHandler<NegotiationResponse, GetNegotiationByVacancyIdQuery>
{
    private readonly IVacanciesService _vacanciesService;
    private readonly IAuthService _authService;
    
    public GetNegotiationByVacancyId(
        IVacanciesService vacanciesService, 
        IAuthService authService)
    {
        _vacanciesService = vacanciesService;
        _authService = authService;
    }  
    public async Task<NegotiationResponse> Handle(
        GetNegotiationByVacancyIdQuery query,
        CancellationToken cancellationToken)
    {
        // Get Employee access token
        (_, bool isTokenFailure, string? token) = await _authService
            .GetValidEmployeeAccessTokenAsync(query.Dto.UserId, cancellationToken);
        if (isTokenFailure)
        {
            throw new GetValidEmployeeAccessTokenException();
        }

        // Get Negotiation by Vacancy ID
        Result<NegotiationDto, Error> negotiationResult = await _vacanciesService
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