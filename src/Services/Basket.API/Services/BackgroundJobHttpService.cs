using Shared.Configurations;

namespace Basket.API.Services;

public class BackgroundJobHttpService
{
    public HttpClient Client { get; }

    public BackgroundJobHttpService(
        HttpClient client,
        BackgroundJobSettings settings)
    {
        client.BaseAddress = new Uri(settings.HangfireUrl);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        Client = client;
    }
}