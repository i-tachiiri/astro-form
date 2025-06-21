using System;
using System.Linq;
using System.Threading.Tasks;
using AstroForm.Domain.Entities;
using AstroForm.Infra;
using Xunit;

namespace Domain.Tests;

public class EncryptedFormRepositoryTests
{
    [Fact]
    public async Task SaveAndGet_EncryptsAndDecrypts()
    {
        var inner = new InMemoryFormRepository();
        var key = new byte[32];
        new Random().NextBytes(key);
        var enc = new AesEncryptionService(key);
        var repo = new EncryptedFormRepository(inner, enc);
        var form = new Form { Id = Guid.NewGuid(), UserId = "u" };
        form.FormSubmissions.Add(new FormSubmission { Id = Guid.NewGuid(), FormId = form.Id, Answers = "ans" });
        await repo.SaveAsync(form);

        var raw = await inner.GetByIdAsync(form.Id);
        Assert.NotNull(raw);
        Assert.NotEqual("ans", raw!.FormSubmissions.First().Answers);

        var loaded = await repo.GetByIdAsync(form.Id);
        Assert.Equal("ans", loaded!.FormSubmissions.First().Answers);
    }
}
