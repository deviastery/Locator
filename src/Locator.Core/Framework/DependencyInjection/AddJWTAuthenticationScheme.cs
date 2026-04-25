using System.Text;
using System.Text.Json;
using Framework.Fails;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Fails.Exceptions;
using Shared.Options;

namespace Framework.DependencyInjection;

public static class JWTAuthenticationScheme
{
    public static IServiceCollection AddJWTAuthenticationScheme(
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
                o.RequireHttpsMetadata = false;
                o.SaveToken = true;
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