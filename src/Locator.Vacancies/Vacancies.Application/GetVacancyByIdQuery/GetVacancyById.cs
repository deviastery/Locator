using CSharpFunctionalExtensions;
using HeadHunter.Contracts;
using HeadHunter.Contracts.Dto;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Abstractions;
using Vacancies.Application.Fails.Exceptions;
using Vacancies.Contracts.Dto;
using Vacancies.Contracts.Responses;

namespace Vacancies.Application.GetVacancyByIdQuery;

public class GetVacancyById : IQueryHandler<VacancyResponse, GetVacancyByIdQuery>
{
    private readonly IQueryHandler<EmployeeTokenResponse, GetRequestEmployeeTokenQuery.GetRequestEmployeeTokenQuery> 
        _getRequestEmployeeTokenQuery;
    private readonly IQueryHandler<RatingByVacancyIdResponse, GetRequestRatingByVacancyIdQuery.GetRequestRatingByVacancyIdQuery> 
        _getRequestRatingByVacancyIdQuery;
    private readonly IVacanciesReadDbContext _vacanciesDbContext;
    private readonly IVacanciesContract _vacanciesContract;
    
    public GetVacancyById(
        IQueryHandler<EmployeeTokenResponse, GetRequestEmployeeTokenQuery.GetRequestEmployeeTokenQuery> getRequestEmployeeTokenQuery,
        IQueryHandler<RatingByVacancyIdResponse, GetRequestRatingByVacancyIdQuery.GetRequestRatingByVacancyIdQuery> 
            getRequestRatingByVacancyIdQuery,
        IVacanciesContract vacanciesContract, 
        IVacanciesReadDbContext vacanciesDbContext)
    {
        _getRequestEmployeeTokenQuery = getRequestEmployeeTokenQuery;
        _getRequestRatingByVacancyIdQuery = getRequestRatingByVacancyIdQuery;
        _vacanciesDbContext = vacanciesDbContext;
        _vacanciesContract = vacanciesContract;
    }  
    public async Task<VacancyResponse> Handle(
        GetVacancyByIdQuery query,
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

        // Get a Vacancy by ID
        Result<VacancyDto, Error> vacancyByIdResult = await _vacanciesContract
            .GetVacancyByIdAsync(query.Dto.VacancyId.ToString(), token, cancellationToken);
        if (vacancyByIdResult.IsFailure && vacancyByIdResult.Error.Code == "record.not.found")
        {
            throw new GetVacancyByIdNotFoundException($"Vacancy not found by ID={query.Dto.VacancyId}");
        }        
        if (vacancyByIdResult.IsFailure || vacancyByIdResult.Value == null)
        {
            throw new GetVacancyByIdFailureException();
        }

        // Get a Rating of a Vacancy
        var ratingResponse = await _getRequestRatingByVacancyIdQuery.Handle(
            new GetRequestRatingByVacancyIdQuery.GetRequestRatingByVacancyIdQuery(query.Dto.VacancyId),
            cancellationToken);
        
        var rating = ratingResponse.Rating;
        
        // Get Reviews of a Vacancy
        var reviews = await _vacanciesDbContext.ReadReviews
            .Where(r => r.VacancyId == query.Dto.VacancyId)
            .ToListAsync(cancellationToken);

        var reviewsDto = reviews.Select(r => new ReviewDto(
            r.Id,
            r.Mark,
            r.Comment,
            r.UserName));

        // Get Vacancy with Reviews and Rating
        var vacancyDto = new FullVacancyDto(query.Dto.VacancyId, vacancyByIdResult.Value, rating?.Value);

        return new VacancyResponse(vacancyDto, reviewsDto);
    }
}