using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Ping_Project.Entities;
using Ping_Project.Infrastructure.Repository;
using Ping_Project.Validation;

namespace Ping_Project.Services;

public class TcpReceiverService(IServiceScopeFactory _scopeFactory, IConfiguration _configuration) : BackgroundService
{
    private readonly int _port = _configuration.GetValue<int>("Port");
    private readonly string? _certificatePassword = _configuration.GetValue <string>("CertPassword");
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        TcpListener listener = new TcpListener(IPAddress.Any, _port);
        listener.Start();
        var serverCertificate = new X509Certificate2("/home/bob/Ping-Project/croc_shield.pfx",_certificatePassword);
        Console.WriteLine("Listening on port " + _port);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using TcpClient client = await listener.AcceptTcpClientAsync(stoppingToken);

                    await using NetworkStream rawStream = client.GetStream();
                    await using SslStream sslStream = new SslStream(rawStream, false);
                    await sslStream.AuthenticateAsServerAsync(serverCertificate, clientCertificateRequired: false,
                        checkCertificateRevocation: false);
                    using StreamReader reader = new StreamReader(sslStream, Encoding.UTF8);

                    string? receivedData = await reader.ReadLineAsync(stoppingToken);
                    if (!string.IsNullOrEmpty(receivedData))
                    {
                        Console.WriteLine("New data received");

                        string[] subData = receivedData.Split('|',2);
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
        
        catch when (stoppingToken.IsCancellationRequested){}
        
        finally
        {
            listener.Stop();
        }
    }
}