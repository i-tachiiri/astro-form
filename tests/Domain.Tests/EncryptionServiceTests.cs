using AstroForm.Domain;

namespace Domain.Tests;

public class EncryptionServiceTests
{
    [Fact]
    public void EncryptAndDecrypt_ReturnsOriginalText()
    {
        byte[] key = Enumerable.Range(1, 32).Select(i => (byte)i).ToArray();
        byte[] iv = Enumerable.Range(1, 16).Select(i => (byte)(i + 32)).ToArray();
        var service = new AesEncryptionService(key, iv);
        const string text = "hello";
        var encrypted = service.Encrypt(text);
        var decrypted = service.Decrypt(encrypted);
        Assert.Equal(text, decrypted);
    }
}

