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
        // Get Employee access token
        string? token = await _authService.GetEmployeeTokenAsync(query.Dto.UserId, cancellationToken);
        if (token is null)
        {
            throw new GetValidEmployeeAccessTokenException();
        }

        // Get all Negotiations by user ID
        var negotiationsResult = await _vacanciesService
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