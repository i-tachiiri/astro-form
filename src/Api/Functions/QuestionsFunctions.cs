using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AstroForm.Api.Functions;

public class QuestionsFunctions
{
    private readonly ILogger _logger;
    public QuestionsFunctions(ILogger<QuestionsFunctions> logger)
    {
        _logger = logger;
    }

    [Function("GetQuestions")]
    public HttpResponseData GetQuestions([
        HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "questions")]
        HttpRequestData req)
    {
        _logger.LogInformation("GetQuestions called");
        var res = req.CreateResponse(HttpStatusCode.OK);
        res.WriteString("questions list");
        return res;
    }

    [Function("CreateQuestion")]
    public HttpResponseData CreateQuestion([
        HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "questions")]
        HttpRequestData req)
    {
        _logger.LogInformation("CreateQuestion called");
        var res = req.CreateResponse(HttpStatusCode.OK);
        res.WriteString("question created");
        return res;
    }
}
