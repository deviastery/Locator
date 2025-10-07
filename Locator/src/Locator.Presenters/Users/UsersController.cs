using Locator.Application.Abstractions;
using Locator.Application.Users.AuthQuery;
using Locator.Contracts.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Locator.Presenters.Users;

[ApiController]
[Route("[controller]")]
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
        var redirectUri = _configuration["HhApi:RedirectUri"];
        var clientId = _configuration["HhApi:ClientId"];
        var scope = "user:read";

        var url = $"https://hh.ru/oauth/authorize?" +
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
        var query = new AuthQuery(request);
        var result = await queryHandler.Handle(query, cancellationToken);
        return Ok(result);
    }
    
    [HttpGet("{vacancyId:guid}")]
    public async Task<IActionResult> GetUsersRespondedToVacancyById(
        [FromRoute] Guid vacancyId,
        CancellationToken cancellationToken)
    {
        return Ok("Users get");
    }
}