using CSharpFunctionalExtensions;
using Locator.Application.Abstractions;
using Locator.Application.Ratings;
using Shared;

namespace Locator.Application.Vacancies.GetVacanciesWithFilters;

public class GetVacanciesWithFilters : IHandler<VacancyResponse, GetVacanciesWithFiltersCommand>
{
    private readonly IVacanciesRepository _vacanciesRepository;
    private readonly IRatingsRepository _ratingsRepository;
    
    public GetVacanciesWithFilters(
        IVacanciesRepository vacanciesRepository, 
        IRatingsRepository ratingsRepository)
    {
        _vacanciesRepository = vacanciesRepository;
        _ratingsRepository = ratingsRepository;
    }

    public async Task<Result<VacancyResponse, Failure>> Handle(
        GetVacanciesWithFiltersCommand command,
        CancellationToken cancellationToken)
    {
        var vacanciesResult = await _vacanciesRepository.GetVacanciesWithFiltersAsync(command, cancellationToken);
        if (vacanciesResult.IsFailure)
        {
            return vacanciesResult.Error.ToFailure();
        }
        var vacancies = vacanciesResult.Value;

        var vacancyRatingIds = vacancies.Select(v => v.RatingId).ToList();
        (_, bool isFailure, var vacancyRatingsDict, Error? error) = 
            await _ratingsRepository.GetVacancyRatingsByIdsAsync(vacancyRatingIds, cancellationToken);
    
        if (isFailure)
        {
            return error.ToFailure();
        }
        
        var vacanciesDto = vacancies.Select(v => new VacancyDto(
            v.Id,
            v.Name,
            v.Description,
            v.Salary,
            v.Experience,
            vacancyRatingsDict[v.RatingId]
        ));

        return new VacancyResponse(vacanciesDto, 10);
    }
}