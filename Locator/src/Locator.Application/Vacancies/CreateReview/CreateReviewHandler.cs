using CSharpFunctionalExtensions;
using FluentValidation;
using Locator.Application.Abstractions;
using Locator.Application.Extensions;
using Locator.Application.Ratings.UpdateVacancyRating;
using Locator.Application.Vacancies.Fails;
using Locator.Application.Vacancies.PrepareToUpdateVacancyRating;
using Locator.Contracts.Vacancies;
using Locator.Domain.Vacancies;
using Microsoft.Extensions.Logging;
using Shared;

namespace Locator.Application.Vacancies.CreateReview;

public class CreateReviewHandler : IHandler<Guid, CreateReviewCommand>
{
    private readonly IVacanciesRepository _vacanciesRepository;
    private readonly IHandler<PrepareToUpdateVacancyRatingCommand> _prepareToUpdateVacancyRatingHandler;
    private readonly IValidator<CreateReviewDto> _validator;
    private readonly ILogger<CreateReviewHandler> _logger;
    
    public CreateReviewHandler(
        IVacanciesRepository vacanciesRepository,
        IHandler<PrepareToUpdateVacancyRatingCommand> prepareToUpdateVacancyRatingHandler,
        IValidator<CreateReviewDto> validator, 
        ILogger<CreateReviewHandler> logger)
    {
        _vacanciesRepository = vacanciesRepository;
        _prepareToUpdateVacancyRatingHandler = prepareToUpdateVacancyRatingHandler;
        _validator = validator;
        _logger = logger;
    }
    private static Result<bool, Failure> IsVacancyReadyForReview(int daysAfterApplying)
    {
        const int minDaysToReview = 5;
        if (daysAfterApplying <= minDaysToReview)
        {
            return Errors.NotReadyForReview().ToFailure();
        }
        return true;
    }
    public async Task<Result<Guid, Failure>> Handle(
        CreateReviewCommand command,
        CancellationToken cancellationToken)
    {
        // Input data validation
        var validationResult = await _validator.ValidateAsync(command.reviewDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors().ToFailure();
        }
        
        // Validation of business logic
        // Existing vacancy
        var vacancyResult = await _vacanciesRepository.GetVacancyByIdAsync(command.vacancyId, cancellationToken);
        if (vacancyResult.IsFailure)
        {
            return vacancyResult.Error.ToFailure();
        }
        
        // Possible to leave a review
        int daysAfterApplying =
            await _vacanciesRepository.GetDaysAfterApplyingAsync(command.vacancyId, command.reviewDto.UserName, cancellationToken);
        var isReadyForReviewResult = IsVacancyReadyForReview(daysAfterApplying);
        if (isReadyForReviewResult.IsFailure)
        {
            return isReadyForReviewResult.Error.ToFailure();
        }
        
        // Create Review
        var review = new Review(command.reviewDto.Mark, command.reviewDto?.Comment, command.reviewDto.UserName, command.vacancyId);
        var reviewId = await _vacanciesRepository.CreateReviewAsync(review, cancellationToken);
        _logger.LogInformation("Review created with id={ReviewId} on vacancy with id={VacancyId}", reviewId, command.vacancyId);
        
        // Updating after create review
        var updateVacancyRatingCommand = new PrepareToUpdateVacancyRatingCommand(command.vacancyId);
        var updateRatingResult = await _prepareToUpdateVacancyRatingHandler
            .Handle(updateVacancyRatingCommand, cancellationToken);
        if (updateRatingResult.IsFailure)
        {
            return updateRatingResult.Error.ToFailure();
        }
        
        return review.Id;
    }
}