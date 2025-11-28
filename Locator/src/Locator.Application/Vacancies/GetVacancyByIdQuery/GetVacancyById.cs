using CSharpFunctionalExtensions;
using Locator.Application.Abstractions;
using Locator.Application.Ratings;
using Locator.Application.Users;
using Locator.Application.Vacancies.Fails.Exceptions;
using Locator.Contracts.Vacancies.Dto;
using Locator.Contracts.Vacancies.Responses;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Locator.Application.Vacancies.GetVacancyByIdQuery;

public class GetVacancyById : IQueryHandler<VacancyResponse, GetVacancyByIdQuery>
{
    private readonly IRatingsReadDbContext _ratingsDbContext;
    private readonly IVacanciesReadDbContext _vacanciesDbContext;
    private readonly IVacanciesService _vacanciesService;
    private readonly IAuthService _authService;
    
    public GetVacancyById(
        IRatingsReadDbContext ratingsDbContext, 
        IVacanciesReadDbContext vacanciesDbContext,
        IVacanciesService vacanciesService, 
        IAuthService authService)
    {
        _ratingsDbContext = ratingsDbContext;
        _vacanciesDbContext = vacanciesDbContext;
        _vacanciesService = vacanciesService;
        _authService = authService;
    }  
    public async Task<VacancyResponse> Handle(
        GetVacancyByIdQuery query,
        CancellationToken cancellationToken)
    {
        // Get Employee access token
        string? token = await _authService.GetEmployeeTokenAsync(query.Dto.UserId, cancellationToken);
        if (token is null)
        {
            throw new GetValidEmployeeAccessTokenException();
        }

        // Get a Vacancy by ID
        Result<VacancyDto, Error> vacancyByIdResult = await _vacanciesService
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
        var rating = await _ratingsDbContext.ReadVacancyRatings
            .Where(r => r.EntityId == query.Dto.VacancyId)
            .FirstOrDefaultAsync(cancellationToken);
        
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