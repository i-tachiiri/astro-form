using System.Threading.Tasks;
using AstroForm.Domain.Entities;
using AstroForm.Infra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Domain.Tests;

public class EntraExternalIdentityServiceTests
{
    [Fact]
    public async Task Methods_DoNotThrow()
    {
        var config = new ConfigurationBuilder().Build();
        var logger = NullLogger<EntraExternalIdentityService>.Instance;
        var service = new EntraExternalIdentityService(config, logger);
        await service.CreateUserAsync("id","name","e@example.com");
        await service.UpdateUserRoleAsync("id", UserRole.Admin);
        await service.DeleteUserAsync("id");
    }
}
