using AstroForm.Application;
using AstroForm.Domain.Entities;
using AstroForm.Domain.Repositories;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace AstroForm.Functions;

public class FormFunctions
{
    private readonly IFormRepository _repository;
    private readonly FormPublishService _publisher;
    private readonly FormAnswerService _answerService;
    private readonly ActivityLogService _logService;

    public FormFunctions(IFormRepository repository, FormPublishService publisher, FormAnswerService answerService, ActivityLogService logService)
    {
        _repository = repository;
        _publisher = publisher;
        _answerService = answerService;
        _logService = logService;
    }

    [FunctionName("GetForms")]
    public async Task<IActionResult> GetForms(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "forms")] HttpRequest req)
    {
        var list = await _repository.GetAllAsync();
        return new OkObjectResult(list.Select(f => new { f.Id, f.Name }));
    }

    [FunctionName("GetFormById")]
    public async Task<IActionResult> GetFormById(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "forms/{id}")] HttpRequest req,
        string id)
    {
        if (!Guid.TryParse(id, out var guid))
        {
            return new BadRequestResult();
        }
        var form = await _repository.GetByIdAsync(guid);
        return form is null ? new NotFoundResult() : new OkObjectResult(form);
    }

    [FunctionName("GetFormAnswers")]
    public async Task<IActionResult> GetFormAnswers(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "forms/{id}/answers")] HttpRequest req,
        string id)
    {
        if (!Guid.TryParse(id, out var guid))
        {
            return new BadRequestResult();
        }
        var answers = await _answerService.GetSubmissionsAsync(guid);
        return answers.Count == 0 ? new NotFoundResult() : new OkObjectResult(answers);
    }

    [FunctionName("ExportFormAnswersCsv")]
    public async Task<IActionResult> ExportFormAnswersCsv(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "forms/{id}/answers/csv")] HttpRequest req,
        string id)
    {
        if (!Guid.TryParse(id, out var guid))
        {
            return new BadRequestResult();
        }
        var csv = await _answerService.ExportCsvAsync(guid);
        return new FileContentResult(Encoding.UTF8.GetBytes(csv), "text/csv") { FileDownloadName = $"{id}.csv" };
    }

    [FunctionName("SendFormSubmissionEmail")]
    public async Task<IActionResult> SendFormSubmissionEmail(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "forms/{formId}/answers/{submissionId}/email")] HttpRequest req,
        string formId,
        string submissionId)
    {
        var data = await req.ReadFromJsonAsync<EmailRequest>() ?? new EmailRequest(string.Empty);
        if (!Guid.TryParse(formId, out var fid) || !Guid.TryParse(submissionId, out var sid))
        {
            return new BadRequestResult();
        }
        await _answerService.SendSubmissionEmailAsync(fid, sid, data.To);
        return new OkResult();
    }

    [FunctionName("DeleteFormSubmission")]
    public async Task<IActionResult> DeleteFormSubmission(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "forms/{formId}/answers/{submissionId}")] HttpRequest req,
        string formId,
        string submissionId)
    {
        if (!Guid.TryParse(formId, out var fid) || !Guid.TryParse(submissionId, out var sid))
        {
            return new BadRequestResult();
        }
        await _answerService.DeleteSubmissionAsync(fid, sid);
        await _logService.AddLogAsync(new ActivityLog { FormId = fid, ActionType = "DeleteSubmission" });
        return new OkResult();
    }

    [FunctionName("DeleteForm")]
    public async Task<IActionResult> DeleteForm(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "forms/{id}")] HttpRequest req,
        string id)
    {
        if (!Guid.TryParse(id, out var guid))
        {
            return new BadRequestResult();
        }
        var form = await _repository.GetByIdAsync(guid);
        if (form is null)
        {
            return new NotFoundResult();
        }
        await _repository.DeleteFormAsync(guid);
        await _logService.AddLogAsync(new ActivityLog { UserId = form.UserId, FormId = guid, ActionType = "DeleteForm" });
        return new OkResult();
    }

    [FunctionName("SaveForm")]
    public async Task<IActionResult> SaveForm(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "forms/{id}/save")] HttpRequest req,
        string id)
    {
        var form = await req.ReadFromJsonAsync<Form>() ?? new Form();
        if (!Guid.TryParse(id, out var guid))
        {
            return new BadRequestResult();
        }
        form.Id = guid;
        await _repository.SaveAsync(form);
        await _logService.AddLogAsync(new ActivityLog { UserId = form.UserId, FormId = form.Id, ActionType = "SaveForm" });
        return new OkObjectResult(form);
    }

    [FunctionName("PreviewForm")]
    public async Task<IActionResult> PreviewForm(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "forms/{id}/preview")] HttpRequest req,
        string id)
    {
        var form = await req.ReadFromJsonAsync<Form>() ?? new Form();
        if (!Guid.TryParse(id, out var guid))
        {
            return new BadRequestResult();
        }
        form.Id = guid;
        var path = await _publisher.GeneratePreviewAsync(form);
        return new OkObjectResult(new { path });
    }

    [FunctionName("DeletePreview")]
    public async Task<IActionResult> DeletePreview(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "forms/{id}/preview")] HttpRequest req,
        string id)
    {
        if (!Guid.TryParse(id, out var guid))
        {
            return new BadRequestResult();
        }
        await _publisher.DeletePreviewAsync(guid);
        return new OkResult();
    }

    [FunctionName("PublishForm")]
    public async Task<IActionResult> PublishForm(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "forms/{id}/publish")] HttpRequest req,
        string id)
    {
        var form = await req.ReadFromJsonAsync<Form>() ?? new Form();
        if (!Guid.TryParse(id, out var guid))
        {
            return new BadRequestResult();
        }
        form.Id = guid;
        var path = await _publisher.PublishAsync(form);
        await _repository.SaveAsync(form);
        await _logService.AddLogAsync(new ActivityLog { UserId = form.UserId, FormId = form.Id, ActionType = "PublishForm" });
        return new OkObjectResult(new { path });
    }

    [FunctionName("UnpublishForm")]
    public async Task<IActionResult> UnpublishForm(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "forms/{id}/draft")] HttpRequest req,
        string id)
    {
        if (!Guid.TryParse(id, out var guid))
        {
            return new BadRequestResult();
        }
        var form = await _repository.GetByIdAsync(guid);
        if (form is null)
        {
            return new NotFoundResult();
        }
        await _publisher.UnpublishAsync(form);
        await _repository.SaveAsync(form);
        await _logService.AddLogAsync(new ActivityLog { UserId = form.UserId, FormId = form.Id, ActionType = "UnpublishForm" });
        return new OkObjectResult(form);
    }

    [FunctionName("SubmitFormAnswers")]
    public async Task<IActionResult> SubmitFormAnswers(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "forms/{id}/answers")] HttpRequest req,
        string id)
    {
        if (!Guid.TryParse(id, out var guid))
        {
            return new BadRequestResult();
        }
        var data = await req.ReadFromJsonAsync<SubmissionRequest>() ?? new SubmissionRequest(new(), DateTime.UtcNow);
        await _answerService.SubmitAsync(guid, data.Answers, data.ConsentGivenAt);
        return new OkResult();
    }

    public record SubmissionRequest(Dictionary<string, string> Answers, DateTime ConsentGivenAt);

    public record EmailRequest(string To);
}
