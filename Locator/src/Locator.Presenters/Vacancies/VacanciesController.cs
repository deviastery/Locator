using Locator.Application.Abstractions;
using Locator.Application.Vacancies;
using Locator.Application.Vacancies.CreateReview;
using Locator.Contracts.Vacancies;
using Locator.Presenters.ResponseExtensions;
using Microsoft.AspNetCore.Mvc;

namespace Locator.Presenters.Vacancies;

[ApiController]
[Route("[controller]")]
public class VacanciesController : ControllerBase
{
    public VacanciesController()
    {
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
        [FromServices] IHandler<Guid, CreateReviewCommand> handler,
        [FromRoute] Guid vacancyId,
        [FromBody] CreateReviewDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreateReviewCommand(vacancyId, request);
        var result = await handler.Handle(command, cancellationToken);
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