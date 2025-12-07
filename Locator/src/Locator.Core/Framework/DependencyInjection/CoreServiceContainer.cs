using Framework.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.DependencyInjection;

public static class CoreServiceContainer
{
    public static IServiceCollection AddCoreServices<TContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        string dbName)
        where TContext : DbContext
    {
        services.AddDbContext<TContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString(dbName)));
        services.AddJWTAuthenticationScheme(configuration);
        
        return services;
    }
    
    public static IApplicationBuilder UseCorePolicies(
        this IApplicationBuilder app)
    {
        app.UseMiddleware<GlobalException>();
        // app.UseMiddleware<LintenToOnlyApiGateway>();

        return app;
    }
}
