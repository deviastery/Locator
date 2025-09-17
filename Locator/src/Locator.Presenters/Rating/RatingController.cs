using Locator.Application.Rating;
using Microsoft.AspNetCore.Mvc;

namespace Locator.Presenters.Rating;

[ApiController]
[Route("[controller]")]
public class RatingController : ControllerBase
{
    private readonly IRatingService _ratingService;

    public RatingController(IRatingService ratingService)
    {
        _ratingService = ratingService;
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
        var reviewId = await _ratingService.CreateVacancyRating(vacancyId, averageMark, cancellationToken);
        return Ok(reviewId);
    }
}