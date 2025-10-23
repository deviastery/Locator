using CSharpFunctionalExtensions;
using Locator.Application.Abstractions;
using Locator.Application.Users;
using Locator.Application.Vacancies.Fails.Exceptions;
using Locator.Contracts.Vacancies.Responses;

namespace Locator.Application.Vacancies.GetNegotiationsQuery;

public class GetNegotiations : IQueryHandler<NegotiationsResponse, GetNegotiationsQuery>
{
    private readonly IVacanciesService _vacanciesService;
    private readonly IAuthService _authService;
    
    public GetNegotiations( 
        IVacanciesService vacanciesService, 
        IAuthService authService)
    {
        _vacanciesService = vacanciesService;
        _authService = authService;
    }  

    public async Task<NegotiationsResponse> Handle(
        GetNegotiationsQuery query, 
        CancellationToken cancellationToken)
    {
        (_, bool isFailure, string? token) = await _authService
            .GetValidEmployeeAccessTokenAsync(query.Dto.UserId, cancellationToken);
        if (isFailure)
        {
            throw new GetValidEmployeeAccessTokenException();
        }

        var negotiationsResult = await _vacanciesService
            .GetNegotiationsByUserIdAsync(query.Dto.Query, token, cancellationToken);
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