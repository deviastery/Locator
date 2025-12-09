using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Abstractions;
using Shared.Options;
using Users.Application.AuthQuery;
using Users.Application.CreateUserCommand;
using Users.Application.GetEmployeeTokenByUserIdQuery;
using Users.Application.GetUserQuery;
using Users.Application.GetValidEmployeeTokenByUserIdQuery;
using Users.Application.JwtTokens;
using Users.Application.RefreshTokenCommand;
using Users.Application.UpdateEmployeeTokenCommand;
using Users.Contracts.Responses;

namespace Users.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        
        services.AddScoped<IQueryHandler<AuthResponse, AuthQuery.AuthQuery>, Auth>();
        services.AddScoped<IQueryHandler<UserResponse, GetUserQuery.GetUserQuery>, GetUser>();
        services.AddScoped<IQueryHandler<EmployeeTokenResponse, GetEmployeeTokenByUserIdQuery.GetEmployeeTokenByUserIdQuery>, 
            GetEmployeeTokenByUserId>();
        services.AddScoped<IQueryHandler<EmployeeTokenResponse, GetValidEmployeeTokenByUserIdQuery.GetValidEmployeeTokenByUserIdQuery>, 
            GetValidEmployeeTokenByUserId>();
        
        services.AddScoped<ICommandHandler<string, RefreshTokenCommand.RefreshTokenCommand>, RefreshTokenCommandHandler>();
        services.AddScoped<ICommandHandler<Guid, UpdateEmployeeTokenCommand.UpdateEmployeeTokenCommand>, 
            UpdateEmployeeTokenCommandHandler>();        
        services.AddScoped<ICommandHandler<Guid, CreateUserCommand.CreateUserCommand>, 
            CreateUserCommandHandler>();
        
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SECTION_NAME));
        services.AddScoped<IJwtProvider, JwtProvider>();
        
        return services;
    }
}