using System.Text;
using System.Text.Json;
using CSharpFunctionalExtensions;
using FluentValidation;
using Framework.Extensions;
using HeadHunter.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared;
using Shared.Abstractions;
using Shared.Options;
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
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ModeratorOptions _moderatorOptions;
    private readonly IValidator<CreateReviewDto> _validator;
    private readonly ILogger<CreateReviewCommandHandler> _logger;
    
    public CreateReviewCommandHandler(
        IVacanciesRepository vacanciesRepository,
        ICommandHandler<PrepareToUpdateVacancyRatingCommand.PrepareToUpdateVacancyRatingCommand> prepareToUpdateVacancyRatingCommandHandler, 
        IQueryHandler<EmployeeTokenResponse, GetRequestEmployeeTokenQuery.GetRequestEmployeeTokenQuery> getRequestEmployeeTokenQuery,
        IQueryHandler<UserResponse, GetRequestUserByIdQuery.GetRequestUserByIdQuery> getRequestUserByIdQuery,
        IVacanciesContract vacanciesContract, 
        IHttpClientFactory httpClientFactory,
        IOptions<ModeratorOptions> moderatorOptions, 
        IValidator<CreateReviewDto> validator, 
        ILogger<CreateReviewCommandHandler> logger)
    {
        _vacanciesRepository = vacanciesRepository;
        _prepareToUpdateVacancyRatingCommandHandler = prepareToUpdateVacancyRatingCommandHandler;
        _getRequestEmployeeTokenQuery = getRequestEmployeeTokenQuery;
        _getRequestUserByIdQuery = getRequestUserByIdQuery;
        _vacanciesContract = vacanciesContract;
        _httpClientFactory = httpClientFactory;
        _moderatorOptions = moderatorOptions.Value;
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
        
        // Moderate comment
        if(review.Comment != null)
        {
            var moderationResult = await ModerateCommentAsync(
                review.Comment, 
                cancellationToken);
            if (moderationResult.IsFailure)
            {
                return moderationResult.Error.ToFailure();
            }
        }
        
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
    
    private async Task<UnitResult<Failure>> ModerateCommentAsync(
        string comment, 
        CancellationToken cancellationToken)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ModerationClient");
            
            var requestBody = new
            {
                text = comment,
            };

            string jsonRequest = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        
            var request = new HttpRequestMessage(
                HttpMethod.Post, 
                "moderate")
            {
                Content = content,
            };
            request.Headers.Add("User-Agent", "Locator/1.0");

            var response = await client.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Moderation service returned status {StatusCode}", response.StatusCode);
                return Errors.General.Failure("Failed to moderate comment").ToFailure();
            }

            string json = await response.Content.ReadAsStringAsync(cancellationToken);
            var moderationResult = JsonSerializer.Deserialize<Dictionary<string, int>>(json);
            if (moderationResult == null)
            {
                return Errors.General.Failure("Invalid moderation response").ToFailure();
            }
            
            var thresholds = new Dictionary<string, int>
            {
                ["insult"] = int.Parse(_moderatorOptions.Insult),
                ["threat"] = int.Parse(_moderatorOptions.Threat),
                ["obscenity"] = int.Parse(_moderatorOptions.Obscenity),
                ["meaningless"] = int.Parse(_moderatorOptions.Meaningless),
            };

            foreach ((string category, int threshold) in thresholds)
            {
                if (moderationResult.TryGetValue(category, out int level) && level >= threshold)
                {
                    _logger.LogWarning(
                        "Comment rejected due to high {Category} level: {Level}. Comment: '{Comment}'",
                        category,
                        level,
                        comment);

                    return Errors.General.Validation(
                        $"Comment contains inappropriate content ({category})").ToFailure();
                }
            }

            return Result.Success<Failure>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during comment moderation");
            return Errors.General.Failure("Comment moderation failed").ToFailure();
        }
    }
}