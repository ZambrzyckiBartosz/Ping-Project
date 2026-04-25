using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace Ping_Project.Workers;

public class TcpReceiverService(IConfiguration _configuration, CrocConnectionService _crocConnection)
    : BackgroundService
{
    private readonly int _port = _configuration.GetValue<int>("Port");
    private readonly string? _certificatePassword = _configuration.GetValue<string>("CertPassword");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        TcpListener listener = new TcpListener(IPAddress.Any, _port);
        listener.Start();
        var serverCertificate = X509CertificateLoader.LoadPkcs12FromFile("/home/bob/Ping-Project/croc_shield.pfx", _certificatePassword);
        Console.WriteLine("Listening on port " + _port);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                TcpClient client = await listener.AcceptTcpClientAsync(stoppingToken);
                _ = Task.Run(() => _crocConnection.HandleClient(client, serverCertificate, stoppingToken),
                    stoppingToken);
            }
        }
        catch when (stoppingToken.IsCancellationRequested)
        {
        }
        finally
        {
            listener.Stop();
        }
    }
}