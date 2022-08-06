using System.Text;
using Ocelot.DependencyInjection;
using Shared.Configurations;
using Infrastructure.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace OcelotApiGw.Extensions;

public static class ServiceExtensions
{
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
    }
}