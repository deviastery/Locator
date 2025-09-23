using CSharpFunctionalExtensions;
using FluentValidation;
using Locator.Application.Extensions;
using Locator.Application.Ratings;
using Locator.Contracts.Ratings;
using Locator.Contracts.Vacancies;
using Locator.Domain.Vacancies;
using Microsoft.Extensions.Logging;
using Shared;

namespace Locator.Application.Vacancies;

public class VacanciesService: IVacanciesService
{
    private readonly IVacanciesRepository _vacanciesRepository;
    private readonly IRatingsService _ratingsService;
    private readonly ILogger<VacanciesService> _logger;
    private readonly IValidator<CreateReviewDto> _validator;

    public VacanciesService(
        IVacanciesRepository vacanciesRepository, 
        IRatingsService ratingsService, 
        IValidator<CreateReviewDto> validator,
        ILogger<VacanciesService> logger)
    {
        _vacanciesRepository = vacanciesRepository;
        _ratingsService = ratingsService;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, Failure>> CreateReview(
        Guid vacancyId,
        CreateReviewDto reviewDto,
        CancellationToken cancellationToken)
    {
        // Валидация входных данных
        var validationResult = await _validator.ValidateAsync(reviewDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors().ToFailure();
        }
        
        // Валидация бизнес логики
        int countOfDaysAfterApplying =
            await _vacanciesRepository.GetDaysAfterApplyingAsync(vacancyId, reviewDto.UserName, cancellationToken);
        
        var isVacancyReadyForReviewResult = _vacanciesRepository.IsVacancyReadyForReview(countOfDaysAfterApplying);
        if (isVacancyReadyForReviewResult.IsFailure)
        {
            return isVacancyReadyForReviewResult.Error.ToFailure();
        }
        
        var review = new Review(reviewDto.Mark, reviewDto?.Comment, reviewDto.UserName, vacancyId);
        await _vacanciesRepository.CreateReviewAsync(review, cancellationToken);
        var reviewsVacancyId = await _vacanciesRepository.GetReviewsByVacancyIdAsync(vacancyId, cancellationToken);

        var averageMarkResult = Review.CalculateAverageMark(reviewsVacancyId);
        if (averageMarkResult.IsFailure)
        {
            return averageMarkResult.Error.ToFailure();
        }

        var vacancyRatingDto = new CreateVacancyRatingDto(vacancyId, averageMarkResult.Value);
        
        var createVacancyRatingResult = await _ratingsService.CreateVacancyRating(vacancyRatingDto, cancellationToken);
        if (createVacancyRatingResult.IsFailure)
        {
            return createVacancyRatingResult.Error;
        }
        
        _logger.LogInformation("Review created with id={ReviewId} on vacancy with id={VacancyId}", review.Id, vacancyId);

        // TODO: Отправить notification тем users, у которых есть в откликах вакансия с VacancyId (Сервис Notifications)

        return review.Id;
    }
}