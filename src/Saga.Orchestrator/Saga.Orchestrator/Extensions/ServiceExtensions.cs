using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Saga.Orchestrator.HttpRepository;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Saga.Orchestrator.Services;
using Saga.Orchestrator.Services.Interfaces;

namespace Saga.Orchestrator.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services) =>
        services.AddTransient<ISerializeService, SerializeService>()
            .AddTransient<ICheckoutSagaService, CheckoutSagaService>()
        ;

    public static IServiceCollection ConfigureHttpRepository(this IServiceCollection services) =>
        services.AddScoped<IOrderHttpRepository, OrderHttpRepository>()
            .AddScoped<IBasketHttpRepository, BasketHttpRepository>()
        ;

    public static void ConfigureHttpClients(this IServiceCollection services)
    {
        ConfigureOrderHttpClient(services);
        ConfigureBasketHttpClient(services);
    }

    private static void ConfigureOrderHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient<IOrderHttpRepository, OrderHttpRepository>("OrdersAPI",(sp, cl) =>
        {
            cl.BaseAddress = new Uri("http://localhost:5005/api/v1/orders");
        });
        services.AddScoped(sp => sp.GetService<IHttpClientFactory>()
            .CreateClient("OrdersAPI"));
    }
    
    private static void ConfigureBasketHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient<IBasketHttpRepository, BasketHttpRepository>("BasketsAPI",(sp, cl) =>
        {
            cl.BaseAddress = new Uri("http://localhost:5004/api/baskets/");
        });
        services.AddScoped(sp => sp.GetService<IHttpClientFactory>()
            .CreateClient("BasketsAPI"));
    }
}