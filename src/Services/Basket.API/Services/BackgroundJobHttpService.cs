using Infrastructure.Extensions;
using Shared.Configurations;
using Shared.DTOs.ScheduledJob;

namespace Basket.API.Services;

public class BackgroundJobHttpService
{
    private readonly HttpClient _client;

    private readonly string _scheduledJobUrl;

    public BackgroundJobHttpService(
        HttpClient client,
        BackgroundJobSettings settings)
    {
        if (settings == null ||
            string.IsNullOrEmpty(settings.HangfireUrl) ||
            string.IsNullOrEmpty(settings.ScheduledJobUrl)
           )
            throw new ArgumentNullException($"{nameof(BackgroundJobSettings)} is not configured properly");

        client.BaseAddress = new Uri(settings.HangfireUrl);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        _client = client;

        _scheduledJobUrl = settings.ScheduledJobUrl;
    }

    public async Task<string> SendEmailReminderCheckout(ReminderCheckoutOrderDto model)
    {
        var uri = $"{_scheduledJobUrl}/send-email-reminder-checkout-order";
        var response = await _client.PostAsJson(uri, model);

        string jobId = null;
        if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            jobId = await response.ReadContentAs<string>();

        return jobId;
    }
    
    public void DeleteReminderCheckoutOrder(string jobId) 
    {
        var uri = $"{_scheduledJobUrl}/delete/jobId/{jobId}";
        _client.DeleteAsync(uri);
    }
}