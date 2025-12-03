using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Ratings.Application.GetRatingByVacancyIdQuery;
using Ratings.Contracts.Responses;
using Shared.Abstractions;
using Vacancies.Contracts.Dto;

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