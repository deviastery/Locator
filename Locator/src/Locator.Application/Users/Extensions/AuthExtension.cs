using System.Text;
using System.Text.Json;
using Locator.Application.Users.Fails;
using Locator.Application.Users.JwtTokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Fails.Exceptions;
using Shared.Options;

namespace Locator.Application.Users.Extensions;

public static class AuthExtension
{
    public static IServiceCollection AddAuth(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(JwtOptions.SECTION_NAME).Get<JwtOptions>();
        if (jwtOptions == null)
        {
            throw new ConfigurationFailureException("Failed to get jwt options.");
        }
        var cookiesOptions = configuration.GetSection(CookiesOptions.SECTION_NAME).Get<CookiesOptions>();
        if (cookiesOptions == null)
        {
            throw new ConfigurationFailureException("Failed to get cookies options.");
        }
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                    ClockSkew = TimeSpan.Zero,
                };
                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies[cookiesOptions.JwtName];
                        
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse(); 

                        var result = Errors.General.Unauthorized();

                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json; charset=utf-8";
                        return context.Response.WriteAsync(JsonSerializer.Serialize(result));
                    },
                };
            });
        services.AddAuthorization();
        return services;
    }
}