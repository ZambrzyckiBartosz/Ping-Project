using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Ping_Project.Handlers;
using Ping_Project.Core.Validation;

namespace Ping_Project.Services;

public class TcpReceiverService(IConfiguration _configuration, DataValidation _dataValidation) : BackgroundService
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

                    using var timeout = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                    timeout.CancelAfter(TimeSpan.FromSeconds(10));

                    await using NetworkStream rawStream = client.GetStream();
                    await using SslStream sslStream = new SslStream(rawStream, false);

                    var sslOptions = new SslServerAuthenticationOptions
                    {
                        ServerCertificate = serverCertificate,
                        ClientCertificateRequired = false,
                        CertificateRevocationCheckMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck
                    };
                    
                    await sslStream.AuthenticateAsServerAsync(sslOptions, timeout.Token);
                    using StreamReader reader = new StreamReader(sslStream, Encoding.UTF8);

                    char[] buffer = new char[4096];
                    int bytesToRead = await reader.ReadAsync(buffer,0, buffer.Length);
                    string rawData = new string (buffer, 0, bytesToRead);

                    string startTag = "session_id=";
                    string endTag = ";";

                    int startIndex = rawData.IndexOf(startTag);
                    if (startIndex != -1)
                    {
                        
                        startIndex += startTag.Length;
                        int endIndex = rawData.IndexOf(endTag,startIndex);

                        if (endIndex == -1)
                        {
                            endIndex = rawData.IndexOf('\n', startIndex);
                        }
                        
                        string receivedData = rawData.Substring(startIndex,endIndex - startIndex);
                        receivedData = receivedData.Trim('\r','\n');
                        if (!string.IsNullOrEmpty(receivedData))
                        {
                            var decryptedData = await rsaHandler.decryptRsa(receivedData);

                            if (string.IsNullOrEmpty(decryptedData))
                            {
                                Console.WriteLine("RSA error");
                                continue;
                            }

                            Console.WriteLine($"Decrypted data: '{decryptedData}'");
                            string[] subData = decryptedData.Split('|', 3);

                            Console.WriteLine(subData.Length);
                            if (subData.Length == 3)
                            {
                                await _dataValidation.ValidData(subData[0], subData[1], subData[2]);

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex is OperationCanceledException)
                    {
                        Console.WriteLine("Tiemout, too slow connection");
                    }

                    else
                    {
                        Console.WriteLine($"Ignored wrong TCP package: {ex.Message}");
                    }
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