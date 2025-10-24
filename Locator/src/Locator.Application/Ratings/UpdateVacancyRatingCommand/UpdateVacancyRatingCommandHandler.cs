using CSharpFunctionalExtensions;
using FluentValidation;
using Locator.Application.Abstractions;
using Locator.Application.Extensions;
using Locator.Contracts.Ratings.Dtos;
using Locator.Domain.Ratings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

    namespace Locator.Application.Ratings.UpdateVacancyRatingCommand;

    public class UpdateVacancyRatingCommandHandler : ICommandHandler<Guid, UpdateVacancyRatingCommand>
{
    private readonly IRatingsRepository _ratingsRepository;
    private readonly IRatingsReadDbContext _ratingsDbContext;
    private readonly IValidator<UpdateVacancyRatingDto> _validator;
        private readonly ILogger<UpdateVacancyRatingCommandHandler> _logger;
    
        public UpdateVacancyRatingCommandHandler(
        IRatingsRepository ratingsRepository,
        IRatingsReadDbContext ratingsDbContext,
        IValidator<UpdateVacancyRatingDto> validator,
        ILogger<UpdateVacancyRatingCommandHandler> logger)
    {
        _ratingsRepository = ratingsRepository;
        _ratingsDbContext = ratingsDbContext;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, Failure>> Handle(
        UpdateVacancyRatingCommand command,
        CancellationToken cancellationToken)
    {
        // Input data validation
        var validationResult = await _validator.ValidateAsync(command.vacancyRatingDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors().ToFailure();
        }
        
        // Create VacancyRating
        (double averageMark, long vacancyId) = (command.vacancyRatingDto.AverageMark, command.vacancyRatingDto.VacancyId);
        var rating = new VacancyRating(averageMark, vacancyId);
        
        // Search VacancyRating
        var ratingRecord = await _ratingsDbContext.ReadVacancyRatings
            .Where(r => r.EntityId == vacancyId)
            .FirstOrDefaultAsync(cancellationToken);
        if (ratingRecord == null)
        {
            // Save VacancyRating
            await _ratingsRepository.CreateVacancyRatingAsync(rating, cancellationToken);
            _logger.LogInformation(
                "Rating created with id={RatingId} on vacancy with id={VacancyId}", rating.Id, vacancyId);
            return rating.Id;
        }
        
        // Update VacancyRating
        var ratingRecordIdResult = await _ratingsRepository.UpdateVacancyRatingAsync(rating, cancellationToken);
        if (ratingRecordIdResult.IsFailure)
        {
            return ratingRecordIdResult.Error.ToFailure();
        }

        var ratingRecordId = ratingRecordIdResult.Value;
        _logger.LogInformation("Rating updated with id={RatingId} on vacancy with id={VacancyId}", ratingRecord.Id, vacancyId);
        
        return ratingRecordId;
    }
}