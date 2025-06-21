using AstroForm.Domain.Entities;
using AstroForm.Domain.Repositories;
using AstroForm.Infra;
using AstroForm.Application;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IFormRepository, InMemoryFormRepository>();
builder.Services.AddSingleton(sp =>
{
    var env = sp.GetRequiredService<IHostEnvironment>();
    var publicDir = Path.Combine(env.ContentRootPath, "public");
    var previewDir = Path.Combine(env.ContentRootPath, "preview");
    return new FormPublishService(publicDir, previewDir);
});

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

app.MapPost("/forms/{id}/save", async (Guid id, Form form, IFormRepository repo) =>
{
    form.Id = id;
    await repo.SaveAsync(form);
    return Results.Ok(form);
});

app.MapPost("/forms/{id}/preview", async (Guid id, Form form, FormPublishService publisher) =>
{
    form.Id = id;
    var path = await publisher.GeneratePreviewAsync(form);
    return Results.Ok(new { path });
});

app.MapPost("/forms/{id}/publish", async (Guid id, Form form, IFormRepository repo, FormPublishService publisher) =>
{
    form.Id = id;
    var path = await publisher.PublishAsync(form);
    await repo.SaveAsync(form);
    return Results.Ok(new { path });
});

app.Run();
