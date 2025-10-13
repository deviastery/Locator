using Locator.Application.Abstractions;
using Locator.Application.Users.AuthQuery;
using Locator.Application.Users.RefreshTokenQuery;
using Locator.Contracts.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared;

namespace Locator.Presenters.Users;

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
        string? redirectUri = _configuration["HhApi:RedirectUri"];
        string? clientId = _configuration["HhApi:ClientId"];
        string scope = "user:read";
        if (redirectUri == null || clientId == null)
        {
            return BadRequest();
        }
        
        string url = $"https://hh.ru/oauth/authorize?" +
                     $"response_type=code&" +
                     $"client_id={Uri.EscapeDataString(clientId)}&" +
                     $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                     $"scope={Uri.EscapeDataString(scope)}";
        
        return Redirect(url);
    }
    
    [HttpGet("auth/callback")]
    public async Task<IActionResult> Callback(
        [FromServices] IQueryHandler<AuthResponse, AuthQuery> queryHandler,
        [FromQuery] AuthorizationCodeDto request,
        CancellationToken cancellationToken)
    {
        var context = HttpContext;
        var query = new AuthQuery(request);
        var result = await queryHandler.Handle(query, cancellationToken);
        if (result.AccessToken == null)
        {
            return StatusCode(500);
        }
        context.Response.Cookies.Append("tasty-cookie", result.AccessToken);
        
        return Ok();
    }    
    
    [HttpGet("auth/refresh")]
    public async Task<IActionResult> Refresh(
        [FromServices] IQueryHandler<RefreshTokenResponse, RefreshTokenQuery> queryHandler,
        [FromQuery] string refreshToken,
        CancellationToken cancellationToken)
    {
        var context = HttpContext;
        var query = new RefreshTokenQuery(refreshToken);
        var result = await queryHandler.Handle(query, cancellationToken);

        if (result.jwtToken == null)
        {
            return Unauthorized();
        }
        
        context.Response.Cookies.Append("tasty-cookie", result.jwtToken);
        return Ok();
    }
    
    [HttpGet("{vacancyId:guid}")]
    public async Task<IActionResult> GetUsersRespondedToVacancyById(
        [FromRoute] Guid vacancyId,
        CancellationToken cancellationToken)
    {
        return Ok("Users get");
    }
}