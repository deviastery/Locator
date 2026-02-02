using Framework.Extensions;
using HeadHunter.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Abstractions;
using Shared.Fails.Exceptions;
using Shared.Options;
using Users.Application.AuthQuery;
using Users.Application.CreateUserCommand;
using Users.Application.GetUserQuery;
using Users.Application.GetValidEmployeeTokenByUserIdQuery;
using Users.Application.LogoutCommand;
using Users.Application.RefreshTokenCommand;
using Users.Contracts.Dto;
using Users.Contracts.Responses;

namespace Users.Presenters;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IAuthContract _authContract;
    
    public UsersController(
        IConfiguration configuration, 
        IAuthContract authContract)
    {
        _configuration = configuration;
        _authContract = authContract;
    }

    [HttpGet("{userId:Guid}")]
    public async Task<IActionResult> GetUserById(
        [FromServices] IQueryHandler<UserResponse, GetUserQuery> queryHandler,
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var cookiesOptions = _configuration.GetSection(CookiesOptions.SECTION_NAME).Get<CookiesOptions>();
        if (cookiesOptions == null)
        {
            throw new ConfigurationFailureException("Failed to get cookies options.");
        }
        
        var query = new GetUserQuery(userId);
        var result = await queryHandler.Handle(query, cancellationToken);
        if (result.User == null)
        {
            return StatusCode(500);
        }
        return Ok(result);
    }   
    
    [HttpPost("{userId:Guid}")]
    public async Task<IActionResult> CreateUser(
        [FromServices] ICommandHandler<Guid, CreateUserCommand> commandHandler,
        [FromRoute] Guid userId,
        [FromBody] CreateUserDto dto,
        CancellationToken cancellationToken)
    {
        var cookiesOptions = _configuration.GetSection(CookiesOptions.SECTION_NAME).Get<CookiesOptions>();
        if (cookiesOptions == null)
        {
            throw new ConfigurationFailureException("Failed to get cookies options.");
        }
        
        var command = new CreateUserCommand(userId, dto);
        var result = await commandHandler.Handle(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }   
    
    [HttpGet("auth/employee_token/{userId:Guid}")]
    public async Task<IActionResult> GetValidEmployeeTokenByUserId(
        [FromServices] IQueryHandler<EmployeeTokenResponse, GetValidEmployeeTokenByUserIdQuery> queryHandler,
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var cookiesOptions = _configuration.GetSection(CookiesOptions.SECTION_NAME).Get<CookiesOptions>();
        if (cookiesOptions == null)
        {
            throw new ConfigurationFailureException("Failed to get cookies options.");
        }
        
        var query = new GetValidEmployeeTokenByUserIdQuery(userId);
        var result = await queryHandler.Handle(query, cancellationToken);
        if (result.EmployeeToken == null)
        {
            return StatusCode(500);
        }
        return Ok(result);
    }   
    
    [HttpGet("auth")]
    public IActionResult Auth()
    {
        string url = _authContract.GetAuthorizationUrl();
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
    [Authorize]
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
    
    [HttpPost("auth/logout")]
    [Authorize]
    public async Task<IActionResult> Logout(
        [FromServices] ICommandHandler<LogoutCommand> commandHandler,
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
        
        var query = new LogoutCommand(userId);
        var result = await commandHandler.Handle(query, cancellationToken);
        
        context.Response.Cookies.Delete(cookiesOptions.JwtName);
        context.Response.Cookies.Delete(cookiesOptions.UserName);
        
        return result.IsFailure ? result.Error.ToResponse() : Ok();
    }
}