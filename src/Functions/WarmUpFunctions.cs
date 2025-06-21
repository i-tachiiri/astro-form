using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace AstroForm.Functions;

public class WarmUpFunctions
{
    [FunctionName("WarmUp")]
    public IActionResult WarmUp(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "warmup")] HttpRequest req,
        ILogger<WarmUpFunctions> logger)
    {
        logger.LogInformation("WarmUp triggered");
        return new OkResult();
    }
}
