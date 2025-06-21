using AstroForm.Application;
using AstroForm.Domain.Entities;
using AstroForm.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace AstroForm.Functions;

public class UserFunctions
{
    private readonly UserService _service;
    private readonly IFormRepository _forms;

    public UserFunctions(UserService service, IFormRepository forms)
    {
        _service = service;
        _forms = forms;
    }

    [FunctionName("RegisterUser")]
    public async Task<IActionResult> RegisterUser(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "users/register")] HttpRequest req)
    {
        var data = await req.ReadFromJsonAsync<UserRegistration>() ?? new UserRegistration(string.Empty, string.Empty, string.Empty, DateTime.MinValue);
        var user = await _service.RegisterAsync(data.Id, data.DisplayName, data.Email, data.ConsentGivenAt);
        return new OkObjectResult(user);
    }

    [FunctionName("UpdateUserRole")]
    public async Task<IActionResult> UpdateUserRole(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "users/{id}/role")] HttpRequest req,
        string id)
    {
        var update = await req.ReadFromJsonAsync<RoleUpdate>() ?? new RoleUpdate(UserRole.Assistant);
        await _service.UpdateRoleAsync(id, update.Role);
        return new OkResult();
    }

    [FunctionName("DeleteUser")]
    public async Task<IActionResult> DeleteUser(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "users/{id}")] HttpRequest req,
        string id)
    {
        await _service.DeleteUserAsync(id, _forms);
        return new OkResult();
    }

    public record UserRegistration(string Id, string DisplayName, string Email, DateTime ConsentGivenAt);
    public record RoleUpdate(UserRole Role);
}
