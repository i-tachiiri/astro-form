using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AstroForm.Api.Functions;

public class WarmUpFunction
{
    private readonly ILogger _logger;
    public WarmUpFunction(ILogger<WarmUpFunction> logger)
    {
        _logger = logger;
    }

    [Function("WarmUp")]
    public HttpResponseData Run([
        HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "warmup")]
        HttpRequestData req)
    {
        _logger.LogInformation("WarmUp triggered");
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.WriteString("ok");
        return response;
    }
}

