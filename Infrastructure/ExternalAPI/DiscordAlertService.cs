using System.Text;
using System.Text.Json;

public class DiscordAlertService
{
    private readonly HttpClient _httpClient;
    private readonly string? _webhookUrl;

    public DiscordAlertService(IConfiguration _configuration)
    {
        _httpClient = new HttpClient();
        _webhookUrl = _configuration.GetValue<string>("DiscordWebhookUrl");
    }

    public async Task SendAlert(string message)
    {
        var tempPayload = new { content = message };
        var payload =  JsonSerializer.Serialize(tempPayload);
        
        var httpContent = new StringContent(payload,Encoding.UTF8, "application/json");

        try
        {
            await _httpClient.PostAsync(_webhookUrl, httpContent);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Discord message exception: " + ex.Message);
        }
    }
}