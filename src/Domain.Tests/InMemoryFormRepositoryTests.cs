using System;
using System.Threading.Tasks;
using AstroForm.Domain.Entities;
using AstroForm.Infra;
using Xunit;

namespace Domain.Tests;

public class InMemoryFormRepositoryTests
{
    [Fact]
    public async Task SaveGetAndDeleteForm_Works()
    {
        var repo = new InMemoryFormRepository();
        var form = new Form { Id = Guid.NewGuid(), UserId = "u" };
        await repo.SaveAsync(form);
        var loaded = await repo.GetByIdAsync(form.Id);
        Assert.NotNull(loaded);
        await repo.DeleteFormAsync(form.Id);
        var afterDelete = await repo.GetByIdAsync(form.Id);
        Assert.Null(afterDelete);
    }

    [Fact]
    public async Task DeleteFormsByUser_RemovesAll()
    {
        var repo = new InMemoryFormRepository();
        var f1 = new Form { Id = Guid.NewGuid(), UserId = "u1" };
        var f2 = new Form { Id = Guid.NewGuid(), UserId = "u1" };
        await repo.SaveAsync(f1);
        await repo.SaveAsync(f2);
        await repo.DeleteFormsByUserAsync("u1");
        var list = await repo.GetAllAsync();
        Assert.Empty(list);
    }

    [Fact]
    public async Task SubmissionOperations_Work()
    {
        var repo = new InMemoryFormRepository();
        var form = new Form { Id = Guid.NewGuid(), UserId = "u" };
        form.FormSubmissions.Add(new FormSubmission { Id = Guid.NewGuid(), FormId = form.Id });
        await repo.SaveAsync(form);
        var submissions = await repo.GetSubmissionsAsync(form.Id);
        Assert.Single(submissions);
        await repo.DeleteSubmissionAsync(form.Id, submissions[0].Id);
        submissions = await repo.GetSubmissionsAsync(form.Id);
        Assert.Empty(submissions);
    }
}
