using System.Threading.Tasks;
using AstroForm.Application;
using AstroForm.Domain.Entities;
using AstroForm.Infra;
using Xunit;

using AstroForm.Domain.Services;
namespace AstroForm.Tests
{
    public class UserServiceTests
    {
        [Fact]
        public async Task RegisterAndUpdateRole_Works()
        {
            var repo = new InMemoryUserRepository();
            var service = new UserService(repo, new StubExternalId());

            var user = await service.RegisterAsync("u1", "test", "t@example.com", DateTime.UtcNow);
            Assert.Equal(UserRole.FortuneTeller, user.Role);

            await service.UpdateRoleAsync("u1", UserRole.Admin);
            var updated = await service.GetByIdAsync("u1");
            Assert.Equal(UserRole.Admin, updated?.Role);
        }

        private class StubExternalId : IExternalIdentityService
        {
            public Task CreateUserAsync(string id, string displayName, string email) => Task.CompletedTask;
            public Task DeleteUserAsync(string id) => Task.CompletedTask;
            public Task UpdateUserRoleAsync(string id, UserRole role) => Task.CompletedTask;
        }
    }
}
