using System.Text;
using Contracts.Identity;
using Ocelot.DependencyInjection;
using Shared.Configurations;
using Infrastructure.Extensions;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace OcelotApiGw.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services, 
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(nameof(JwtSettings))
            .Get<JwtSettings>();
        services.AddSingleton(jwtSettings);

        return services;
    }
    
    public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration["AllowOrigins"];
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder.WithOrigins(origins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
            );
        });
    }

    public static void ConfigureOcelot(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOcelot(configuration);
        services.AddTransient<ITokenService, TokenService>();
        services.AddJwtAuthentication();
    }
    
    internal static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        var settings = services.GetOptions<JwtSettings>(nameof(JwtSettings));
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));

        var tokenValidationParameters = new TokenValidationParameters  
        {  
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateIssuer = false,  
            ValidateAudience = false,  
            ValidateLifetime = false,  
            ClockSkew = TimeSpan.Zero,  
            RequireExpirationTime = true,  
        };
        
        services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme =  JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme =  JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.RequireHttpsMetadata = false;
                x.TokenValidationParameters = tokenValidationParameters;
            });

        return services;
    }
}