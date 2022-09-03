using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Basket.API.Repositories.Interfaces;
using Basket.API.Services;
using Basket.API.Services.Interfaces;
using Common.Logging;
using Contracts.Common.Interfaces;
using EventBus.Messages.IntegrationEvents.Events;
using Infrastructure.Common;
using Infrastructure.Extensions;
using Infrastructure.Policies;
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Configurations;
using Inventory.Grpc.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Basket.API.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services, 
        IConfiguration configuration)
    {
        var eventBusSettings = configuration.GetSection(nameof(EventBusSettings))
            .Get<EventBusSettings>();
        services.AddSingleton(eventBusSettings);
        
        var cacheSettings = configuration.GetSection(nameof(CacheSettings))
            .Get<CacheSettings>();
        services.AddSingleton(cacheSettings);
        
        var grpcSettings = configuration.GetSection(nameof(GrpcSettings))
            .Get<GrpcSettings>();
        services.AddSingleton(grpcSettings);
        
        var backgroundJobSettings = configuration.GetSection(nameof(BackgroundJobSettings))
            .Get<BackgroundJobSettings>();
        services.AddSingleton(backgroundJobSettings);

        return services;
    }
    
    public static IServiceCollection ConfigureServices(this IServiceCollection services) =>
        services.AddScoped<IBasketRepository, BasketRepository>()
            .AddTransient<ISerializeService, SerializeService>()
            .AddTransient<IEmailTemplateService, BasketEmailTemplateService>()
            .AddTransient<LoggingDelegatingHandler>()
        ;
    
    public static void ConfigureHttpClientService(this IServiceCollection services)
    {
        services.AddHttpClient<BackgroundJobHttpService>()
            .AddHttpMessageHandler<LoggingDelegatingHandler>()
            .UseImmediateHttpRetryPolicy()
            .UseCircuitBreakerPolicy()
            .ConfigureTimeoutPolicy()
            ;
    }

    public static void ConfigureGrpcService(this IServiceCollection services)
    {
        var settings = services.GetOptions<GrpcSettings>(nameof(GrpcSettings));
        services.AddGrpcClient<StockProtoService.StockProtoServiceClient>(x =>
            x.Address = new Uri(settings.StockUrl));
            // .UseImmediateHttpRetryPolicy(); //not work yet
        services.AddScoped<StockItemGrpcService>();
    }

    public static void ConfigureRedis(this IServiceCollection services)
    {
        var settings = services.GetOptions<CacheSettings>(nameof(CacheSettings));
        if (string.IsNullOrEmpty(settings.ConnectionString))
            throw new ArgumentNullException("Redis Connection string is not configured.");
        
        //Redis Configuration
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = settings.ConnectionString;
        });
    }

    public static void ConfigureMassTransit(this IServiceCollection services)
    {
        var settings = services.GetOptions<EventBusSettings>(nameof(EventBusSettings));
        if (settings == null || string.IsNullOrEmpty(settings.HostAddress) ||
            string.IsNullOrEmpty(settings.HostAddress)) throw new ArgumentNullException("EventBusSettings is not configured!");

        var mqConnection = new Uri(settings.HostAddress);
        
        services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
        services.AddMassTransit(config =>
        {
            config.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(mqConnection);
            });
            // Publish submit order message, instead of sending it to a specific queue directly.
            config.AddRequestClient<IBasketCheckoutEvent>();
        });
    }
    
    public static void ConfigureHealthChecks(this IServiceCollection services)
    {
        var cacheSettings = services.GetOptions<CacheSettings>(nameof(CacheSettings));
        services.AddHealthChecks()
            .AddRedis(cacheSettings.ConnectionString,
                name: "Redis Health",
                failureStatus: HealthStatus.Degraded);
    }
}