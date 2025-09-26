using FluentValidation;
using Locator.Application.Abstractions;
using Locator.Application.Ratings;
using Locator.Application.Ratings.UpdateVacancyRating;
using Locator.Application.Vacancies;
using Locator.Application.Vacancies.CreateReview;
using Locator.Application.Vacancies.PrepareToUpdateVacancyRating;
using Microsoft.Extensions.DependencyInjection;

namespace Locator.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<IHandler<PrepareToUpdateVacancyRatingCommand>, PrepareToUpdateVacancyRatingHandler>();
        services.AddScoped<IHandler<Guid, CreateReviewCommand>, CreateReviewHandler>();
        services.AddScoped<IHandler<Guid, UpdateVacancyRatingCommand>, UpdateVacancyRatingHandler>();

        return services;
    }
}