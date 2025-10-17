using Locator.Application.Abstractions;
using Locator.Application.Ratings.GetRatingByVacancyIdQuery;
using Locator.Contracts.Ratings;
using Locator.Contracts.Vacancies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Locator.Presenters.Ratings;

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
        var dto = new GetVacancyIdDto(vacancyId);
        var query = new GetRatingByVacancyIdQuery(dto);
        var result = await queryHandler.Handle(query, cancellationToken);
        return Ok(result);
    }
}