using HealthChecks.UI.Client;
using Infrastructure.Middlewares;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Ordering.API.Extensions;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Information($"Start {builder.Environment.ApplicationName} up");

try
{
    // Add services to the container.
    builder.Host.AddAppConfigurations();
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices();
    builder.Services.ConfigureMassTransit();
    builder.Services.ConfigureHealthChecks();

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    builder.Services.AddSwaggerGen();
    
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json",
            $"{builder.Environment.ApplicationName} v1"));
    }

    // Initialise and seed database
    using (var scope = app.Services.CreateScope())
    {
        var orderContextSeed = scope.ServiceProvider.GetRequiredService<OrderContextSeed>();
        await orderContextSeed.InitialiseAsync();
        await orderContextSeed.SeedAsync();
    }

    app.UseMiddleware<ErrorWrappingMiddleware>();
    
    // app.UseHttpsRedirection(); //production only
    app.UseRouting();

    app.UseAuthorization();

    app.MapControllers();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        endpoints.MapDefaultControllerRoute();
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