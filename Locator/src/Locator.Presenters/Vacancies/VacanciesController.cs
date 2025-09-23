using Locator.Application.Vacancies;
using Locator.Contracts.Vacancies;
using Locator.Presenters.ResponseExtensions;
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
    [HttpPost("{vacancyId:guid}/reviews")]
    public async Task<IActionResult> CreateReview(
        [FromRoute] Guid vacancyId,
        [FromBody] CreateReviewDto request,
        CancellationToken cancellationToken)
    {
        var result = await _vacanciesService.CreateReview(vacancyId, request, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }
    [HttpGet("{vacancyId:guid}/reviews")]
    public async Task<IActionResult> GetReviewsByVacancyId(
        [FromRoute] Guid vacancyId,
        CancellationToken cancellationToken)
    {
        return Ok("Reviews get");
    }
}