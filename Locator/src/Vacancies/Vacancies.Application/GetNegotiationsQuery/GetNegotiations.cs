using HeadHunter.Contracts;
using Shared.Abstractions;
using Vacancies.Application.Fails.Exceptions;
using Vacancies.Contracts.Responses;

namespace Vacancies.Application.GetNegotiationsQuery;

public class GetNegotiations : IQueryHandler<NegotiationsResponse, GetNegotiationsQuery>
{
    private readonly IVacanciesContract _vacanciesContract;
    private readonly IAuthContract _authContract;
    
    public GetNegotiations( 
        IVacanciesContract vacanciesContract, 
        IAuthContract authContract)
    {
        _vacanciesContract = vacanciesContract;
        _authContract = authContract;
    }  

    public async Task<NegotiationsResponse> Handle(
        GetNegotiationsQuery query, 
        CancellationToken cancellationToken)
    {
        // Get Employee access token
        string? token = await _authContract.GetEmployeeTokenAsync(query.Dto.UserId, cancellationToken);
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