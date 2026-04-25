using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ratings.Application;
using Ratings.Contracts;
using Ratings.Infrastructure.Postgresql;

namespace Ratings.Presenters;

public static class DependencyInjection
{
    public static IServiceCollection AddRatings(
        this IServiceCollection services,
        IConfiguration configuration) =>

        services.AddApplication(configuration)
            .AddPostgresInfrastructure(configuration);
}