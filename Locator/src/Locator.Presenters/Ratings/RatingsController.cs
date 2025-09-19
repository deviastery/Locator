using Locator.Application.Ratings;
using Locator.Contracts.Ratings;
using Microsoft.AspNetCore.Mvc;

namespace Locator.Presenters.Ratings;

[ApiController]
[Route("[controller]")]
public class RatingsController : ControllerBase
{
    private readonly IRatingsService _ratingsService;

    public RatingsController(IRatingsService ratingsService)
    {
        _ratingsService = ratingsService;
    }

    [HttpGet("/vacancies/{vacancyId:guid}")]
    public async Task<IActionResult> GetByVacancyId(
        [FromRoute] Guid vacancyId,
        CancellationToken cancellationToken)
    {
        return Ok("Rating get");
    }
    
    [HttpPost("/vacancies/{vacancyId:guid}")]
    public async Task<IActionResult> Create(
        [FromRoute] Guid vacancyId,
        [FromBody] double averageMark,
        CancellationToken cancellationToken)
    {
        var vacancyRatingDto = new CreateVacancyRatingDto(vacancyId, averageMark);
        var reviewId = await _ratingsService.CreateVacancyRating(vacancyRatingDto, cancellationToken);
        return Ok(reviewId);
    }
}