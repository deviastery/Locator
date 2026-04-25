using CSharpFunctionalExtensions;
using FluentValidation;
using Framework.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ratings.Contracts.Dto;
using Ratings.Domain;
using Shared;
using Shared.Abstractions;

namespace Ratings.Application.UpdateVacancyRatingCommand;

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
        var validationResult = await _validator.ValidateAsync(command.VacancyRatingDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors().ToFailure();
        }
        
        // Create vacancy Rating
        (double averageMark, long vacancyId) = (command.VacancyRatingDto.AverageMark, command.VacancyRatingDto.VacancyId);
        var rating = new VacancyRating(averageMark, vacancyId);
        
        // Search vacancy Rating
        var ratingRecord = await _ratingsDbContext.ReadVacancyRatings
            .Where(r => r.EntityId == vacancyId)
            .FirstOrDefaultAsync(cancellationToken);
        if (ratingRecord == null)
        {
            // Save vacancy Rating
            var ratingIdResult = await _ratingsRepository.CreateVacancyRatingAsync(rating, cancellationToken);
            if (ratingIdResult.IsFailure)
            {
                return ratingIdResult.Error.ToFailure();
            }
            _logger.LogInformation(
                "Rating created with id={RatingId} on vacancy with id={VacancyId}", rating.Id, vacancyId);
            return rating.Id;
        }
        
        // Update vacancy Rating
        var ratingRecordIdResult = await _ratingsRepository.UpdateVacancyRatingAsync(rating, cancellationToken);
        if (ratingRecordIdResult.IsFailure)
        {
            return ratingRecordIdResult.Error.ToFailure();
        }
        
        // Return results
        var ratingRecordId = ratingRecordIdResult.Value;
        _logger.LogInformation("Rating updated with id={RatingId} on vacancy with id={VacancyId}", ratingRecord.Id, vacancyId);
        
        return ratingRecordId;
    }
}