using Basket.API;
using Basket.API.Extensions;
using HealthChecks.UI.Client;
using Infrastructure.Middlewares;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Information($"Start {builder.Environment.ApplicationName} up");

try
{
    builder.Host.AddAppConfigurations();
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));
    // Add services to the container.
    builder.Services.ConfigureServices();
    builder.Services.ConfigureHttpClientService();
    builder.Services.ConfigureRedis();
    builder.Services.ConfigureGrpcService();
    builder.Services.Configure<RouteOptions>(options 
        => options.LowercaseUrls = true);
    
    // configure Mass Transit
    builder.Services.ConfigureMassTransit();

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.ConfigureHealthChecks();

    var app = builder.Build();
    
    // Configure the HTTP request pipeline.
    //if (app.Environment.IsDevelopment())
    //{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json",
        $"{builder.Environment.ApplicationName} v1"));
    //}

    app.UseMiddleware<ErrorWrappingMiddleware>();
    // app.UseHttpsRedirection();

    app.UseAuthorization();
    app.UseRouting();
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