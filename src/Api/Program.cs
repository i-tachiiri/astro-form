using AstroForm.Domain.Entities;
using AstroForm.Domain.Repositories;
using AstroForm.Domain.Security;
using AstroForm.Infra;
using AstroForm.Application;
using AstroForm.Domain.Services;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Presentation.Shared;
using System.Collections.Generic;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) =>
    lc.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddSingleton<InMemoryFormRepository>();
builder.Services.AddSingleton<IEncryptionService>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var base64 = cfg["EncryptionKey"] ?? throw new InvalidOperationException("EncryptionKey not configured");
    var key = Convert.FromBase64String(base64);
    return new AesEncryptionService(key);
});
builder.Services.AddSingleton<IFormRepository>(sp =>
    new EncryptedFormRepository(
        sp.GetRequiredService<InMemoryFormRepository>(),
        sp.GetRequiredService<IEncryptionService>()));
builder.Services.AddSingleton<IActivityLogRepository, InMemoryActivityLogRepository>();
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<IEmailService, InMemoryEmailService>();
builder.Services.AddSingleton(sp =>
{
    var env = sp.GetRequiredService<IHostEnvironment>();
    var publicDir = Path.Combine(env.ContentRootPath, "public");
    var previewDir = Path.Combine(env.ContentRootPath, "preview");
    return new FormPublishService(publicDir, previewDir);
});
builder.Services.AddSingleton<FormAnswerService>();
builder.Services.AddSingleton<ActivityLogService>();
builder.Services.AddSingleton<UserService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["AzureAd:Authority"];
        options.Audience = builder.Configuration["AzureAd:ClientId"];
    });
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/forms", async (IFormRepository repo) =>
{
    var forms = await repo.GetAllAsync();
    return Results.Ok(forms.Select(f => new FormDto(f.Id, f.Name)));
});

app.MapGet("/forms/{id}", async (Guid id, IFormRepository repo) =>
{
    var form = await repo.GetByIdAsync(id);
    return form is null ? Results.NotFound() : Results.Ok(form);
});

app.MapGet("/forms/{id}/answers", async (Guid id, FormAnswerService service) =>
{
    var answers = await service.GetSubmissionsAsync(id);
    return answers.Count == 0 ? Results.NotFound() : Results.Ok(answers);
});

app.MapGet("/forms/{id}/answers/csv", async (Guid id, FormAnswerService service) =>
{
    var csv = await service.ExportCsvAsync(id);
    return Results.File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", $"{id}.csv");
});

app.MapPost("/forms/{formId}/answers/{submissionId}/email", async (Guid formId, Guid submissionId, EmailRequest req, FormAnswerService service) =>
{
    await service.SendSubmissionEmailAsync(formId, submissionId, req.To);
    return Results.Ok();
});

app.MapDelete("/forms/{formId}/answers/{submissionId}", async (Guid formId, Guid submissionId, FormAnswerService service, ActivityLogService logs) =>
{
    await service.DeleteSubmissionAsync(formId, submissionId);
    await logs.AddLogAsync(new ActivityLog { FormId = formId, ActionType = "DeleteSubmission" });
    return Results.Ok();
});

app.MapPost("/forms/{id}/answers", async (Guid id, SubmissionRequest req, FormAnswerService service) =>
{
    await service.SubmitAsync(id, req.Answers, req.ConsentGivenAt);
    return Results.Ok();
});

app.MapPost("/forms/{id}/save", async (Guid id, Form form, IFormRepository repo, ActivityLogService logs) =>
{
    form.Id = id;
    await repo.SaveAsync(form);
    await logs.AddLogAsync(new ActivityLog { UserId = form.UserId, FormId = form.Id, ActionType = "SaveForm" });
    return Results.Ok(form);
});

app.MapPost("/forms/{id}/preview", async (Guid id, Form form, FormPublishService publisher) =>
{
    form.Id = id;
    var path = await publisher.GeneratePreviewAsync(form);
    return Results.Ok(new { path });
});

app.MapPost("/forms/{id}/publish", async (Guid id, Form form, IFormRepository repo, FormPublishService publisher, ActivityLogService logs) =>
{
    form.Id = id;
    var path = await publisher.PublishAsync(form);
    await repo.SaveAsync(form);
    await logs.AddLogAsync(new ActivityLog { UserId = form.UserId, FormId = form.Id, ActionType = "PublishForm" });
    return Results.Ok(new { path });
});

app.MapGet("/logs", async (string? userId, Guid? formId, ActivityLogService service) =>
{
    var logs = await service.GetLogsAsync(userId, formId);
    return Results.Ok(logs);
});

app.MapGet("/logviewer", async context =>
{
    var env = context.RequestServices.GetRequiredService<IHostEnvironment>();
    var path = Path.Combine(env.ContentRootPath, "LogViewer.html");
    await context.Response.SendFileAsync(path);
});

app.MapPost("/users/register", async (UserRegistration req, ClaimsPrincipal principal, UserService service) =>
{
    var oid = principal.FindFirstValue("oid") ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);
    if (oid is null || oid != req.Id)
    {
        return Results.Unauthorized();
    }
    var user = await service.RegisterAsync(req.Id, req.DisplayName, req.Email, req.ConsentGivenAt);
    return Results.Ok(user);
}).RequireAuthorization();
app.MapPost("/users/{id}/role", async (string id, RoleUpdate req, UserService service) =>
{
    await service.UpdateRoleAsync(id, req.Role);
    return Results.Ok();
});

app.Run();

record UserRegistration(string Id, string DisplayName, string Email, DateTime ConsentGivenAt);
record RoleUpdate(UserRole Role);
record SubmissionRequest(Dictionary<string, string> Answers, DateTime ConsentGivenAt);
record EmailRequest(string To);

public partial class Program { }

