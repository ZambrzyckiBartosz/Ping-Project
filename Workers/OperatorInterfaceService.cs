using System.Net;
using System.Net.Sockets;
using Ping_Project.Core.State;
using System.Text;

namespace Ping_Project.Workers;

public class OperatorInterfaceService(IConfiguration _configuration, TaskQueueManager _queueManager) : BackgroundService
{
    private readonly int _port = _configuration.GetValue<int>("LoopbackPort");
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        TcpListener listener = new TcpListener(IPAddress.Loopback, _port);
        listener.Start();

        while (!stoppingToken.IsCancellationRequested)
        {
            using TcpClient client = await listener.AcceptTcpClientAsync(stoppingToken);

            using StreamReader streamReader = new StreamReader(client.GetStream(), Encoding.UTF8);
            using StreamWriter streamWriter = new StreamWriter(client.GetStream(), Encoding.UTF8);

            string? rawData = await streamReader.ReadLineAsync(stoppingToken);
            
            if (!string.IsNullOrEmpty(rawData))
            {
                _queueManager.AddTask(rawData);
                
                await streamWriter.WriteLineAsync("Added new order");
            }
        }
        throw new NotImplementedException();
    }
}