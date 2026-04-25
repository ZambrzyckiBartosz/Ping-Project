using System.Text;

namespace Ping_Project.Workers;

public static class CommandService
{
    public static string CreateResponse(string? command)
    {
        StringBuilder httpResponse = new StringBuilder();

        if (!string.IsNullOrEmpty(command))
        {
            httpResponse.Append("HTTP/1.1 200 OK\r\n");
            httpResponse.Append("Content-Type: text/plain\r\n");
            httpResponse.Append($"Content-Length: {Encoding.UTF8.GetByteCount(command)}\r\n");
            httpResponse.Append("Connection: close\r\n");
            httpResponse.Append("\r\n");
            httpResponse.Append(command);
            return httpResponse.ToString();
        }
        
        httpResponse.Append("HTTP/1.1 200 OK\r\n");
        httpResponse.Append("Content-Length: 0\r\n");
        httpResponse.Append("Connection: close\r\n");
        httpResponse.Append("\r\n");
        
        return httpResponse.ToString();
    }
}