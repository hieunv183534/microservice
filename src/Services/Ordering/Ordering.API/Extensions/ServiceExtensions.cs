using EventBus.MessageComponents.Consumers.Basket;
using Infrastructure.Configurations;
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ordering.API.EventBusConsumer;
using Shared.Configurations;

namespace Ordering.API.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services, 
        IConfiguration configuration)
    {
        var emailSettings = configuration.GetSection(nameof(SMTPEmailSetting))
            .Get<SMTPEmailSetting>();
        services.AddSingleton(emailSettings);
        
        var eventBusSettings = configuration.GetSection(nameof(EventBusSettings))
            .Get<EventBusSettings>();
        services.AddSingleton(eventBusSettings);

        return services;
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
            config.AddConsumersFromNamespaceContaining<BasketCheckoutConsumer>();
            
            config.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(mqConnection);
                
                // cfg.ReceiveEndpoint("basket-checkout-queue", c =>
                // {
                //     c.ConfigureConsumer<BasketCheckoutConsumer>(ctx);
                // });
                
                // Auto configure the endpoints for all defined consumer, saga....
                cfg.ConfigureEndpoints(ctx);
            });
        });

        services.AddScoped<IBasketCheckoutConsumer, BasketCheckoutConsumer>();
    }
}