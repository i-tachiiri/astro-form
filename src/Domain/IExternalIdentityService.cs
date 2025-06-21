namespace AstroForm.Domain.Services;

using System.Threading.Tasks;
using AstroForm.Domain.Entities;

public interface IExternalIdentityService
{
    Task CreateUserAsync(string id, string displayName, string email);
    Task UpdateUserRoleAsync(string id, UserRole role);
    Task DeleteUserAsync(string id);
}
