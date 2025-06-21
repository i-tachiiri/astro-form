using AstroForm.Application;
using AstroForm.Domain.Repositories;
using AstroForm.Domain.Services;
using AstroForm.Infra;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using AstroForm.Domain.Entities;
namespace Application.Tests;
public class SmokeTests
{
    [Fact]
    public void ServiceProviderBuildsWithoutExceptions()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IFormRepository, InMemoryFormRepository>();
        services.AddSingleton<IActivityLogRepository, InMemoryActivityLogRepository>();
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddSingleton<IEmailService, InMemoryEmailService>();
        services.AddSingleton<IExternalIdentityService, StubExternalId>();
        services.AddSingleton(sp => new FormPublishService("/tmp/public", "/tmp/preview"));
        services.AddSingleton<FormAnswerService>();
        services.AddSingleton<ActivityLogService>();
        services.AddSingleton<UserService>();

        var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<FormAnswerService>());
        Assert.NotNull(provider.GetService<UserService>());
        Assert.NotNull(provider.GetService<ActivityLogService>());
        Assert.NotNull(provider.GetService<FormPublishService>());
    }

    private class StubExternalId : IExternalIdentityService
    {
        public Task CreateUserAsync(string id, string displayName, string email) => Task.CompletedTask;
        public Task DeleteUserAsync(string id) => Task.CompletedTask;
        public Task UpdateUserRoleAsync(string id, UserRole role) => Task.CompletedTask;
    }
}
