using CSharpFunctionalExtensions;
using Locator.Application.Abstractions;
using Locator.Application.Ratings;
using Locator.Application.Users;
using Locator.Application.Vacancies.Fails.Exceptions;
using Locator.Contracts.Vacancies.Dtos;
using Locator.Contracts.Vacancies.Responses;
using Microsoft.EntityFrameworkCore;

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
        (_, bool isTokenFailure, string? token) = await _authService
            .GetValidEmployeeAccessTokenAsync(query.Dto.UserId, cancellationToken);
        if (isTokenFailure)
        {
            throw new GetValidEmployeeAccessTokenException();
        }

        (_, bool isFailure, VacancyDto? vacancy) = await _vacanciesService
            .GetVacancyByIdAsync(query.Dto.VacancyId.ToString(), token, cancellationToken);
        if (isFailure || vacancy == null)
        {
            throw new GetVacancyByIdException();
        }

        var rating = await _ratingsDbContext.ReadVacancyRatings
            .Where(r => r.EntityId == query.Dto.VacancyId)
            .FirstOrDefaultAsync(cancellationToken);
        
        var reviews = await _vacanciesDbContext.ReadReviews
            .Where(r => r.VacancyId == query.Dto.VacancyId)
            .ToListAsync(cancellationToken);

        var reviewsDto = reviews.Select(r => new ReviewDto(
            r.Id,
            r.Mark,
            r.Comment,
            r.UserName
        ));

        var vacancyDto = new FullVacancyDto(query.Dto.VacancyId, vacancy, rating?.Value);

        return new VacancyResponse(vacancyDto, reviewsDto);
    }
}