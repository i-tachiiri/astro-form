using System.Threading.Tasks;
using AstroForm.Domain.Entities;
using AstroForm.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AstroForm.Infra;

public class EntraExternalIdentityService : IExternalIdentityService
{
    private readonly ILogger<EntraExternalIdentityService> _logger;

    public EntraExternalIdentityService(IConfiguration config, ILogger<EntraExternalIdentityService> logger)
    {
        _logger = logger;
        // In this template implementation we only log operations. Actual Graph integration is omitted.
    }

    public Task CreateUserAsync(string id, string displayName, string email)
    {
        _logger.LogInformation("Create user {Id} {Email}", id, email);
        return Task.CompletedTask;
    }

    public Task UpdateUserRoleAsync(string id, UserRole role)
    {
        _logger.LogInformation("Update role of {Id} to {Role}", id, role);
        return Task.CompletedTask;
    }

    public Task DeleteUserAsync(string id)
    {
        _logger.LogInformation("Delete user {Id}", id);
        return Task.CompletedTask;
    }
}
