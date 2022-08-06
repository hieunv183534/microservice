using Infrastructure.Middlewares;
using Ocelot.Middleware;
using OcelotApiGw.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

Log.Information($"Start {builder.Environment.ApplicationName} up");

try
{
    builder.Host.AddAppConfigurations();
    // Add services to the container.
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.ConfigureOcelot(builder.Configuration);
    builder.Services.ConfigureCors(builder.Configuration);

    var app = builder.Build();

    // app.MapGet("/", () => $"Welcome to {builder.Environment.ApplicationName}!");

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json",
                $"{builder.Environment.ApplicationName} v1"));
        });
    }

    app.UseCors("CorsPolicy");

    app.UseMiddleware<ErrorWrappingMiddleware>();

    // app.UseHttpsRedirection(); //production only
    app.UseAuthentication();
    app.UseRouting();
    app.UseAuthorization();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync($"Hello TEDU members! This is {builder.Environment.ApplicationName}");
            });
    });

    app.MapControllers();
    await app.UseOcelot();
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