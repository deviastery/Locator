using CSharpFunctionalExtensions;
using FluentValidation;
using Locator.Application.Abstractions;
using Locator.Application.Extensions;
using Locator.Application.Users;
using Locator.Application.Vacancies.Fails;
using Locator.Contracts.Vacancies;
using Locator.Domain.Vacancies;
using Microsoft.Extensions.Logging;
using Shared;

namespace Locator.Application.Vacancies.CreateReviewCommand;

public class CreateReviewCommandHandler : ICommandHandler<Guid, CreateReviewCommand>
{
    private readonly IVacanciesRepository _vacanciesRepository;
    private readonly ICommandHandler<PrepareToUpdateVacancyRatingCommand.PrepareToUpdateVacancyRatingCommand> _prepareToUpdateVacancyRatingCommandHandler;
    private readonly IEmployeeVacanciesService _vacanciesService;
    private readonly IAuthService _authService;
    private readonly IValidator<CreateReviewDto> _validator;
    private readonly ILogger<CreateReviewCommandHandler> _logger;
    
    public CreateReviewCommandHandler(
        IVacanciesRepository vacanciesRepository,
        ICommandHandler<PrepareToUpdateVacancyRatingCommand.PrepareToUpdateVacancyRatingCommand> prepareToUpdateVacancyRatingCommandHandler,
        IEmployeeVacanciesService vacanciesService, 
        IAuthService authService,
        IValidator<CreateReviewDto> validator, 
        ILogger<CreateReviewCommandHandler> logger)
    {
        _vacanciesRepository = vacanciesRepository;
        _prepareToUpdateVacancyRatingCommandHandler = prepareToUpdateVacancyRatingCommandHandler;
        _vacanciesService = vacanciesService;
        _authService = authService;
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
        var validationResult = await _validator.ValidateAsync(command.ReviewDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors().ToFailure();
        }
        
        // Get Token
        var tokenResult = await _authService
            .GetValidEmployeeAccessTokenAsync(command.UserId, cancellationToken);
        if (tokenResult.IsFailure)
        {
            return tokenResult.Error.ToFailure();
        }
        var token = tokenResult.Value;
        
        // Validation of business logic
        // Existing vacancy
        var vacancyResult = await _vacanciesService
            .GetVacancyByIdAsync(command.VacancyId.ToString(), token, cancellationToken);
        if (vacancyResult.IsFailure)
        {
            return vacancyResult.Error.ToFailure();
        }
        
        // Possible to leave a review
        int daysAfterApplying =
            await _vacanciesRepository.GetDaysAfterApplyingAsync(command.VacancyId, command.ReviewDto.UserName, cancellationToken);
        var isReadyForReviewResult = IsVacancyReadyForReview(daysAfterApplying);
        if (isReadyForReviewResult.IsFailure)
        {
            return isReadyForReviewResult.Error.ToFailure();
        }
        
        // Create Review
        var review = new Review(command.ReviewDto.Mark, command.ReviewDto?.Comment, command.ReviewDto.UserName, command.VacancyId);
        var reviewId = await _vacanciesRepository.CreateReviewAsync(review, cancellationToken);
        _logger.LogInformation("Review created with id={ReviewId} on vacancy with id={VacancyId}", reviewId, command.VacancyId);
        
        // Updating after create review
        var updateVacancyRatingCommand = new PrepareToUpdateVacancyRatingCommand.PrepareToUpdateVacancyRatingCommand(command.VacancyId);
        var updateRatingResult = await _prepareToUpdateVacancyRatingCommandHandler
            .Handle(updateVacancyRatingCommand, cancellationToken);
        if (updateRatingResult.IsFailure)
        {
            return updateRatingResult.Error.ToFailure();
        }
        
        return review.Id;
    }
}