using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using AstroForm.Domain;

namespace AstroForm.Api.Functions;

public class AnswersFunctions
{
    private readonly ILogger _logger;
    private readonly IEncryptionService _encryptionService;
    private static readonly List<string> _answers = new();

    public AnswersFunctions(ILogger<AnswersFunctions> logger, IEncryptionService encryptionService)
    {
        _logger = logger;
        _encryptionService = encryptionService;
    }

    [Function("PostAnswer")]
    public async Task<HttpResponseData> PostAnswer([
        HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "answers")]
        HttpRequestData req)
    {
        _logger.LogInformation("PostAnswer called");
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        var encrypted = _encryptionService.Encrypt(body);
        _answers.Add(encrypted);
        var res = req.CreateResponse(HttpStatusCode.OK);
        res.WriteString(encrypted);
        return res;
    }

    [Function("GetAnswers")]
    public HttpResponseData GetAnswers([
        HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "answers")]
        HttpRequestData req)
    {
        _logger.LogInformation("GetAnswers called");
        var res = req.CreateResponse(HttpStatusCode.OK);
        var list = string.Join("\n", _answers);
        res.WriteString(list);
        return res;
    }

    [Function("GetAnswersCsv")]
    public HttpResponseData GetAnswersCsv([
        HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "answers/csv")]
        HttpRequestData req)
    {
        _logger.LogInformation("GetAnswersCsv called");
        var res = req.CreateResponse(HttpStatusCode.OK);
        var csv = string.Join("\n", _answers.Select(a => $"\"{a}\""));
        res.Headers.Add("Content-Disposition", "attachment; filename=answers.csv");
        res.WriteString(csv);
        return res;
    }
}
