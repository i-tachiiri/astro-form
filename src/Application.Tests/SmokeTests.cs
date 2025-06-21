using AstroForm.Application;
using AstroForm.Domain.Repositories;
using AstroForm.Infra;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

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
}
