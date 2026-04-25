using System.Security.Cryptography;
using System.Text;

namespace Ping_Project.Infrastructure.Security;
public static class rsaHandler
{

    public static async Task<string> decryptRsa(string receivedData)
    {
        Console.WriteLine("New encrypted data");

        string decryptedData = string.Empty;

        try
        {
            string privateKey = await File.ReadAllTextAsync("/home/bob/Ping-Project/rsa_private.pem");
            using RSA rsa = RSA.Create();
            rsa.ImportFromPem(privateKey);

            byte[] encryptedBytes = Convert.FromBase64String(receivedData);
            byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.Pkcs1);

            decryptedData = Encoding.UTF8.GetString(decryptedBytes);
            Console.WriteLine("RSA decrypted data");
        }
        catch 
        {
            Console.WriteLine("Not encrypted data");
        }

        return decryptedData;
    }
}