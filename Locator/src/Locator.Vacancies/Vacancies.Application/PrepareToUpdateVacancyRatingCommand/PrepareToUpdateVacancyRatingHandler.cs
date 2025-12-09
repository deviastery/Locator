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
    private readonly ICommandHandler<Guid, CreateRequestVacancyRatingCommand.CreateRequestVacancyRatingCommand> 
        _createRequestVacancyRatingCommandHandler;
    private readonly IVacanciesRepository _vacanciesRepository;
    private readonly ILogger<PrepareToUpdateVacancyRatingCommandHandler> _logger;
    private readonly IValidator<UpdateVacancyRatingDto> _validator;
    
    public PrepareToUpdateVacancyRatingCommandHandler(
        ICommandHandler<Guid, CreateRequestVacancyRatingCommand.CreateRequestVacancyRatingCommand> createRequestVacancyRatingCommandHandler,
        IVacanciesRepository vacanciesRepository,
        ILogger<PrepareToUpdateVacancyRatingCommandHandler> logger, 
        IValidator<UpdateVacancyRatingDto> validator)
    {
        _createRequestVacancyRatingCommandHandler = createRequestVacancyRatingCommandHandler;
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
        var dto = new CreateRequestVacancyRatingDto(command.VacancyId, averageMarkResult.Value);
        var ratingIdResponse = await _createRequestVacancyRatingCommandHandler.Handle(
            new CreateRequestVacancyRatingCommand.CreateRequestVacancyRatingCommand(dto),
            cancellationToken);
        if (ratingIdResponse.IsFailure)
        {
            return ratingIdResponse.Error.ToFailure();
        }
        return default;
    }
}