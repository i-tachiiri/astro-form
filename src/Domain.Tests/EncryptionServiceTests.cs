using AstroForm.Domain.Security;
using AstroForm.Infra;
using Xunit;

namespace Domain.Tests;

public class EncryptionServiceTests
{
    [Fact]
    public void EncryptDecrypt_RoundTrip()
    {
        var key = new byte[32];
        new Random().NextBytes(key);
        IEncryptionService service = new AesEncryptionService(key);
        const string original = "secret";
        var encrypted = service.Encrypt(original);
        Assert.NotEqual(original, encrypted);
        var decrypted = service.Decrypt(encrypted);
        Assert.Equal(original, decrypted);
    }
}
