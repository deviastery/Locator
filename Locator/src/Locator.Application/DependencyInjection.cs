using FluentValidation;
using Locator.Application.Abstractions;
using Locator.Application.Ratings;
using Locator.Application.Ratings.UpdateVacancyRatingCommand;
using Locator.Application.Vacancies;
using Locator.Application.Vacancies.CreateReviewCommand;
using Locator.Application.Vacancies.GetVacanciesWithFiltersQuery;
using Locator.Application.Vacancies.PrepareToUpdateVacancyRatingCommand;
using Locator.Contracts.Vacancies;
using Microsoft.Extensions.DependencyInjection;

namespace Locator.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<ICommandHandler<PrepareToUpdateVacancyRatingCommand>, PrepareToUpdateVacancyRatingCommandHandler>();
        services.AddScoped<ICommandHandler<Guid, CreateReviewCommand>, CreateReviewCommandHandler>();
        services.AddScoped<ICommandHandler<Guid, UpdateVacancyRatingCommand>, UpdateVacancyRatingCommandHandler>();
        
        services.AddScoped<IQueryHandler<VacancyResponse, GetVacanciesWithFiltersQuery>, GetVacanciesWithFilters>();
        
        return services;
    }
}