using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using AstroForm.Api.Services;
using AstroForm.Presentation.Shared;

namespace AstroForm.Api.Functions;

public class UsersFunctions
{
    private readonly ILogger _logger;
    private readonly IExternalIdUserService _userService;

    public UsersFunctions(ILogger<UsersFunctions> logger, IExternalIdUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [Function("GetUsers")]
    public HttpResponseData GetUsers([
        HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users")]
        HttpRequestData req)
    {
        _logger.LogInformation("GetUsers called");
        var res = req.CreateResponse(HttpStatusCode.OK);
        res.WriteString("users list");
        return res;
    }

    [Function("CreateUser")]
    public async Task<HttpResponseData> CreateUser([
        HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users")]
        HttpRequestData req)
    {
        _logger.LogInformation("CreateUser called");
        var request = await JsonSerializer.DeserializeAsync<RegisterUserRequest>(req.Body);
        if (request is null)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        var id = await _userService.RegisterUserAsync(request.DisplayName, request.Email);
        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(new { UserId = id });
        return res;
    }
}
