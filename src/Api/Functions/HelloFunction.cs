using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AstroForm.Api.Functions;

public class HelloFunction
{
    private readonly ILogger _logger;

    public HelloFunction(ILogger<HelloFunction> logger)
    {
        _logger = logger;
    }

    [Function("Hello")]
    public HttpResponseData Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "hello")] HttpRequestData req)
    {
        _logger.LogInformation("HelloFunction triggered");
        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        response.WriteString("Hello World");
        return response;
    }
}
