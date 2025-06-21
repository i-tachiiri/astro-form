using System;
using System.Threading.Tasks;
using AstroForm.Domain.Entities;
using AstroForm.Domain.Repositories;
using AstroForm.Domain.Services;

namespace AstroForm.Application
{
    public class UserService
    {
        private readonly IUserRepository _repository;
        private readonly IExternalIdentityService _externalId;

        public UserService(IUserRepository repository, IExternalIdentityService externalId)
        {
            _repository = repository;
            _externalId = externalId;
        }

        public async Task<User> RegisterAsync(string id, string displayName, string email, DateTime consentGivenAt)
        {
            var user = new User
            {
                Id = id,
                DisplayName = displayName,
                Email = email,
                Role = UserRole.FortuneTeller,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ConsentGivenAt = consentGivenAt
            };
            await _externalId.CreateUserAsync(id, displayName, email);
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
            await _externalId.UpdateUserRoleAsync(id, role);
        }

        public async Task DeleteUserAsync(string id, IFormRepository forms)
        {
            await _repository.DeleteAsync(id);
            await forms.DeleteFormsByUserAsync(id);
            await _externalId.DeleteUserAsync(id);
        }
    }
}
