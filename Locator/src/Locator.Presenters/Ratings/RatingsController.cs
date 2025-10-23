using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Locator.Application.Abstractions;
using Locator.Application.Ratings.GetRatingByVacancyIdQuery;
using Locator.Contracts.Ratings.Responses;
using Locator.Contracts.Vacancies.Dtos;
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
        if (!Guid.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        var dto = new GetVacancyIdDto(vacancyId, userId);
        var query = new GetRatingByVacancyIdQuery(dto);
        var result = await queryHandler.Handle(query, cancellationToken);
        return Ok(result);
    }
}