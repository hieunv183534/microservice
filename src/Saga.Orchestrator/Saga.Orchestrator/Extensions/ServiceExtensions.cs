using Common.Logging;
using Contracts.Sagas.OrderManager;
using Infrastructure.Extensions;
using Infrastructure.Policies;
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Saga.Orchestrator.Application.IntegrationEvents.EventsHanler;
using Saga.Orchestrator.HttpRepository;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Saga.Orchestrator.OrderManager;
using Saga.Orchestrator.Services;
using Saga.Orchestrator.Services.Interfaces;
using Shared.Configurations;
using Shared.DTOs.Basket;

namespace Saga.Orchestrator.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services) =>
        services.AddTransient<ICheckoutSagaService, CheckoutSagaService>()
            .AddTransient<ISagaOrderManager<BasketCheckoutDto, OrderResponse>, SagaOrderManager>()
            .AddTransient<LoggingDelegatingHandler>()
        ;

    public static IServiceCollection ConfigureHttpRepository(this IServiceCollection services) =>
        services.AddScoped<IOrderHttpRepository, OrderHttpRepository>()
            .AddScoped<IBasketHttpRepository, BasketHttpRepository>()
            .AddScoped<IInventoryHttpRepository, InventoryHttpRepository>()
        ;

    public static void ConfigureHttpClients(this IServiceCollection services)
    {
        ConfigureOrderHttpClient(services);
        ConfigureBasketHttpClient(services);
        ConfigureInventoryHttpClient(services);
    }

    private static void ConfigureOrderHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient<IOrderHttpRepository, OrderHttpRepository>("OrdersAPI", (sp, cl) =>
            {
                cl.BaseAddress = new Uri("http://localhost:5005/api/v1/");
            }).AddHttpMessageHandler<LoggingDelegatingHandler>()
            .UseExponentialHttpRetryPolicy();
        services.AddScoped(sp => sp.GetService<IHttpClientFactory>()
            .CreateClient("OrdersAPI"));
    }
    
    private static void ConfigureBasketHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient<IBasketHttpRepository, BasketHttpRepository>("BasketsAPI", (sp, cl) =>
            {
                cl.BaseAddress = new Uri("http://localhost:5004/api/");
            }).AddHttpMessageHandler<LoggingDelegatingHandler>()
            .UseImmediateHttpRetryPolicy();
        services.AddScoped(sp => sp.GetService<IHttpClientFactory>()
            .CreateClient("BasketsAPI"));
    }
    
    private static void ConfigureInventoryHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient<IInventoryHttpRepository, InventoryHttpRepository>("InventoryAPI", (sp, cl) =>
        {
            cl.BaseAddress = new Uri("http://localhost:5006/api/");
        }).AddHttpMessageHandler<LoggingDelegatingHandler>()
            .UseExponentialHttpRetryPolicy();
        services.AddScoped(sp => sp.GetService<IHttpClientFactory>()
            .CreateClient("InventoryAPI"));
    }

    public static void ConfigureMassTransit(this IServiceCollection services)
    {
        var settings = services.GetOptions<EventBusSettings>("EventBusSettings");
        if (settings == null || string.IsNullOrEmpty(settings.HostAddress))
            throw new ArgumentNullException("EventBusSetting is not configured");

        var mqConnection = new Uri(settings.HostAddress);
        services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
        services.AddMassTransit(config =>
        {
            config.AddConsumersFromNamespaceContaining<SagaBasketCheckoutEventHandler>();
            config.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(mqConnection);
                cfg.ConfigureEndpoints(ctx);
            });
        });
    }
}