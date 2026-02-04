using Framework.Extensions;
using Microsoft.AspNetCore.Mvc;
using Ratings.Application.GetRatingByVacancyIdQuery;
using Ratings.Application.GetVacancyRatingsQuery;
using Ratings.Application.UpdateVacancyRatingCommand;
using Ratings.Contracts.Dto;
using Ratings.Contracts.Responses;
using Shared.Abstractions;

namespace Ratings.Presenters;

[ApiController]
[Route("api/ratings")]
public class RatingsController : ControllerBase
{
    [HttpGet("vacancies/{vacancyId:long}")]
    public async Task<IActionResult> GetByVacancyId(
        [FromServices] IQueryHandler<RatingByVacancyIdResponse, GetRatingByVacancyIdQuery> queryHandler,
        [FromRoute] long vacancyId,
        CancellationToken cancellationToken)
    {
        var dto = new GetRatingByVacancyIdDto(vacancyId);
        var query = new GetRatingByVacancyIdQuery(dto);
        var result = await queryHandler.Handle(query, cancellationToken);
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetVacancyRatings(
        [FromServices] IQueryHandler<VacancyRatingsResponse, GetVacancyRatingsQuery> queryHandler,
        CancellationToken cancellationToken)
    {
        var query = new GetVacancyRatingsQuery();
        var result = await queryHandler.Handle(query, cancellationToken);
        return Ok(result);
    }
    
    [HttpPost("vacancies/{vacancyId:long}")]
    public async Task<IActionResult> UpdateVacancyRating(
        [FromServices] ICommandHandler<Guid, UpdateVacancyRatingCommand> commandHandler,
        [FromRoute] long vacancyId,
        [FromBody] VacancyRatingValueDto request,
        CancellationToken cancellationToken)
    {
        var dto = new UpdateVacancyRatingDto(vacancyId, request.AverageMark);
        var command = new UpdateVacancyRatingCommand(dto);
        var result = await commandHandler.Handle(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }
}