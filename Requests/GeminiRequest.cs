using GenerativeAI;

namespace Ping_Project.Requests;

public class GeminiRequest
{
    public static async Task<string> AskGemini(string apiKey, string question)
    {
        var model = new GenerativeModel(apiKey, "gemini-2.5-flash");

        try
        {
            var respone = await model.GenerateContentAsync(question);
            return respone.Text;
        }
        catch (Exception e)
        {
            return "Exception: " + e.Message;
        }
    }
}