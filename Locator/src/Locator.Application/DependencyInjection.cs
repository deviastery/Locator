using FluentValidation;
using Locator.Application.Abstractions;
using Locator.Application.Ratings;
using Locator.Application.Ratings.GetRatingByVacancyIdQuery;
using Locator.Application.Ratings.UpdateVacancyRatingCommand;
using Locator.Application.Users;
using Locator.Application.Users.AuthQuery;
using Locator.Application.Users.Extensions;
using Locator.Application.Users.JwtTokens;
using Locator.Application.Vacancies;
using Locator.Application.Vacancies.CreateReviewCommand;
using Locator.Application.Vacancies.GetReviewsByVacancyIdQuery;
using Locator.Application.Vacancies.GetVacanciesWithFiltersQuery;
using Locator.Application.Vacancies.GetVacancyByIdQuery;
using Locator.Application.Vacancies.PrepareToUpdateVacancyRatingCommand;
using Locator.Contracts.Ratings;
using Locator.Contracts.Users;
using Locator.Contracts.Vacancies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Locator.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<ICommandHandler<PrepareToUpdateVacancyRatingCommand>, PrepareToUpdateVacancyRatingCommandHandler>();
        services.AddScoped<ICommandHandler<Guid, CreateReviewCommand>, CreateReviewCommandHandler>();
        services.AddScoped<ICommandHandler<Guid, UpdateVacancyRatingCommand>, UpdateVacancyRatingCommandHandler>();
        
        services.AddScoped<IQueryHandler<VacanciesResponse, GetVacanciesWithFiltersQuery>, GetVacanciesWithFilters>();
        services.AddScoped<IQueryHandler<VacancyResponse, GetVacancyByIdQuery>, GetVacancyById>();
        services.AddScoped<IQueryHandler<ReviewsByVacancyIdResponse, GetReviewsByVacancyIdQuery>, GetReviewsByVacancyId>();
        
        services.AddScoped<IQueryHandler<RatingByVacancyIdResponse, GetRatingByVacancyIdQuery>, GetRatingByVacancyId>();
        
        services.AddScoped<IQueryHandler<AuthResponse, AuthQuery>, Auth>();
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddAuth(configuration);
        
        return services;
    }
}