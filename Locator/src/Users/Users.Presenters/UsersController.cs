using Framework.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Abstractions;
using Shared.Fails.Exceptions;
using Shared.Options;
using Users.Application.AuthQuery;
using Users.Application.RefreshTokenCommand;
using Users.Contracts.Dto;
using Users.Contracts.Responses;

namespace Users.Presenters;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public UsersController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    [HttpGet("auth")]
    public IActionResult Auth()
    {
        var hhOptions = _configuration.GetSection(HeadHunterOptions.SECTION_NAME).Get<HeadHunterOptions>();
        if (hhOptions == null)
        {
            throw new ConfigurationFailureException("Failed to get hh api options.");
        }
        
        string url = $"https://hh.ru/oauth/authorize?" +
                     $"response_type=code&" +
                     $"client_id={Uri.EscapeDataString(hhOptions.ClientId)}&" +
                     $"redirect_uri={Uri.EscapeDataString(hhOptions.RedirectUri)}&" +
                     $"scope={Uri.EscapeDataString(hhOptions.Scope)}";
        
        return Redirect(url);
    }
    
    [HttpGet("auth/callback")]
    public async Task<IActionResult> Callback(
        [FromServices] IQueryHandler<AuthResponse, AuthQuery> queryHandler,
        [FromQuery] AuthorizationCodeDto request,
        CancellationToken cancellationToken)
    {
        var cookiesOptions = _configuration.GetSection(CookiesOptions.SECTION_NAME).Get<CookiesOptions>();
        if (cookiesOptions == null)
        {
            throw new ConfigurationFailureException("Failed to get cookies options.");
        }
        
        var context = HttpContext;
        var query = new AuthQuery(request);
        var result = await queryHandler.Handle(query, cancellationToken);
        if (result.AccessToken == null)
        {
            return StatusCode(500);
        }
        context.Response.Cookies.Append(cookiesOptions.JwtName, result.AccessToken);
        context.Response.Cookies.Append(cookiesOptions.UserName, result.UserId.ToString() ?? string.Empty);
        
        return Ok();
    }    
    
    [HttpGet("auth/refresh")]
    public async Task<IActionResult> Refresh(
        [FromServices] ICommandHandler<string, RefreshTokenCommand> commandHandler,
        CancellationToken cancellationToken)
    {
        var cookiesOptions = _configuration.GetSection(CookiesOptions.SECTION_NAME).Get<CookiesOptions>();
        if (cookiesOptions == null)
        {
            throw new ConfigurationFailureException("Failed to get cookies options.");
        }
        
        var context = HttpContext;
        string? userIdString = Request.Cookies[cookiesOptions.UserName];
        if (!Guid.TryParse(userIdString, out var userId))
        {
            return BadRequest("Invalid user ID format.");
        }
        var query = new RefreshTokenCommand(userId);
        var result = await commandHandler.Handle(query, cancellationToken);
        
        context.Response.Cookies.Append(cookiesOptions.JwtName, result.Value);
        return result.IsFailure ? result.Error.ToResponse() : Ok();
    }
}