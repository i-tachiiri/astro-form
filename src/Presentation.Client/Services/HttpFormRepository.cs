using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AstroForm.Domain.Entities;
using AstroForm.Domain.Repositories;

namespace Presentation.Client.Services;

public class HttpFormRepository : IFormRepository
{
    private readonly HttpClient _http;

    public HttpFormRepository(HttpClient http)
    {
        _http = http;
    }

    public Task<IReadOnlyList<Form>> GetAllAsync()
    {
        return _http.GetFromJsonAsync<IReadOnlyList<Form>>("forms")!;
    }

    public Task<Form?> GetByIdAsync(Guid id)
    {
        return _http.GetFromJsonAsync<Form>($"forms/{id}");
    }

    public async Task SaveAsync(Form form)
    {
        await _http.PostAsJsonAsync($"forms/{form.Id}/save", form);
    }

    public Task<IReadOnlyList<FormSubmission>> GetSubmissionsAsync(Guid formId)
    {
        return _http.GetFromJsonAsync<IReadOnlyList<FormSubmission>>($"forms/{formId}/answers")!;
    }

    public async Task DeleteSubmissionAsync(Guid formId, Guid submissionId)
    {
        await _http.DeleteAsync($"forms/{formId}/answers/{submissionId}");
    }
}
