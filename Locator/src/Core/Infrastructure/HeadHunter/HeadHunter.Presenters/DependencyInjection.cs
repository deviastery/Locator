using HeadHunter.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Options;

namespace HeadHunter.Presenters;

public static class DependencyInjection
{
    public static IServiceCollection AddHeadHunterModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IAuthContract, HeadHunterAuthService>();
        services.Configure<HeadHunterOptions>(configuration
            .GetSection(HeadHunterOptions.SECTION_NAME));
        
        services.AddHttpClient<IVacanciesContract, HeadHunterVacanciesService>();
        
        return services;
    } 
}