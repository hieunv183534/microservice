using Contracts.ScheduledJobs;
using Contracts.Services;
using Hangfire.API.Services.Interfaces;
using Shared.Services.Email;
using ILogger = Serilog.ILogger;

namespace Hangfire.API.Services;

public class BackgroundJobService : IBackgroundJobService
{
    private readonly IScheduledJobService _jobService;
    private readonly ISmtpEmailService _emailService;
    private readonly ILogger _logger;
    
    public BackgroundJobService(IScheduledJobService jobService, ISmtpEmailService emailService, ILogger logger)
    {
        _jobService = jobService;
        _emailService = emailService;
        _logger = logger;
    }

    public IScheduledJobService ScheduledJobService => _jobService;
    
    public string SendEmailContent(string email, string subject, string emailContent, DateTimeOffset enqueueAt)
    {
        var emailRequest = new MailRequest
        {
            ToAddress = email,
            Body = emailContent,
            Subject = subject
        };

        try
        {
            var jobId = _jobService.Schedule(() => _emailService.SendEmail(emailRequest), enqueueAt);
            _logger.Information($"Sent email to {email} with subject: {subject} - Job Id: {jobId}");

            return jobId;
        }
        catch (Exception ex)
        {
            _logger.Error($"failed due to an error with the email service: {ex.Message}");
        }

        return null;
    }
}