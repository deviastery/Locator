using Locator.Application.Ratings;
using Locator.Contracts.Ratings;
using Microsoft.AspNetCore.Mvc;

namespace Locator.Presenters.Ratings;

[ApiController]
[Route("[controller]")]
public class RatingsController : ControllerBase
{
    public RatingsController()
    {
    }

    [HttpGet("/vacancies/{vacancyId:guid}")]
    public async Task<IActionResult> GetByVacancyId(
        [FromRoute] Guid vacancyId,
        CancellationToken cancellationToken)
    {
        return Ok("Rating get");
    }
}