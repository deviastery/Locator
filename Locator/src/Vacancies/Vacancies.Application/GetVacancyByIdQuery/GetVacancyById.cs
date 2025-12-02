
using CSharpFunctionalExtensions;
using HeadHunter.Contracts;
using Microsoft.EntityFrameworkCore;
using Ratings.Contracts;
using Shared;
using Shared.Abstractions;
using Vacancies.Application.Fails.Exceptions;
using Vacancies.Contracts.Dto;
using Vacancies.Contracts.Responses;

namespace Vacancies.Application.GetVacancyByIdQuery;

public class GetVacancyById : IQueryHandler<VacancyResponse, GetVacancyByIdQuery>
{
    private readonly IRatingsContract _ratingsContract;
    private readonly IVacanciesReadDbContext _vacanciesDbContext;
    private readonly IVacanciesContract _vacanciesContract;
    private readonly IAuthContract _authContract;
    
    public GetVacancyById(
        IRatingsContract ratingsContract, 
        IVacanciesReadDbContext vacanciesDbContext,
        IVacanciesContract vacanciesContract, 
        IAuthContract authContract)
    {
        _ratingsContract = ratingsContract;
        _vacanciesDbContext = vacanciesDbContext;
        _vacanciesContract = vacanciesContract;
        _authContract = authContract;
    }  
    public async Task<VacancyResponse> Handle(
        GetVacancyByIdQuery query,
        CancellationToken cancellationToken)
    {
        // Get Employee access token
        string? token = await _authContract.GetEmployeeTokenAsync(query.Dto.UserId, cancellationToken);
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
        var ratingResult = await _ratingsContract.GetRatingDtoByVacancyIdAsync(
                query.Dto.VacancyId, 
                cancellationToken);
        if (ratingResult.IsFailure)
        {
            throw new GetRatingByVacancyIdNotFoundException(
                $"Rating not found by Vacancy ID={query.Dto.VacancyId}");
        }

        var rating = ratingResult.Value;
        
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