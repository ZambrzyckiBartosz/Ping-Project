using System.Net;
using System.Net.Sockets;
using System.Text;
using Ping_Project.Entities;
using Ping_Project.Infrastructure.Repository;
using Ping_Project.Validation;

namespace Ping_Project.Services;

public class TcpReceiverService(IServiceScopeFactory _scopeFactory) : BackgroundService
{
    private readonly int _port = 4444;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        TcpListener listener = new TcpListener(IPAddress.Any, _port);
        listener.Start();
        Console.WriteLine("Listening on port " + _port);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using TcpClient client = await listener.AcceptTcpClientAsync(stoppingToken);

                    await using NetworkStream stream = client.GetStream();
                    using StreamReader reader = new StreamReader(stream, Encoding.UTF8);

                    string? receivedData = await reader.ReadLineAsync(stoppingToken);
                    if (!string.IsNullOrEmpty(receivedData))
                    {
                        Console.WriteLine("New data received");

                        string[] subData = receivedData.Split('|');
                        if (subData.Length == 2)
                        {
                            Console.WriteLine($"{DateTime.Now}: Received {subData[1]}");
                            
                            if (TokenValidaton.IsValid(subData[0]))
                            {
                                Console.WriteLine("Token is valid");

                                using (var scope = _scopeFactory.CreateScope())
                                {
                                    var repo = scope.ServiceProvider.GetRequiredService<LogRepository>();
                                    var toSave = new Payload { log = subData[1] };
                                    await repo.SaveLogs(toSave);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"Ignored wrong TCP package: {ex.Message}");
                }
            }
        }
        
        catch(Exception ex) when (stoppingToken.IsCancellationRequested){}
        
        finally
        {
            listener.Stop();
        }
    }
}