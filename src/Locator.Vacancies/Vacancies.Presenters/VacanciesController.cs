using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Framework.Extensions;
using HeadHunter.Contracts.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Abstractions;
using Vacancies.Application.CreateNegotiationCommand;
using Vacancies.Application.CreateReviewCommand;
using Vacancies.Application.GetNegotiationByVacancyIdQuery;
using Vacancies.Application.GetNegotiationsQuery;
using Vacancies.Application.GetReviewsByVacancyIdQuery;
using Vacancies.Application.GetVacanciesWithFiltersQuery;
using Vacancies.Application.GetVacancyByIdQuery;
using Vacancies.Contracts.Dto;
using Vacancies.Contracts.Responses;

namespace Vacancies.Presenters;

[ApiController]
[Route("api/vacancies")]
public class VacanciesController : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get(
        [FromServices] IQueryHandler<VacanciesResponse, GetVacanciesWithFiltersQuery> queryHandler,
        [FromQuery] GetVacanciesDto request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                           User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var userId))
        {
            return Unauthorized();
        }
        var dto = new GetVacanciesByUserIdDto(userId, request);
        var query = new GetVacanciesWithFiltersQuery(dto);
        var result = await queryHandler.Handle(query, cancellationToken);
        return Ok(result);
    }
    [HttpGet("{vacancyId:long}")]
    [Authorize]
    public async Task<IActionResult> GetById(
        [FromServices] IQueryHandler<VacancyResponse, GetVacancyByIdQuery> queryHandler,
        [FromRoute] long vacancyId,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var userId))
        {
            return Unauthorized();
        }
        var dto = new GetVacancyIdDto(vacancyId, userId);
        var query = new GetVacancyByIdQuery(dto);
        var result = await queryHandler.Handle(query, cancellationToken);
        return Ok(result);
    }
    [HttpGet("{vacancyId:long}/negotiations")]
    [Authorize]
    public async Task<IActionResult> GetNegotiationByVacancyId(
        [FromServices] IQueryHandler<NegotiationResponse, GetNegotiationByVacancyIdQuery> queryHandler,
        [FromRoute] long vacancyId,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var userId))
        {
            return Unauthorized();
        }
        var dto = new GetNegotiationByVacancyIdDto(userId, vacancyId);
        var query = new GetNegotiationByVacancyIdQuery(dto);
        var result = await queryHandler.Handle(query, cancellationToken);
        return Ok(result);
    }
    
    [HttpPost("{vacancyId:long}/reviews")]
    [Authorize]
    public async Task<IActionResult> CreateReview(
        [FromServices] ICommandHandler<Guid, CreateReviewCommand> commandHandler,
        [FromRoute] long vacancyId,
        [FromBody] CreateReviewDto request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var userId))
        {
            return Unauthorized();
        }
        var command = new CreateReviewCommand(vacancyId, userId, request);
        var result = await commandHandler.Handle(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [HttpGet("{vacancyId:long}/reviews")]
    public async Task<IActionResult> GetReviewsByVacancyId(
        [FromServices] IQueryHandler<ReviewsByVacancyIdResponse, GetReviewsByVacancyIdQuery> queryHandler,
        [FromRoute] long vacancyId,
        CancellationToken cancellationToken)
    {
        var query = new GetReviewsByVacancyIdQuery(vacancyId);
        var result = await queryHandler.Handle(query, cancellationToken);
        return Ok(result);
    }
    
    [HttpGet("negotiations")]
    [Authorize]
    public async Task<IActionResult> GetNegotiations(
        [FromServices] IQueryHandler<NegotiationsResponse, GetNegotiationsQuery> queryHandler,
        [FromQuery] GetNegotiationsDto request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var userId))
        {
            return Unauthorized();
        }
        var dto = new GetNegotiationsByUserIdDto(userId, request);
        var query = new GetNegotiationsQuery(dto);
        var result = await queryHandler.Handle(query, cancellationToken);
        return Ok(result);
    }
    
    [HttpPost("{vacancyId:long}/negotiations")]
    [Authorize]
    public async Task<IActionResult> CreateNegotiation(
        [FromServices] ICommandHandler<CreateNegotiationCommand> commandHandler,
        [FromRoute] long vacancyId,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var userId))
        {
            return Unauthorized();
        }
        var command = new CreateNegotiationCommand(vacancyId, userId);
        var result = await commandHandler.Handle(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok();
    }
}