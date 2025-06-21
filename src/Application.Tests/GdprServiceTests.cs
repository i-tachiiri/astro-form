using System;
using System.Threading.Tasks;
using AstroForm.Application;
using AstroForm.Domain.Entities;
using AstroForm.Infra;
using Xunit;

namespace AstroForm.Tests;

public class GdprServiceTests
{
    [Fact]
    public async Task ProcessDeletionQueue_RemovesUserAndForms()
    {
        var users = new InMemoryUserRepository();
        var forms = new InMemoryFormRepository();
        var service = new GdprService(users, forms);

        var user = new User
        {
            Id = "u1",
            Email = "u1@example.com",
            DisplayName = "u1",
            Role = UserRole.Assistant,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ConsentGivenAt = DateTime.UtcNow
        };
        await users.SaveAsync(user);

        var form1 = new Form { Id = Guid.NewGuid(), UserId = "u1" };
        var form2 = new Form { Id = Guid.NewGuid(), UserId = "u1" };
        await forms.SaveAsync(form1);
        await forms.SaveAsync(form2);

        service.RequestUserDeletion("u1");
        await service.ProcessDeletionQueueAsync();

        Assert.Null(await users.GetByIdAsync("u1"));
        Assert.Empty(await forms.GetAllAsync());
    }
}
