using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using AstroForm.Domain.Security;

namespace AstroForm.Infra
{
    public class AesEncryptionService : IEncryptionService
    {
        private readonly byte[] _key;

        public AesEncryptionService(byte[] key)
        {
            if (key.Length != 32)
            {
                throw new ArgumentException("Key length must be 32 bytes for AES-256.", nameof(key));
            }
            _key = key;
        }

        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.GenerateIV();
            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            using var ms = new MemoryStream();
            ms.Write(aes.IV, 0, aes.IV.Length);
            ms.Write(cipherBytes, 0, cipherBytes.Length);
            return Convert.ToBase64String(ms.ToArray());
        }

        public string Decrypt(string cipherText)
        {
            var full = Convert.FromBase64String(cipherText);
            using var aes = Aes.Create();
            aes.Key = _key;
            var ivLength = aes.BlockSize / 8;
            var iv = new byte[ivLength];
            Array.Copy(full, 0, iv, 0, ivLength);
            aes.IV = iv;
            using var decryptor = aes.CreateDecryptor();
            var cipherBytes = new byte[full.Length - ivLength];
            Array.Copy(full, ivLength, cipherBytes, 0, cipherBytes.Length);
            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}
