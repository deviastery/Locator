namespace ApiGateway.Web.Middlewares;

public class AttachSignatureToRequest(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        httpContext.Request.Headers["Api-Gateway"] = "Signed";
        await next(httpContext);
    }
}