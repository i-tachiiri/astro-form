using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AstroForm.Api.Functions;

public class UsersFunctions
{
    private readonly ILogger _logger;
    public UsersFunctions(ILogger<UsersFunctions> logger)
    {
        _logger = logger;
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
    public HttpResponseData CreateUser([
        HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users")]
        HttpRequestData req)
    {
        _logger.LogInformation("CreateUser called");
        var res = req.CreateResponse(HttpStatusCode.OK);
        res.WriteString("user created");
        return res;
    }
}
