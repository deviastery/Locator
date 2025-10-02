using Locator.Application.Abstractions;
using Locator.Application.Vacancies.CreateReviewCommand;
using Locator.Application.Vacancies.GetReviewsByVacancyIdQuery;
using Locator.Application.Vacancies.GetVacanciesWithFiltersQuery;
using Locator.Application.Vacancies.GetVacancyByIdQuery;
using Locator.Contracts.Vacancies;
using Locator.Presenters.ResponseExtensions;
using Microsoft.AspNetCore.Mvc;

namespace Locator.Presenters.Vacancies;

[ApiController]
[Route("[controller]")]
public class VacanciesController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromServices] IQueryHandler<VacanciesResponse, GetVacanciesWithFiltersQuery> queryHandler,
        [FromQuery] GetVacanciesDto request,
        CancellationToken cancellationToken)
    {
        var query = new GetVacanciesWithFiltersQuery(request);
        var result = await queryHandler.Handle(query, cancellationToken);
        return Ok(result);
    }
    [HttpGet("{vacancyId:guid}")]
    public async Task<IActionResult> GetById(
        [FromServices] IQueryHandler<VacancyResponse, GetVacancyByIdQuery> queryHandler,
        [FromRoute] Guid vacancyId,
        CancellationToken cancellationToken)
    {
        var dto = new GetVacancyIdDto(vacancyId);
        var query = new GetVacancyByIdQuery(dto);
        var result = await queryHandler.Handle(query, cancellationToken);
        return Ok(result);
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
        [FromServices] IQueryHandler<ReviewsByVacancyIdResponse, GetReviewsByVacancyIdQuery> queryHandler,
        [FromRoute] Guid vacancyId,
        CancellationToken cancellationToken)
    {
        var dto = new GetVacancyIdDto(vacancyId);
        var query = new GetReviewsByVacancyIdQuery(dto);
        var result = await queryHandler.Handle(query, cancellationToken);
        return Ok(result);
    }
}