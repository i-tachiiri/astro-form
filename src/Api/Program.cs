using AstroForm.Domain.Entities;
using AstroForm.Domain.Repositories;
using AstroForm.Infra;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IFormRepository, InMemoryFormRepository>();

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

app.Run();
