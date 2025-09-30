using Locator.Application.Abstractions;
using Locator.Application.Vacancies;
using Locator.Application.Vacancies.CreateReviewCommand;
using Locator.Application.Vacancies.GetVacanciesWithFiltersQuery;
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
        [FromServices] IQueryHandler<VacancyResponse, GetVacanciesWithFiltersQuery> commandHandler,
        [FromQuery] GetVacanciesDto request,
        CancellationToken cancellationToken)
    {
        var query = new GetVacanciesWithFiltersQuery(request);
        var result = await commandHandler.Handle(query, cancellationToken);
        return Ok(result);
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
        [FromServices] ICommandHandler<Guid, CreateReviewCommand> commandHandler,
        [FromRoute] Guid vacancyId,
        [FromBody] CreateReviewDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreateReviewCommand(vacancyId, request);
        var result = await commandHandler.Handle(command, cancellationToken);
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