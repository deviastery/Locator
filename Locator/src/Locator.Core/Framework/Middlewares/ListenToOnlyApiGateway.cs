using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Framework.Middlewares;

public class ListenToOnlyApiGateway
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalException> _logger;

    public ListenToOnlyApiGateway(RequestDelegate next, ILogger<GlobalException> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var signedHeader = httpContext.Request.Headers["Api-Gateway"];

        // If request is not coming from Api Gateway
        if (signedHeader.FirstOrDefault() is null)
        {
            httpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await httpContext.Response.WriteAsync("Service is unavailable");
        }
        else
        {
            await _next(httpContext); 
        }
    }
}

public static class ListenToOnlyApiGatewayMiddlewareExtension
{
    public static IApplicationBuilder UseListenToOnlyApiGatewayMiddleware(this WebApplication app) =>
        app.UseMiddleware<ListenToOnlyApiGateway>();
}