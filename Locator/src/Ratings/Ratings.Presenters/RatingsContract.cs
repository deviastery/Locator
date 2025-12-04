using CSharpFunctionalExtensions;
using Ratings.Application.GetRatingByVacancyIdQuery;
using Ratings.Application.GetVacancyRatingsQuery;
using Ratings.Application.UpdateVacancyRatingCommand;
using Ratings.Contracts;
using Ratings.Contracts.Dto;
using Ratings.Contracts.Responses;
using Shared;
using Shared.Abstractions;

namespace Ratings.Presenters;

public class RatingsContract : IRatingsContract
{
    private readonly ICommandHandler<Guid, UpdateVacancyRatingCommand> _updateVacancyRatingCommandHandler;
    private readonly IQueryHandler<RatingByVacancyIdResponse, GetRatingByVacancyIdQuery> _getRatingDtoByVacancyIdQueryHandler;
    private readonly IQueryHandler<VacancyRatingsResponse, GetVacancyRatingsQuery> _getVacancyRatingsQueryHandler;

    public RatingsContract(
        ICommandHandler<Guid, UpdateVacancyRatingCommand> updateVacancyRatingCommandHandler, 
        IQueryHandler<RatingByVacancyIdResponse, GetRatingByVacancyIdQuery> getRatingDtoByVacancyIdQueryHandler, 
        IQueryHandler<VacancyRatingsResponse, GetVacancyRatingsQuery> getVacancyRatingsQueryHandler)
    {
        _updateVacancyRatingCommandHandler = updateVacancyRatingCommandHandler;
        _getRatingDtoByVacancyIdQueryHandler = getRatingDtoByVacancyIdQueryHandler;
        _getVacancyRatingsQueryHandler = getVacancyRatingsQueryHandler;
    }

    public async Task<VacancyRatingDto?> GetRatingDtoByVacancyIdAsync(
        GetRatingByVacancyIdDto dto, CancellationToken cancellationToken)
    {
        var getRatingDtoByVacancyIdQuery = new GetRatingByVacancyIdQuery(dto);
        var rating = await _getRatingDtoByVacancyIdQueryHandler
            .Handle(getRatingDtoByVacancyIdQuery, cancellationToken);
        return rating.Rating ?? null;
    }
    
    public async Task<VacancyRatingDto[]> GetVacancyRatingsDtoAsync(
        CancellationToken cancellationToken)
    {
        var getRatingsDtoByVacancyIdQuery = new GetVacancyRatingsQuery();
        var ratings = await _getVacancyRatingsQueryHandler
            .Handle(getRatingsDtoByVacancyIdQuery, cancellationToken);
        return ratings.VacancyRatings ?? [];
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