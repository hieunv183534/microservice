namespace Shared.Services;

public class BackgroundJobHttpService
{
    private const string BASE_URL = "http://localhost:5008";
    
    public BackgroundJobHttpService(
        HttpClient client)
    {
        client.BaseAddress = new Uri(BASE_URL);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        Client = client;
    }
    
    public HttpClient Client { get; }
}