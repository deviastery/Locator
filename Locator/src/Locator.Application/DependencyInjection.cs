using FluentValidation;
using Locator.Application.Vacancies;
using Microsoft.Extensions.DependencyInjection;

namespace Locator.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<IVacanciesService, VacanciesService>();

        return services;
    }
}