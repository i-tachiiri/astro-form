using System.Threading.Tasks;
using AstroForm.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace AstroForm.Functions;

public class GdprFunctions
{
    private readonly GdprService _service;

    public GdprFunctions(GdprService service)
    {
        _service = service;
    }

    [FunctionName("RequestUserDeletion")]
    public IActionResult RequestUserDeletion(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "gdpr/delete/{id}")] HttpRequest req,
        string id)
    {
        _service.RequestUserDeletion(id);
        return new OkResult();
    }

    [FunctionName("ProcessDeletionRequests")]
    public async Task ProcessDeletionRequests([
        TimerTrigger("0 */5 * * * *")] TimerInfo timer)
    {
        await _service.ProcessDeletionQueueAsync();
    }
}
