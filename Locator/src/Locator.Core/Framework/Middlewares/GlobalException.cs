using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Fails.Exceptions;

namespace Framework.Middlewares;

public class GlobalException
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalException> _logger;

    public GlobalException(RequestDelegate next, ILogger<GlobalException> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            if (ex is TaskCanceledException or OperationCanceledException)
            {
                return;
            }
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, exception.Message);

        (int code, IEnumerable<Error>? errors) = exception switch
        {
            BadRequestException =>
                (StatusCodes.Status400BadRequest,
                    JsonSerializer.Deserialize<IEnumerable<Error>>(exception.Message)),
            
            UnauthorizedException =>
                (StatusCodes.Status401Unauthorized,
                    JsonSerializer.Deserialize<IEnumerable<Error>>(exception.Message)),

            NotFoundException =>
                (StatusCodes.Status404NotFound,
                    JsonSerializer.Deserialize<IEnumerable<Error>>(exception.Message)),

            _ => (StatusCodes.Status500InternalServerError,
                [Error.Failure("Something went wrong")]),
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = code;

        await context.Response.WriteAsJsonAsync(errors);
    }
}

public static class ExceptionMiddlewareExtension
{
    public static IApplicationBuilder UseExceptionMiddleware(this WebApplication app) =>
        app.UseMiddleware<GlobalException>();
}