using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AstroForm.Api.Functions;

public class FormsFunctions
{
    private readonly ILogger _logger;
    public FormsFunctions(ILogger<FormsFunctions> logger)
    {
        _logger = logger;
    }

    [Function("GetForms")]
    public HttpResponseData GetForms([
        HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "forms")]
        HttpRequestData req)
    {
        _logger.LogInformation("GetForms called");
        var res = req.CreateResponse(HttpStatusCode.OK);
        res.WriteString("forms list");
        return res;
    }

    [Function("CreateForm")]
    public HttpResponseData CreateForm([
        HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "forms")]
        HttpRequestData req)
    {
        _logger.LogInformation("CreateForm called");
        var res = req.CreateResponse(HttpStatusCode.OK);
        res.WriteString("form created");
        return res;
    }
}
