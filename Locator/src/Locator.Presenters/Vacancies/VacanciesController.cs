using Locator.Application.Vacancies;
using Locator.Contracts.Vacancies;
using Microsoft.AspNetCore.Mvc;

namespace Locator.Presenters.Vacancies;

[ApiController]
[Route("[controller]")]
public class VacanciesController : ControllerBase
{
    private readonly IVacanciesService _vacanciesService;

    public VacanciesController(IVacanciesService vacanciesService)
    {
        _vacanciesService = vacanciesService;
    }
    
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
        var reviewId = await _vacanciesService.AddReview(vacancyId, request, cancellationToken);
        return Ok(reviewId);
    }
    [HttpGet("{vacancyId:guid}/reviews")]
    public async Task<IActionResult> GetReviewsByVacancyId(
        [FromRoute] Guid vacancyId,
        CancellationToken cancellationToken)
    {
        return Ok("Reviews get");
    }
}