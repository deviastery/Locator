using System.Text;
using System.Text.Json;
using CSharpFunctionalExtensions;
using FluentValidation;
using Framework.Extensions;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Abstractions;
using Vacancies.Application.Fails;
using Vacancies.Contracts.Dto;
using Vacancies.Domain;

namespace Vacancies.Application.PrepareToUpdateVacancyRatingCommand;

    public class PrepareToUpdateVacancyRatingCommandHandler : ICommandHandler<PrepareToUpdateVacancyRatingCommand>
{
    private readonly HttpClient _httpClient;
    private readonly IVacanciesRepository _vacanciesRepository;
    private readonly ILogger<PrepareToUpdateVacancyRatingCommandHandler> _logger;
    private readonly IValidator<UpdateVacancyRatingDto> _validator;
    
    public PrepareToUpdateVacancyRatingCommandHandler(
        HttpClient httpClient,
        IVacanciesRepository vacanciesRepository,
        ILogger<PrepareToUpdateVacancyRatingCommandHandler> logger, 
        IValidator<UpdateVacancyRatingDto> validator)
    {
        _httpClient = httpClient;
        _vacanciesRepository = vacanciesRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<UnitResult<Failure>> Handle(
        PrepareToUpdateVacancyRatingCommand command, 
        CancellationToken cancellationToken)
    {
        // Get all Reviews of a Vacancy
        var reviewsVacancyId = await _vacanciesRepository.GetReviewsByVacancyIdAsync(
            command.VacancyId, cancellationToken);
        if (reviewsVacancyId.Count == 0)
        {
            return Errors.General.NotFound($"Reviews not found by vacancy ID={command.VacancyId}").ToFailure();
        }

        // Calculate average mark
        var averageMarkResult = Review.CalculateAverageMark(reviewsVacancyId);
        if (averageMarkResult.IsFailure)
        {
            return averageMarkResult.Error.ToFailure();
        }
        
        // Input data validation
        var updateVacancyRatingDto = new UpdateVacancyRatingDto(command.VacancyId, averageMarkResult.Value);
        var validationResult = await _validator.ValidateAsync(updateVacancyRatingDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors().ToFailure();
        }
        
        // Create vacancy Rating
        var requestBody = new
        {
            averageMark = updateVacancyRatingDto.AverageMark,
        };

        string jsonRequest = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        
        var request = new HttpRequestMessage(
            HttpMethod.Post, 
            $"https://localhost:5001/api/ratings/vacancies/{updateVacancyRatingDto.VacancyId}")
        {
            Content = content,
        };
        request.Headers.Add("User-Agent", "Locator/1.0");
        request.Headers.Add("Api-Gateway", "Signed");

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return Errors.CreateVacancyRatingFail().ToFailure();
        }

        string jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);
        var ratingId = JsonSerializer.Deserialize<Guid>(jsonResponse);
        if (ratingId == Guid.Empty)
        {
            return Errors.CreateVacancyRatingFail().ToFailure();
        }

        return default;
    }
}