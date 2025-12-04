using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ratings.Application.GetRatingByVacancyIdQuery;
using Ratings.Contracts.Dto;
using Ratings.Contracts.Responses;
using Shared.Abstractions;

namespace Ratings.Presenters;

[ApiController]
[Route("api/ratings")]
[Authorize]
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
}