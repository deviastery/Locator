using Locator.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Locator.Presenters;

[ApiController]
[Route("[controller]")]
public class VacanciesController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] GetVacanciesDto request,
        CancellationToken cancellationToken)
    {
        return Ok("Vacancies get");
    }
    [HttpGet("{vacancyId:guid}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid vacancyId,
        CancellationToken cancellationToken)
    {
        return Ok("Vacancy get");
    }
    [HttpPut("{vacancyId:guid}/reviews")]
    public async Task<IActionResult> AddReview(
        [FromRoute] Guid vacancyId,
        [FromBody] AddReviewDto request,
        CancellationToken cancellationToken)
    {
        return Ok("Review created");
    }
    [HttpGet("{vacancyId:guid}/reviews")]
    public async Task<IActionResult> GetReviewsByVacancyId(
        [FromRoute] Guid vacancyId,
        CancellationToken cancellationToken)
    {
        return Ok("Reviews get");
    }
}