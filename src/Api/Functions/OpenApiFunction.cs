using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace AstroForm.Api.Functions;

public class OpenApiFunction
{
    [Function("OpenApiJson")]
    public HttpResponseData Run([
        HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "openapi.json")]
        HttpRequestData req)
    {
        var res = req.CreateResponse(HttpStatusCode.OK);
        res.Headers.Add("Content-Type", "application/json");
        res.WriteString("{\"openapi\":\"3.0.1\"}");
        return res;
    }
}
