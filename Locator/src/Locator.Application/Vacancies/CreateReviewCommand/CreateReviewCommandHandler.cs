using CSharpFunctionalExtensions;
using FluentValidation;
using Locator.Application.Abstractions;
using Locator.Application.Extensions;
using Locator.Application.Users;
using Locator.Application.Vacancies.Fails;
using Locator.Contracts.Vacancies.Dto;
using Locator.Domain.Users;
using Locator.Domain.Vacancies;
using Microsoft.Extensions.Logging;
using Shared;

namespace Locator.Application.Vacancies.CreateReviewCommand;

    public class CreateReviewCommandHandler : ICommandHandler<Guid, CreateReviewCommand>
{
    private readonly IVacanciesRepository _vacanciesRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly ICommandHandler<PrepareToUpdateVacancyRatingCommand.PrepareToUpdateVacancyRatingCommand> _prepareToUpdateVacancyRatingCommandHandler;
    private readonly IVacanciesService _vacanciesService;
    private readonly IAuthService _authService;
    private readonly IValidator<CreateReviewDto> _validator;
    private readonly ILogger<CreateReviewCommandHandler> _logger;
    
    public CreateReviewCommandHandler(
        IVacanciesRepository vacanciesRepository,
        IUsersRepository usersRepository,
        ICommandHandler<PrepareToUpdateVacancyRatingCommand.PrepareToUpdateVacancyRatingCommand> prepareToUpdateVacancyRatingCommandHandler,
        IVacanciesService vacanciesService, 
        IAuthService authService,
        IValidator<CreateReviewDto> validator, 
        ILogger<CreateReviewCommandHandler> logger)
    {
        _vacanciesRepository = vacanciesRepository;
        _usersRepository = usersRepository;
        _prepareToUpdateVacancyRatingCommandHandler = prepareToUpdateVacancyRatingCommandHandler;
        _vacanciesService = vacanciesService;
        _authService = authService;
        _usersRepository = usersRepository;
        _validator = validator;
        _logger = logger;
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
        
        // Get Employee access token
        string? token = await _authService.GetEmployeeTokenAsync(command.UserId, cancellationToken);
        if (token is null)
        {
            return Errors.General.Failure("Error getting Employee access token").ToFailure();
        }

        // Validation of business logic
        // Existing Negotiation
        var negotiationResult = await _vacanciesService
            .GetNegotiationByVacancyIdAsync(command.VacancyId, token, cancellationToken);
        if (negotiationResult.IsFailure)
        {
            return negotiationResult.Error.ToFailure();
        }
        if (!long.TryParse(negotiationResult.Value.Id, out long negotiationId))
        {
            return Errors.TryParseNegotiationIdFail().ToFailure();
        }
        
        // Has user reviewed vacancy?
        bool hasUserReviewedVacancy = await _vacanciesRepository.HasUserReviewedVacancyAsync(
            command.UserId, command.VacancyId, cancellationToken);
        if (hasUserReviewedVacancy)
        {
            return Errors.UserAlreadyReviewedVacancy().ToFailure();
        }
        
        // Have enough days passed since the applying?
        var daysAfterApplyingResult =
            await _vacanciesService.GetDaysAfterApplyingAsync(negotiationId, token, cancellationToken);
        if (daysAfterApplyingResult.IsFailure)
        {
            return daysAfterApplyingResult.Error.ToFailure();
        }
        int daysAfterApplying = daysAfterApplyingResult.Value;
        
        var isReadyForReviewResult = IsVacancyReadyForReview(daysAfterApplying);
        if (isReadyForReviewResult.IsFailure)
        {
            return isReadyForReviewResult.Error.ToFailure();
        }
        
        // Get User
        var userResult = await _usersRepository.GetUserAsync(command.UserId, cancellationToken);
        if (userResult.IsFailure)
        {
            return userResult.Error.ToFailure();
        }
        
        // Create Review
        var review = new Review(
            command.ReviewDto.Mark, 
            command.ReviewDto.Comment, 
            command.UserId, 
            userResult.Value.Name, 
            command.VacancyId);
        var reviewId = await _vacanciesRepository.CreateReviewAsync(review, cancellationToken);
        _logger.LogInformation("Review created with id={ReviewId} on vacancy with id={VacancyId}", reviewId, command.VacancyId);
        
        // Update Rating after create Review
        var updateVacancyRatingCommand = new PrepareToUpdateVacancyRatingCommand.PrepareToUpdateVacancyRatingCommand(command.VacancyId);
        var updateRatingResult = await _prepareToUpdateVacancyRatingCommandHandler
            .Handle(updateVacancyRatingCommand, cancellationToken);
        if (updateRatingResult.IsFailure)
        {
            return updateRatingResult.Error.ToFailure();
        }
        
        return review.Id;
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
}