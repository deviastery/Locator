using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ratings.Application.GetRatingByVacancyIdQuery;
using Ratings.Application.GetVacancyRatingsQuery;
using Ratings.Application.UpdateVacancyRatingCommand;
using Ratings.Contracts.Responses;
using Shared.Abstractions;

namespace Ratings.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<ICommandHandler<Guid, UpdateVacancyRatingCommand.UpdateVacancyRatingCommand>, UpdateVacancyRatingCommandHandler>();
        
        services.AddScoped<IQueryHandler<RatingByVacancyIdResponse, GetRatingByVacancyIdQuery.GetRatingByVacancyIdQuery>, GetRatingByVacancyId>();
        services.AddScoped<IQueryHandler<VacancyRatingsResponse, GetVacancyRatingsQuery.GetVacancyRatingsQuery>, GetVacancyRatings>();
        
        return services;
    }
}