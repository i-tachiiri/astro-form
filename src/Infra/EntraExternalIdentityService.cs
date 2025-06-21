using System.Threading.Tasks;
using AstroForm.Domain.Entities;
using AstroForm.Domain.Services;
using Microsoft.Extensions.Logging;

namespace AstroForm.Infra;

public class EntraExternalIdentityService : IExternalIdentityService
{
    private readonly ILogger<EntraExternalIdentityService> _logger;

    public EntraExternalIdentityService(ILogger<EntraExternalIdentityService> logger)
    {
        _logger = logger;
    }

    public async Task CreateUserAsync(string id, string displayName, string email)
    {
        _logger.LogInformation("Create user {Id} {Email}", id, email);
        await Task.CompletedTask;
    }

    public async Task UpdateUserRoleAsync(string id, UserRole role)
    {
        _logger.LogInformation("Update role of {Id} to {Role}", id, role);
        await Task.CompletedTask;
    }

    public Task DeleteUserAsync(string id)
    {
        _logger.LogInformation("Delete user {Id}", id);
        return Task.CompletedTask;
    }
}
