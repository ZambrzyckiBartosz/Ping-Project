using System.Security.Cryptography;

namespace Ping_Project.Services;


public class Decrypt
{

    private readonly byte[] _key;

    public Decrypt(IConfiguration config)
    {
        string? baseKey = config.GetValue<string>("EncryptionKey");
        if (string.IsNullOrEmpty(baseKey)) throw new ArgumentException("Empty Encryptionkey");
        _key = Convert.FromBase64String(baseKey);
    }

    public string DecryptService(string cipherText)
    {
        byte[] fullCipher = Convert.FromBase64String(cipherText);

        using Aes aes = Aes.Create();
        aes.Key = _key;

        byte[] iv = new byte[16];
        Array.Copy(fullCipher, 0, iv, 0, iv.Length);
        aes.IV = iv;
        
        using var decryptor = aes.CreateDecryptor(aes.Key,aes.IV);
        using var ms = new MemoryStream(fullCipher,16,fullCipher.Length - 16);
        using var cs = new CryptoStream(ms,decryptor,CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        
        return sr.ReadToEnd();
    }
}