using Locator.Application.Rating;
using Microsoft.AspNetCore.Mvc;

namespace Locator.Presenters.Rating;

[ApiController]
[Route("[controller]")]
public class RatingsController : ControllerBase
{
    private readonly IRatingsService _ratingsService;

    public RatingsController(IRatingsService ratingsService)
    {
        _ratingsService = ratingsService;
    }

    [HttpGet("{vacancyId:guid}/rating")]
    public async Task<IActionResult> GetByVacancyId(
        [FromRoute] Guid vacancyId,
        CancellationToken cancellationToken)
    {
        return Ok("Rating get");
    }
    
    [HttpPost("{vacancyId:guid}/rating")]
    public async Task<IActionResult> Create(
        [FromRoute] Guid vacancyId,
        [FromBody] double averageMark,
        CancellationToken cancellationToken)
    {
        var reviewId = await _ratingsService.CreateVacancyRating(vacancyId, averageMark, cancellationToken);
        return Ok(reviewId);
    }
}