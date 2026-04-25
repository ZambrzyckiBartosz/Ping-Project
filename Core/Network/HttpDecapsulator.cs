namespace Ping_Project.Core.Network;

public static class HttpDecapsulator
{
    public static string? ExtractPayload(string rawHttpRequest)
    {

        string startTag = "session_id=";
        string endTag = ";";

        int startIndex = rawHttpRequest.IndexOf(startTag);
        if (startIndex != -1)
        {
            startIndex += startTag.Length;
            int endIndex = rawHttpRequest.IndexOf(endTag, startIndex);

            if (endIndex == -1)
            {
                endIndex = rawHttpRequest.IndexOf('\n', startIndex);
            }

            string receivedData = rawHttpRequest.Substring(startIndex, endIndex - startIndex);
            return receivedData.Trim('\r', '\n');
        }

        return null;
    }
}