using System.Text.Json;
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
    private readonly HttpClient _httpClient;
    private readonly IVacanciesRepository _vacanciesRepository;
    private readonly ICommandHandler<PrepareToUpdateVacancyRatingCommand.PrepareToUpdateVacancyRatingCommand> _prepareToUpdateVacancyRatingCommandHandler;
    private readonly IVacanciesContract _vacanciesContract;
    private readonly IValidator<CreateReviewDto> _validator;
    private readonly ILogger<CreateReviewCommandHandler> _logger;
    
    public CreateReviewCommandHandler(
        HttpClient httpClient,
        IVacanciesRepository vacanciesRepository,
        ICommandHandler<PrepareToUpdateVacancyRatingCommand.PrepareToUpdateVacancyRatingCommand> prepareToUpdateVacancyRatingCommandHandler,
        IVacanciesContract vacanciesContract, 
        IValidator<CreateReviewDto> validator, 
        ILogger<CreateReviewCommandHandler> logger)
    {
        _httpClient = httpClient;
        _vacanciesRepository = vacanciesRepository;
        _prepareToUpdateVacancyRatingCommandHandler = prepareToUpdateVacancyRatingCommandHandler;
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
        var tokenRequest = new HttpRequestMessage(
            HttpMethod.Get, 
            $"https://localhost:5000/api/users/auth/employee_token/{command.UserId}");
        tokenRequest.Headers.Add("User-Agent", "Locator/1.0");
        tokenRequest.Headers.Add("Api-Gateway", "Signed");

        var tokenResponse = await _httpClient.SendAsync(tokenRequest, cancellationToken);
        if (!tokenResponse.IsSuccessStatusCode)
        {
            throw new GetValidEmployeeAccessTokenException();
        }

        string tokenJson = await tokenResponse.Content.ReadAsStringAsync(cancellationToken);
        var employeeTokenResponse = JsonSerializer.Deserialize<EmployeeTokenResponse>(tokenJson);
        if (employeeTokenResponse?.EmployeeToken?.Token == null)
        {
            throw new GetValidEmployeeAccessTokenException();
        }
        
        string? token = employeeTokenResponse.EmployeeToken.Token;

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
        var request = new HttpRequestMessage(
            HttpMethod.Get, 
            $"https://localhost:5000/api/users/{command.UserId}");
        request.Headers.Add("User-Agent", "Locator/1.0");
        request.Headers.Add("Api-Gateway", "Signed");

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return Errors.GetUserByIdFail().ToFailure();
        }

        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        var userResponse = JsonSerializer.Deserialize<UserResponse>(json);
        if (userResponse == null)
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