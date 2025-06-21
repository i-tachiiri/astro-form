using AstroForm.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using AstroForm.Domain.Entities;

namespace AstroForm.Functions;

public class LogFunctions
{
    private readonly ActivityLogService _service;

    public LogFunctions(ActivityLogService service)
    {
        _service = service;
    }

    [FunctionName("GetLogs")]
    public async Task<IActionResult> GetLogs(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "logs")] HttpRequest req)
    {
        if (!req.HttpContext.User.IsInRole(UserRole.Admin.ToString()))
        {
            return new UnauthorizedResult();
        }
        string? userId = req.Query["userId"]; // may be empty
        Guid? formId = Guid.TryParse(req.Query["formId"], out var id) ? id : null;
        var logs = await _service.GetLogsAsync(userId, formId);
        return new OkObjectResult(logs);
    }
}
