using System.Security.Cryptography;

namespace Ping_Project.Services;

public class Encrypt
{
    private readonly byte[] _key;

    public Encrypt(IConfiguration _config)
    {
        string? baseKey = _config.GetValue<string>("EncryptionKey");
        if (string.IsNullOrEmpty(baseKey)) throw new ArgumentException("Empty EncryptionKey");
        _key = Convert.FromBase64String(baseKey);
    }

    public string EncryptService(string PlainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        
        ms.Write(aes.IV, 0, aes.IV.Length);

        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(PlainText);
        }
        return Convert.ToBase64String(ms.ToArray());
    }
}