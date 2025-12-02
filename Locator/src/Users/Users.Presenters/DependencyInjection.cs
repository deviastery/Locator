using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Application;
using Users.Contracts;
using Users.Infrastructure.Postgresql;

namespace Users.Presenters;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersModule(
        this IServiceCollection services,
        IConfiguration configuration) =>
        
        services.AddApplication(configuration)
            .AddPostgresInfrastructure(configuration)
            .AddScoped<IUsersContract, UsersContract>();
}