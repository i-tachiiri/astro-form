using AstroForm.Application;
using AstroForm.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace AstroForm.Functions;

public class UserFunctions
{
    private readonly UserService _service;

    public UserFunctions(UserService service)
    {
        _service = service;
    }

    [FunctionName("RegisterUser")]
    public async Task<IActionResult> RegisterUser(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "users/register")] HttpRequest req)
    {
        var data = await req.ReadFromJsonAsync<UserRegistration>() ?? new UserRegistration();
        var user = await _service.RegisterAsync(data.Id, data.DisplayName, data.Email, data.Role);
        return new OkObjectResult(user);
    }

    [FunctionName("UpdateUserRole")]
    public async Task<IActionResult> UpdateUserRole(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "users/{id}/role")] HttpRequest req,
        string id)
    {
        var update = await req.ReadFromJsonAsync<RoleUpdate>() ?? new RoleUpdate();
        await _service.UpdateRoleAsync(id, update.Role);
        return new OkResult();
    }

    public record UserRegistration(string Id, string DisplayName, string Email, UserRole Role);
    public record RoleUpdate(UserRole Role);
}
