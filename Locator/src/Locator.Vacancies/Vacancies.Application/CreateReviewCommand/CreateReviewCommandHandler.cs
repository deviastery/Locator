using CSharpFunctionalExtensions;
using FluentValidation;
using Framework.Extensions;
using HeadHunter.Contracts;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Abstractions;
using Vacancies.Application.Fails;
using Vacancies.Application.Fails.Exceptions;
using Vacancies.Contracts.Dto;
using Vacancies.Contracts.Responses;
using Vacancies.Domain;

namespace Vacancies.Application.CreateReviewCommand;

    public class CreateReviewCommandHandler : ICommandHandler<Guid, CreateReviewCommand>
{
    private readonly IVacanciesRepository _vacanciesRepository;
    private readonly ICommandHandler<PrepareToUpdateVacancyRatingCommand.PrepareToUpdateVacancyRatingCommand> 
        _prepareToUpdateVacancyRatingCommandHandler;
    private readonly IQueryHandler<EmployeeTokenResponse, GetRequestEmployeeTokenQuery.GetRequestEmployeeTokenQuery> 
        _getRequestEmployeeTokenQuery;
    private readonly IQueryHandler<UserResponse, GetRequestUserByIdQuery.GetRequestUserByIdQuery> 
        _getRequestUserByIdQuery;
    private readonly IVacanciesContract _vacanciesContract;
    private readonly IValidator<CreateReviewDto> _validator;
    private readonly ILogger<CreateReviewCommandHandler> _logger;
    
    public CreateReviewCommandHandler(
        IVacanciesRepository vacanciesRepository,
        ICommandHandler<PrepareToUpdateVacancyRatingCommand.PrepareToUpdateVacancyRatingCommand> prepareToUpdateVacancyRatingCommandHandler, 
        IQueryHandler<EmployeeTokenResponse, GetRequestEmployeeTokenQuery.GetRequestEmployeeTokenQuery> getRequestEmployeeTokenQuery,
        IQueryHandler<UserResponse, GetRequestUserByIdQuery.GetRequestUserByIdQuery> getRequestUserByIdQuery,
        IVacanciesContract vacanciesContract, 
        IValidator<CreateReviewDto> validator, 
        ILogger<CreateReviewCommandHandler> logger)
    {
        _vacanciesRepository = vacanciesRepository;
        _prepareToUpdateVacancyRatingCommandHandler = prepareToUpdateVacancyRatingCommandHandler;
        _getRequestEmployeeTokenQuery = getRequestEmployeeTokenQuery;
        _getRequestUserByIdQuery = getRequestUserByIdQuery;
        _vacanciesContract = vacanciesContract;
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
        var getRequestEmployeeTokenQuery = new GetRequestEmployeeTokenQuery.GetRequestEmployeeTokenQuery(
            command.UserId);
        var employeeTokenResponse = await _getRequestEmployeeTokenQuery.Handle(
            getRequestEmployeeTokenQuery,
            cancellationToken);
        if (employeeTokenResponse?.EmployeeToken?.Token == null)
        {
            throw new GetValidEmployeeAccessTokenException();
        }
        
        string token = employeeTokenResponse.EmployeeToken.Token;

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
        
        // Get User
        var getRequestUserByIdQuery = new GetRequestUserByIdQuery.GetRequestUserByIdQuery(
            command.UserId);
        var userResponse = await _getRequestUserByIdQuery.Handle(
            getRequestUserByIdQuery,
            cancellationToken);
        
        if (userResponse.User == null)
        {
            return Errors.GetUserByIdFail().ToFailure();
        }
        var user = userResponse.User;
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