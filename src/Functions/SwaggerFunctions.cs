using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace AstroForm.Functions;

public class SwaggerFunctions
{
    private readonly ISwaggerProvider _swaggerProvider;

    public SwaggerFunctions(ISwaggerProvider swaggerProvider)
    {
        _swaggerProvider = swaggerProvider;
    }

    [FunctionName("OpenApiJson")]
    public IActionResult GetOpenApiJson(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "openapi.json")] HttpRequest req)
    {
        var doc = _swaggerProvider.GetSwagger("v1");
        return new OkObjectResult(doc);
    }
}
