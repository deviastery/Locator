using Confluent.Kafka;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Abstractions;
using Shared.Options;
using Vacancies.Application.CreateRequestVacancyRatingCommand;
using Vacancies.Application.CreateReviewCommand;
using Vacancies.Application.GetNegotiationByVacancyIdQuery;
using Vacancies.Application.GetNegotiationsQuery;
using Vacancies.Application.GetRequestEmployeeTokenQuery;
using Vacancies.Application.GetRequestRatingByVacancyIdQuery;
using Vacancies.Application.GetRequestRatingsQuery;
using Vacancies.Application.GetRequestUserByIdQuery;
using Vacancies.Application.GetReviewsByVacancyIdQuery;
using Vacancies.Application.GetVacanciesWithFiltersQuery;
using Vacancies.Application.GetVacancyByIdQuery;
using Vacancies.Application.PrepareToUpdateVacancyRatingCommand;
using Vacancies.Contracts.Responses;

namespace Vacancies.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<ICommandHandler<PrepareToUpdateVacancyRatingCommand.PrepareToUpdateVacancyRatingCommand>, PrepareToUpdateVacancyRatingCommandHandler>();
        services.AddScoped<ICommandHandler<Guid, CreateReviewCommand.CreateReviewCommand>, CreateReviewCommandHandler>();
        services.AddScoped<ICommandHandler<Guid, CreateRequestVacancyRatingCommand.CreateRequestVacancyRatingCommand>, CreateRequestVacancyRatingCommandHandler>();

        services.AddScoped<IQueryHandler<VacanciesResponse, GetVacanciesWithFiltersQuery.GetVacanciesWithFiltersQuery>, GetVacanciesWithFilters>();
        services.AddScoped<IQueryHandler<VacancyResponse, GetVacancyByIdQuery.GetVacancyByIdQuery>, GetVacancyById>();
        services.AddScoped<IQueryHandler<ReviewsByVacancyIdResponse, GetReviewsByVacancyIdQuery.GetReviewsByVacancyIdQuery>, GetReviewsByVacancyId>();
        services.AddScoped<IQueryHandler<NegotiationsResponse, GetNegotiationsQuery.GetNegotiationsQuery>, GetNegotiations>();
        services.AddScoped<IQueryHandler<NegotiationResponse, GetNegotiationByVacancyIdQuery.GetNegotiationByVacancyIdQuery>, GetNegotiationByVacancyId>();
        services.AddScoped<IQueryHandler<EmployeeTokenResponse, GetRequestEmployeeTokenQuery.GetRequestEmployeeTokenQuery>, 
            GetRequestEmployeeToken>();
        services.AddScoped<IQueryHandler<UserResponse, GetRequestUserByIdQuery.GetRequestUserByIdQuery>, GetRequestUserById>();
        services.AddScoped<IQueryHandler<VacancyRatingsResponse, GetRequestRatingsQuery.GetRequestRatingsQuery>, GetRequestRatings>();
        services.AddScoped<IQueryHandler<RatingByVacancyIdResponse, GetRequestRatingByVacancyIdQuery.GetRequestRatingByVacancyIdQuery>, GetRequestRatingByVacancyId>();
        
        services.Configure<KafkaOptions>(configuration.GetSection(KafkaOptions.SECTION_NAME));
        var kafkaOptions = configuration.GetSection(KafkaOptions.SECTION_NAME).Get<KafkaOptions>();
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = kafkaOptions?.BootstrapServers ?? "localhost:9092",
        };

        services.AddSingleton<IProducer<Null, string>>(sp => 
            new ProducerBuilder<Null, string>(producerConfig).Build());
        
        return services;
    }
}