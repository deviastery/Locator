using CSharpFunctionalExtensions;
using Ratings.Application;
using Ratings.Application.UpdateVacancyRatingCommand;
using Ratings.Contracts;
using Ratings.Contracts.Dto;
using Ratings.Domain;
using Shared;
using Shared.Abstractions;

namespace Ratings.Presenters;

public class RatingsContract : IRatingsContract
{
    private readonly IRatingsRepository _ratingsRepository;
    private readonly ICommandHandler<Guid, UpdateVacancyRatingCommand> _updateVacancyRatingCommandHandler;

    public RatingsContract(
        IRatingsRepository ratingsRepository,
        ICommandHandler<Guid, UpdateVacancyRatingCommand> updateVacancyRatingCommandHandler)
    {
        _ratingsRepository = ratingsRepository;
        _updateVacancyRatingCommandHandler = updateVacancyRatingCommandHandler;
    }

    public async Task<Result<VacancyRatingDto, Error>> GetRatingDtoByVacancyIdAsync(
        long vacancyId, CancellationToken cancellationToken)
    {
        var ratingResult = await _ratingsRepository.GetRatingByVacancyIdAsync(vacancyId, cancellationToken);
        if (ratingResult.IsFailure)
        {
            return ratingResult.Error;
        }
        var rating = ratingResult.Value;
        
        var ratingDto = new VacancyRatingDto(
            rating.Id,
            rating.Value,
            rating.EntityId);
        
        return ratingDto;
    }
    
    public async Task<Result<VacancyRatingDto[], Error>> GetRatingsDtoAsync(
        CancellationToken cancellationToken)
    {
        (_, bool isFailure, VacancyRating[]? ratings, Error? error) = await _ratingsRepository
            .GetRatingsAsync(cancellationToken);
        if (isFailure)
        {
            return error;
        }

        var ratingsDto = ratings.Select(r =>
            new VacancyRatingDto(
                r.Id,
                r.Value,
                r.EntityId));
            
        return ratingsDto.ToArray();
    }

    public async Task<Result<Guid, Failure>> UpdateVacancyRatingAsync(
        UpdateVacancyRatingDto updateVacancyRatingDto,
        CancellationToken cancellationToken)
    {
        var updateVacancyRatingCommand = new UpdateVacancyRatingCommand(updateVacancyRatingDto);
        var createVacancyRatingResult = await _updateVacancyRatingCommandHandler
            .Handle(updateVacancyRatingCommand, cancellationToken);

        return createVacancyRatingResult;
    }
}