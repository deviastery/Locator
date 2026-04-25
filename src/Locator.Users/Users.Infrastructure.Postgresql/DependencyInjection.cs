using FluentValidation;
using Framework.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Application;

namespace Users.Infrastructure.Postgresql;

public static class DependencyInjection
{
    public static IServiceCollection AddPostgresInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddCoreServices<UsersDbContext>(configuration, "UsersDb");

        services.AddScoped<IUsersRepository, UsersEfCoreRepository>();
        
        services.AddScoped<IUsersReadDbContext>(sp => sp.GetRequiredService<UsersDbContext>());
        return services;
    }

    public static IApplicationBuilder UseInfrastructurePolicy(
        this IApplicationBuilder app)
    {
        app.UseCorePolicies();
        return app;
    }
}