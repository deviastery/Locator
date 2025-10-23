using CSharpFunctionalExtensions;
using Locator.Application.Abstractions;
using Locator.Application.Users;
using Locator.Application.Vacancies.Fails.Exceptions;
using Locator.Contracts.Vacancies.Dtos;
using Locator.Contracts.Vacancies.Responses;

namespace Locator.Application.Vacancies.GetNegotiationByVacancyIdQuery;

public class GetNegotiationByVacancyId : IQueryHandler<NegotiationByVacancyIdResponse, GetNegotiationByVacancyIdQuery>
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
    public async Task<NegotiationByVacancyIdResponse> Handle(
        GetNegotiationByVacancyIdQuery query,
        CancellationToken cancellationToken)
    {
        (_, bool isTokenFailure, string? token) = await _authService
            .GetValidEmployeeAccessTokenAsync(query.Dto.UserId, cancellationToken);
        if (isTokenFailure)
        {
            throw new GetValidEmployeeAccessTokenException();
        }

        (_, bool isFailure, NegotiationDto? negotiation) = await _vacanciesService
            .GetNegotiationByVacancyIdAsync(query.Dto.VacancyId, token, cancellationToken);
        if (isFailure || negotiation is null)
        {
            throw new GetNegotiationsFailureException();
        }

        return new NegotiationByVacancyIdResponse(negotiation);
    }
}