using Microsoft.Extensions.Caching.Memory;
using Ping_Project.Infrastructure.Repository;
using Ping_Project.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace Ping_Project.Validation;
public class DataValidation(TokenValidaton _tokenValidator, IServiceScopeFactory _scopeFactory, DiscordAlertService _discordAlert, IMemoryCache _memoryCache)
{
    public async Task ValidData(string token, string timestamp, string payload)
    {
        if (long.TryParse(timestamp, out var timestampLong))
        {
            long serverTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            
            long timeDiff = Math.Abs(serverTime - timestampLong);

            if (timeDiff > 10)
            {
                Console.WriteLine("Package terminated");
                return;
            }
            
            Console.WriteLine($"Time diffrence: {timeDiff}");
            
            if(_memoryCache.TryGetValue(timestamp, out _))
            {
                return;
            }

            _memoryCache.Set(timestamp, true, TimeSpan.FromSeconds(10));
            
            if (_tokenValidator.IsValid(token))
            {
                Console.WriteLine("Token is valid");
                
                using (var scope = _scopeFactory.CreateScope())
                {
                    var repo = scope.ServiceProvider.GetRequiredService<LogRepository>();
                    var toSave = new Payload { log = payload };
                    await repo.SaveLogs(toSave);

                    string discordMessage = $"New data in database {DateTime.Now:HH:mm:ss}";
                    await _discordAlert.SendAlert(discordMessage);
                }
            }
        }
    }
}