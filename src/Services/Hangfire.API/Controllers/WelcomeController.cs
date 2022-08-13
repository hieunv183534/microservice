using Contracts.ScheduledJobs;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace Hangfire.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WelcomeController : ControllerBase
{
    private readonly IScheduledJobService _jobService;
    private readonly ILogger _logger;
    
    public WelcomeController(IScheduledJobService jobService, ILogger logger)
    {
        _jobService = jobService;
        _logger = logger;
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult Welcome()
    {
        var jobId = _jobService.Enqueue(() => ResponseWelcome("Welcome to Hangfire"));
        return Ok($"Job ID: {jobId} - Enqueue Job");
    }
    
    [HttpPost]
    [Route("delayed-welcome")]
    public IActionResult DelayedWelcome()
    {
        var seconds = 5;
        var jobId = _jobService.Schedule(() => ResponseWelcome($"Welcome to Hangfire - Delayed {seconds} seconds"), 
            TimeSpan.FromSeconds(seconds));
        return Ok($"Job ID: {jobId} - Delayed (Scheduled) Job");
    }
    
    [HttpPost]
    [Route("welcome-at")]
    public IActionResult WelcomeAt()
    {
        var enqueueAt = DateTimeOffset.UtcNow.AddSeconds(10);
        var jobId = _jobService.Schedule(() => ResponseWelcome($"Welcome to Hangfire - At: {enqueueAt}"), 
            enqueueAt);
        return Ok($"Job ID: {jobId} - enqueueAt {enqueueAt}");
    }
    
    [HttpPost]
    [Route("confirmed-welcome")]
    public IActionResult ConfirmedWelcome()
    {
        const int timeInSeconds = 10;
        var parentJobId = _jobService.Schedule(() => ResponseWelcome("Welcome to Hangfire"),
            TimeSpan.FromSeconds(timeInSeconds));

        var jobId = _jobService.ContinueQueueWith(parentJobId, () => ResponseWelcome("Welcome message is sent"));
        
        return Ok($"Job ID: {jobId} - Confirmed Welcome will be sent in {timeInSeconds} seconds");
    }
    
    [NonAction]    
    public void ResponseWelcome(string text)
    {
        _logger.Information(text);
    }
}