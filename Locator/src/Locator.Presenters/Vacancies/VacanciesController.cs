using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Locator.Application.Abstractions;
using Locator.Application.Vacancies.CreateReviewCommand;
using Locator.Application.Vacancies.GetReviewsByVacancyIdQuery;
using Locator.Application.Vacancies.GetVacanciesWithFiltersQuery;
using Locator.Application.Vacancies.GetVacancyByIdQuery;
using Locator.Contracts.Vacancies;
using Locator.Presenters.ResponseExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Locator.Presenters.Vacancies;

[ApiController]
[Route("api/vacancies")]
[Authorize]
public class VacanciesController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromServices] IQueryHandler<EmployeeVacanciesResponse, GetVacanciesWithFiltersQuery> queryHandler,
        [FromQuery] GetVacanciesDto request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                           User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var userId))
        {
            return Unauthorized("User ID not found in token.");
        }
        var dto = new GetVacanciesByUserId(userId, request);
        var query = new GetVacanciesWithFiltersQuery(dto);
        var result = await queryHandler.Handle(query, cancellationToken);
        return Ok(result);
    }
    [HttpGet("{vacancyId:guid}")]
    public async Task<IActionResult> GetById(
        [FromServices] IQueryHandler<VacancyResponse, GetVacancyByIdQuery> queryHandler,
        [FromRoute] string vacancyId,
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
        [FromRoute] string vacancyId,
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
        [FromRoute] string vacancyId,
        CancellationToken cancellationToken)
    {
        var dto = new GetVacancyIdDto(vacancyId);
        var query = new GetReviewsByVacancyIdQuery(dto);
        var result = await queryHandler.Handle(query, cancellationToken);
        return Ok(result);
    }
}