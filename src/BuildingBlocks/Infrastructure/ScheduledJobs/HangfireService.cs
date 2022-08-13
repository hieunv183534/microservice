using System.Linq.Expressions;
using Contracts.ScheduledJobs;
using Hangfire;

namespace Infrastructure.ScheduledJobs;

public class HangfireService : IScheduledJobService
{
    public string Enqueue(Expression<Action> functionCall) 
        => BackgroundJob.Enqueue(functionCall);

    public string Enqueue<T>(Expression<Action<T>> functionCall) 
        => BackgroundJob.Enqueue<T>(functionCall);
   
    public string Schedule(Expression<Action> functionCall, TimeSpan delay) 
        => BackgroundJob.Schedule(functionCall, delay);

    public string Schedule(Expression<Action> functionCall, DateTimeOffset enqueueAt) 
        => BackgroundJob.Schedule(functionCall, enqueueAt);

    public string Schedule<T>(Expression<Action<T>> functionCall, TimeSpan delay) 
        => BackgroundJob.Schedule<T>(functionCall, delay);
    
    public bool Delete(string jobId) => BackgroundJob.Delete(jobId);
    
    public bool Requeue(string jobId) => BackgroundJob.Requeue(jobId);
}