using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;

namespace AstroForm.Api.Services;

public interface IExternalIdUserService
{
    Task<string> RegisterUserAsync(string displayName, string email);
}

public class ExternalIdUserService : IExternalIdUserService
{
    private readonly GraphServiceClient _client;
    private readonly string _tenantId;

    public ExternalIdUserService(IConfiguration configuration)
    {
        _tenantId = configuration["TENANT_ID"] ?? throw new ArgumentNullException("TENANT_ID");
        var clientId = configuration["CLIENT_ID"] ?? throw new ArgumentNullException("CLIENT_ID");
        var clientSecret = configuration["CLIENT_SECRET"] ?? throw new ArgumentNullException("CLIENT_SECRET");
        var credential = new ClientSecretCredential(_tenantId, clientId, clientSecret);
        _client = new GraphServiceClient(credential, new[] { "https://graph.microsoft.com/.default" });
    }

    public async Task<string> RegisterUserAsync(string displayName, string email)
    {
        var password = Guid.NewGuid().ToString("N") + "!Aa1";
        var user = new User
        {
            AccountEnabled = true,
            DisplayName = displayName,
            Identities = new List<ObjectIdentity>
            {
                new ObjectIdentity
                {
                    SignInType = "emailAddress",
                    Issuer = _tenantId,
                    IssuerAssignedId = email
                }
            },
            PasswordProfile = new PasswordProfile
            {
                Password = password,
                ForceChangePasswordNextSignIn = true
            }
        };

        var result = await _client.Users.PostAsync(user);
        return result?.Id ?? string.Empty;
    }
}
