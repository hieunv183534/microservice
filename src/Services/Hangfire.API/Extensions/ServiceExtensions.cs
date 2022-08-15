using Contracts.ScheduledJobs;
using Contracts.Services;
using Hangfire.API.Services;
using Hangfire.API.Services.Interfaces;
using Infrastructure.Configurations;
using Infrastructure.ScheduledJobs;
using Infrastructure.Services;
using Shared.Configurations;

namespace Hangfire.API.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services, 
        IConfiguration configuration)
    {
        var hangFireSettings = configuration.GetSection(nameof(HangFireSettings))
            .Get<HangFireSettings>();
        services.AddSingleton(hangFireSettings);
        
        var emailSettings = configuration.GetSection(nameof(SMTPEmailSetting))
            .Get<SMTPEmailSetting>();
        services.AddSingleton(emailSettings);

        return services;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services)
        => services.AddTransient<IScheduledJobService, HangfireService>()
            .AddScoped<ISmtpEmailService, SmtpEmailService>()
            .AddScoped<IBackgroundJobService, BackgroundJobService>()
        ;
}