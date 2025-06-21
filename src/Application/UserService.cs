using System;
using System.Threading.Tasks;
using AstroForm.Domain.Entities;
using AstroForm.Domain.Repositories;

namespace AstroForm.Application
{
    public class UserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<User> RegisterAsync(string id, string displayName, string email)
        {
            var user = new User
            {
                Id = id,
                DisplayName = displayName,
                Email = email,
                Role = UserRole.FortuneTeller,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _repository.SaveAsync(user);
            return user;
        }

        public Task<User?> GetByIdAsync(string id) => _repository.GetByIdAsync(id);

        public async Task UpdateRoleAsync(string id, UserRole role)
        {
            var user = await _repository.GetByIdAsync(id) ?? throw new InvalidOperationException("User not found");
            user.Role = role;
            user.UpdatedAt = DateTime.UtcNow;
            await _repository.SaveAsync(user);
        }
    }
}
