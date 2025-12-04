using CSharpFunctionalExtensions;
using FluentValidation;
using Framework.Extensions;
using HeadHunter.Contracts;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Abstractions;
using Users.Contracts;
using Users.Contracts.Dto;
using Vacancies.Application.Fails;
using Vacancies.Contracts.Dto;
using Vacancies.Domain;

namespace Vacancies.Application.CreateReviewCommand;

    public class CreateReviewCommandHandler : ICommandHandler<Guid, CreateReviewCommand>
{
    private readonly IVacanciesRepository _vacanciesRepository;
    private readonly IUsersContract _usersContract;
    private readonly ICommandHandler<PrepareToUpdateVacancyRatingCommand.PrepareToUpdateVacancyRatingCommand> _prepareToUpdateVacancyRatingCommandHandler;
    private readonly IVacanciesContract _vacanciesContract;
    private readonly IAuthContract _authContract;
    private readonly IValidator<CreateReviewDto> _validator;
    private readonly ILogger<CreateReviewCommandHandler> _logger;
    
    public CreateReviewCommandHandler(
        IVacanciesRepository vacanciesRepository,
        IUsersContract usersContract,
        ICommandHandler<PrepareToUpdateVacancyRatingCommand.PrepareToUpdateVacancyRatingCommand> prepareToUpdateVacancyRatingCommandHandler,
        IVacanciesContract vacanciesContract, 
        IAuthContract authContract,
        IValidator<CreateReviewDto> validator, 
        ILogger<CreateReviewCommandHandler> logger)
    {
        _vacanciesRepository = vacanciesRepository;
        _usersContract = usersContract;
        _prepareToUpdateVacancyRatingCommandHandler = prepareToUpdateVacancyRatingCommandHandler;
        _vacanciesContract = vacanciesContract;
        _authContract = authContract;
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
        string? token = await _authContract.GetEmployeeTokenAsync(command.UserId, cancellationToken);
        if (token is null)
        {
            return Errors.General.Failure("Error getting Employee access token").ToFailure();
        }

        // Validation of business logic
        // Existing Negotiation
        var negotiationResult = await _vacanciesContract
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
            await _vacanciesContract.GetDaysAfterApplyingAsync(negotiationId, token, cancellationToken);
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

        var getUserDto = new GetUserDto(command.UserId);
        
        // Get User
        var user = await _usersContract.GetUserDtoAsync(getUserDto, cancellationToken);
        if (user is null)
        {
            return Errors.General.NotFound($"User not found be ID={command.UserId}").ToFailure();
        }
        
        // Create Review
        var review = new Review(
            command.ReviewDto.Mark, 
            command.ReviewDto.Comment, 
            command.UserId, 
            user.FirstName ?? string.Empty, 
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