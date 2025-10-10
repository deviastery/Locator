using Locator.Application.Users;
using Locator.Infrastructure.HhApi.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Locator.Infrastructure.HhApi;

public static class DependencyInjection
{
    public static IServiceCollection AddHhApiInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IAuthService, HhAuthService>();
        services.Configure<HhApiConfiguration>(configuration
            .GetSection(HhApiConfiguration.SectionName));
        
        return services;
    }
}