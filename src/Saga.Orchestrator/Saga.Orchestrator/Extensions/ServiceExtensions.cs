using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Saga.Orchestrator.HttpRepository;
using Saga.Orchestrator.HttpRepository.Interfaces;

namespace Saga.Orchestrator.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services) =>
        services.AddTransient<ISerializeService, SerializeService>();

    public static IServiceCollection ConfigureHttpRepository(this IServiceCollection services) =>
        services.AddScoped<IOrderHttpRepository, OrderHttpRepository>();

    public static void ConfigureOrderHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient("OrdersAPI",
            (sp, cl) =>
            {
                cl.BaseAddress = new Uri("http://localhost:5005/api/v1/orders");
            });

        services.AddScoped(sp => sp.GetService<IHttpClientFactory>()
            .CreateClient("OrdersAPI"));
    }
}