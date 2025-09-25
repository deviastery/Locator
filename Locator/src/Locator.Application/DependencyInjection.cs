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

        services.AddScoped<ICommandHandler<PrepareToUpdateVacancyRatingCommand>, PrepareToUpdateVacancyRatingHandler>();
        services.AddScoped<ICommandHandler<Guid, CreateReviewCommand>, CreateReviewHandler>();
        services.AddScoped<ICommandHandler<Guid, UpdateVacancyRatingCommand>, UpdateVacancyRatingHandler>();

        return services;
    }
}