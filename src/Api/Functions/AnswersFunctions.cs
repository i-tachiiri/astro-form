using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AstroForm.Api.Functions;

public class AnswersFunctions
{
    private readonly ILogger _logger;
    public AnswersFunctions(ILogger<AnswersFunctions> logger)
    {
        _logger = logger;
    }

    [Function("PostAnswer")]
    public HttpResponseData PostAnswer([
        HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "answers")]
        HttpRequestData req)
    {
        _logger.LogInformation("PostAnswer called");
        var res = req.CreateResponse(HttpStatusCode.OK);
        res.WriteString("answer posted");
        return res;
    }

    [Function("GetAnswers")]
    public HttpResponseData GetAnswers([
        HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "answers")]
        HttpRequestData req)
    {
        _logger.LogInformation("GetAnswers called");
        var res = req.CreateResponse(HttpStatusCode.OK);
        res.WriteString("answers list");
        return res;
    }
}
