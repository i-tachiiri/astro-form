using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AstroForm.Domain.Entities;
using AstroForm.Domain.Services;
using AppUser = AstroForm.Domain.Entities.User;
using GraphUser = Microsoft.Graph.Models.User;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions;

namespace AstroForm.Infra;

public class EntraExternalIdentityService : IExternalIdentityService
{
    private readonly ILogger<EntraExternalIdentityService> _logger;
    private readonly GraphServiceClient? _graph;
    private readonly Dictionary<UserRole, string?> _groups;
    public EntraExternalIdentityService(IConfiguration config, ILogger<EntraExternalIdentityService> logger)
    {
        _logger = logger;
        var tenant = config["Entra:TenantId"];
        var clientId = config["Entra:ClientId"];
        var secret = config["Entra:ClientSecret"];
        if (!string.IsNullOrEmpty(tenant) && !string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(secret))
        {
            var cred = new ClientSecretCredential(tenant, clientId, secret);
            _graph = new GraphServiceClient(cred);
        }
        else
        {
            _graph = null!; // will log only
        }
        _groups = new()
        {
            [UserRole.FortuneTeller] = config["Entra:GroupFortuneTeller"],
            [UserRole.Assistant] = config["Entra:GroupAssistant"],
            [UserRole.Admin] = config["Entra:GroupAdmin"]
        };
    }

    public async Task CreateUserAsync(string id, string displayName, string email)
    {
        if (_graph is null)
        {
            _logger.LogInformation("Create user {Id} {Email}", id, email);
            return;
        }
        var user = new GraphUser
        {
            Id = id,
            DisplayName = displayName,
            Mail = email
        };
        await _graph.Users[id].PatchAsync(user);
    }

    public async Task UpdateUserRoleAsync(string id, UserRole role)
    {
        if (_graph is null)
        {
            _logger.LogInformation("Update role of {Id} to {Role}", id, role);
            return;
        }
        foreach (var g in _groups.Values.Where(g => !string.IsNullOrEmpty(g)))
        {
            try
            {
                await _graph.Groups[g].Members[id].Ref.DeleteAsync();
            }
            catch (ApiException ex) when (ex.ResponseStatusCode == (int)System.Net.HttpStatusCode.NotFound)
            {
            }
        }
        var target = _groups.GetValueOrDefault(role);
        if (!string.IsNullOrEmpty(target))
        {
            var body = new ReferenceCreate { OdataId = $"https://graph.microsoft.com/v1.0/users/{id}" };
            await _graph.Groups[target].Members.Ref.PostAsync(body);
        }
    }

    public Task DeleteUserAsync(string id)
    {
        if (_graph is null)
        {
            _logger.LogInformation("Delete user {Id}", id);
            return Task.CompletedTask;
        }
        return _graph.Users[id].DeleteAsync();
    }
}
