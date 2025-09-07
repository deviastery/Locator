using FluentValidation;
using Locator.Contracts.Vacancies;
using Locator.Domain.Rating;
using Locator.Domain.Vacancies;
using Microsoft.Extensions.Logging;

namespace Locator.Application.Vacancies;

public class VacanciesService: IVacanciesService
{
    private readonly IVacanciesRepository _vacanciesRepository;
    private readonly ILogger<VacanciesService> _logger;
    private readonly IValidator<AddReviewDto> _validator;

    public VacanciesService(
        IVacanciesRepository vacanciesRepository, 
        IValidator<AddReviewDto> validator,
        ILogger<VacanciesService> logger)
    {
        _vacanciesRepository = vacanciesRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Guid> AddReview(
        Guid vacancyId,
        AddReviewDto reviewDto,
        CancellationToken cancellationToken)
    {
        // Валидация входных данных
        var validationResult = await _validator.ValidateAsync(reviewDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        // Валидация бизнес логики
        int countOfDaysAfterApplying =
            await _vacanciesRepository.GetDaysAfterApplyingAsync(vacancyId, reviewDto.UserId, cancellationToken);
        if (countOfDaysAfterApplying >= 5)
        {
            throw new Exception("Можно оставить отзыв только спустя 5 дней после отклика.");
        }
        
        var review = new Review(reviewDto.Mark, reviewDto?.Comment, reviewDto.UserId, vacancyId);
        await _vacanciesRepository.AddReviewAsync(review, cancellationToken);
        var reviewsVacancyId = await _vacanciesRepository.GetReviewsByVacancyIdAsync(vacancyId, cancellationToken);
        var rating = new VacancyRating(Review.CalculateAverageMark(reviewsVacancyId), vacancyId);
        await _vacanciesRepository.SaveRatingAsync(rating, cancellationToken);
        
        _logger.LogInformation("Review created with id={ReviewId} on vacancy with id={VacancyId}", review.Id, vacancyId);

        // TODO: Отправить notification тем users, у которых есть в откликах вакансия с VacancyId

        return review.Id;
    }
}