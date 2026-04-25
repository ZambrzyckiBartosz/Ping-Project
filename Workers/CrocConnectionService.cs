using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Ping_Project.Core.Network;
using Ping_Project.Core.State;

namespace Ping_Project.Workers;

public class CrocConnectionService(PayloadProcessor _payloadProcessor, TaskQueueManager _queueManager)
{
    public async Task HandleClient(TcpClient client, X509Certificate2 serverCertificate,
        CancellationToken stoppingToken)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(10));

        await using NetworkStream rawStream = client.GetStream();
        await using SslStream sslStream = new SslStream(rawStream, false);

        var sslOptions = new SslServerAuthenticationOptions
        {
            ServerCertificate = serverCertificate,
            ClientCertificateRequired = false,
            CertificateRevocationCheckMode =
                System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck
        };

        await sslStream.AuthenticateAsServerAsync(sslOptions, timeout.Token);
        using StreamReader reader = new StreamReader(sslStream, Encoding.UTF8);

        char[] buffer = new char[4096];
        int bytesToRead = await reader.ReadAsync(buffer, 0, buffer.Length);
        string rawData = new string(buffer, 0, bytesToRead);

        string? receivedData = HttpDecapsulator.ExtractPayload(rawData);

        if (!string.IsNullOrEmpty(receivedData))
        {
            await _payloadProcessor.Processor(receivedData);

            string? command = _queueManager.GetTask();

            byte[] responseBytes = Encoding.UTF8.GetBytes(CommandService.CreateResponse(command));
            await sslStream.WriteAsync(responseBytes, stoppingToken);
            await sslStream.FlushAsync(stoppingToken);
        }
    }
}