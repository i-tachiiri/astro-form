using AstroForm.Domain.Entities;
using AstroForm.Domain.Repositories;
using AstroForm.Domain.Security;
using AstroForm.Infra;
using AstroForm.Application;
using AstroForm.Domain.Services;
using System.IO;
using Microsoft.AspNetCore.Http;
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

app.MapPost("/users/register", async (UserRegistration req, UserService service) =>
{
    var user = await service.RegisterAsync(req.Id, req.DisplayName, req.Email);
    return Results.Ok(user);
});
app.MapPost("/users/{id}/role", async (string id, RoleUpdate req, UserService service) =>
{
    await service.UpdateRoleAsync(id, req.Role);
    return Results.Ok();
});

app.Run();

record UserRegistration(string Id, string DisplayName, string Email);
record RoleUpdate(UserRole Role);
record EmailRequest(string To);

public partial class Program { }

