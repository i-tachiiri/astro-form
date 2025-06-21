using System.Threading.Tasks;
using AstroForm.Application;
using AstroForm.Domain.Entities;
using AstroForm.Infra;
using Xunit;

namespace AstroForm.Tests
{
    public class UserServiceTests
    {
        [Fact]
        public async Task RegisterAndUpdateRole_Works()
        {
            var repo = new InMemoryUserRepository();
            var service = new UserService(repo);

            var user = await service.RegisterAsync("u1", "test", "t@example.com", DateTime.UtcNow);
            Assert.Equal(UserRole.FortuneTeller, user.Role);

            await service.UpdateRoleAsync("u1", UserRole.Admin);
            var updated = await service.GetByIdAsync("u1");
            Assert.Equal(UserRole.Admin, updated?.Role);
        }
    }
}
