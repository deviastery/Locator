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
    private readonly IValidator<CreateReviewDto> _validator;

    public VacanciesService(
        IVacanciesRepository vacanciesRepository, 
        IValidator<CreateReviewDto> validator,
        ILogger<VacanciesService> logger)
    {
        _vacanciesRepository = vacanciesRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Guid> CreateReview(
        Guid vacancyId,
        CreateReviewDto reviewDto,
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
        await _vacanciesRepository.CreateReviewAsync(review, cancellationToken);
        var reviewsVacancyId = await _vacanciesRepository.GetReviewsByVacancyIdAsync(vacancyId, cancellationToken);
        
        // TODO: Отправить запрос на создание рейтинга (Сервис Rating)
        
        _logger.LogInformation("Review created with id={ReviewId} on vacancy with id={VacancyId}", review.Id, vacancyId);

        // TODO: Отправить notification тем users, у которых есть в откликах вакансия с VacancyId (Сервис Notifications)

        return review.Id;
    }
}