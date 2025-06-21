using AstroForm.Application;
using AstroForm.Domain.Repositories;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AstroForm.Functions;

public class DataPurgeFunctions
{
    private readonly IFormRepository _repository;
    private readonly FormAnswerService _answerService;

    public DataPurgeFunctions(IFormRepository repository, FormAnswerService answerService)
    {
        _repository = repository;
        _answerService = answerService;
    }

    [FunctionName("PurgeOldSubmissions")]
    public async Task PurgeOldSubmissions([TimerTrigger("0 0 0 * * *")]TimerInfo timer, ILogger logger)
    {
        var forms = await _repository.GetAllAsync();
        foreach (var form in forms)
        {
            await _answerService.PurgeOldSubmissionsAsync(form.Id, TimeSpan.FromDays(30));
        }
        logger.LogInformation("Old submissions purged");
    }
}
