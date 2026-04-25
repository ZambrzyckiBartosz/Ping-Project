using Ping_Project.Core.Validation;
using Ping_Project.Infrastructure.Security;

namespace Ping_Project.Core.Network;

public class PayloadProcessor(DataValidation _validation)
{
    public async Task Processor(string cipherText)
    {
        string decryptedText = await rsaHandler.decryptRsa(cipherText);

        if (string.IsNullOrEmpty(decryptedText)) return;
        
        string[] subData = decryptedText.Split('|', 3);

        Console.WriteLine(subData.Length);
        if (subData.Length == 3)
        {
            await _validation.ValidData(subData[0], subData[1], subData[2]);
        }
    }
}