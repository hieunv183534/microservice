using HealthChecks.UI.Client;
using Inventory.Grpc.Extensions;
using Inventory.Grpc.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
builder.Host.AddAppConfigurations();

Log.Information($"Start {builder.Environment.ApplicationName} up");

try
{
    builder.Host.AddAppConfigurations();
    // Add services to the container.
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.ConfigureMongoDbClient();
    builder.Services.AddInfrastructureServices();
    builder.Services.ConfigureHealthChecks();
    // Additional configuration is required to successfully run gRPC on macOS.
    // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
    builder.Services.AddGrpc();
    builder.Services.AddGrpcReflection();

    // builder.WebHost.ConfigureKestrel(options =>
    // {
    //     // Setup a HTTP/2 endpoint without TLS.
    //     options.ListenLocalhost(5007, o => o.Protocols =
    //         HttpProtocols.Http2);
    // });

    builder.WebHost.ConfigureKestrel(options =>
    {
        if (builder.Environment.IsDevelopment())
        {
            options.ListenAnyIP(5007);
            options.ListenAnyIP(5107, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
            });
        }
        if (builder.Environment.IsProduction())
        {
            options.ListenAnyIP(8080);
            options.ListenAnyIP(8585, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
            });
        }
    });

    var app = builder.Build();
    app.UseRouting();
    // app.UseHttpsRedirection();
   
    app.UseEndpoints(endpoints =>
    {
        // health checks
        endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        endpoints.MapGrpcHealthChecksService();
        endpoints.MapGrpcService<InventoryService>();
        endpoints.MapGrpcReflectionService();

        endpoints.MapGet("/", async context =>
        {
            await context.Response.WriteAsync("Inventory.GRPC - Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
        });
    });

    app.Run();
}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;

    Log.Fatal(ex, $"Unhandled exception: {ex.Message}");
}
finally
{
    Log.Information($"Shut down {builder.Environment.ApplicationName} complete");
    Log.CloseAndFlush();
}