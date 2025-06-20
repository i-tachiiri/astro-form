using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AstroForm.Api.Functions;

public class LogsFunctions
{
    private readonly ILogger _logger;
    public LogsFunctions(ILogger<LogsFunctions> logger)
    {
        _logger = logger;
    }

    [Function("GetLogs")]
    public HttpResponseData GetLogs([
        HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "logs")]
        HttpRequestData req)
    {
        _logger.LogInformation("GetLogs called");
        var res = req.CreateResponse(HttpStatusCode.OK);
        res.WriteString("logs list");
        return res;
    }
}
