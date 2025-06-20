namespace AstroForm.Domain;

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}

public class AesEncryptionService : IEncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public AesEncryptionService(byte[] key, byte[] iv)
    {
        if (key.Length != 32)
        {
            throw new ArgumentException("Key must be 256 bit", nameof(key));
        }
        if (iv.Length != 16)
        {
            throw new ArgumentException("IV must be 128 bit", nameof(iv));
        }
        _key = key;
        _iv = iv;
    }

    public string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(plainText);
        }
        return Convert.ToBase64String(ms.ToArray());
    }

    public string Decrypt(string cipherText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        var bytes = Convert.FromBase64String(cipherText);
        using var ms = new MemoryStream(bytes);
        using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        return sr.ReadToEnd();
    }
}

