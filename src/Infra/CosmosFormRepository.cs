using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AstroForm.Domain.Entities;
using AstroForm.Domain.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace AstroForm.Infra;

public class CosmosFormRepository : IFormRepository
{
    private readonly Container _forms;
    private readonly Container _submissions;

    public CosmosFormRepository(CosmosClient client, string database)
    {
        var db = client.GetDatabase(database);
        _forms = db.GetContainer("Forms");
        _submissions = db.GetContainer("FormSubmissions");
    }

    public async Task<IReadOnlyList<Form>> GetAllAsync()
    {
        var query = _forms.GetItemLinqQueryable<Form>().ToFeedIterator();
        var list = new List<Form>();
        while (query.HasMoreResults)
        {
            foreach (var item in await query.ReadNextAsync())
            {
                list.Add(item);
            }
        }
        return list;
    }

    public async Task<Form?> GetByIdAsync(Guid id)
    {
        try
        {
            var resp = await _forms.ReadItemAsync<Form>(id.ToString(), new PartitionKey(id.ToString()));
            return resp.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task SaveAsync(Form form)
    {
        await _forms.UpsertItemAsync(form, new PartitionKey(form.Id.ToString()));
    }

    public async Task DeleteFormAsync(Guid id)
    {
        await _forms.DeleteItemAsync<Form>(id.ToString(), new PartitionKey(id.ToString()));
    }

    public async Task<IReadOnlyList<FormSubmission>> GetSubmissionsAsync(Guid formId)
    {
        var query = _submissions.GetItemLinqQueryable<FormSubmission>()
            .Where(s => s.FormId == formId)
            .ToFeedIterator();
        var list = new List<FormSubmission>();
        while (query.HasMoreResults)
        {
            foreach (var item in await query.ReadNextAsync())
            {
                list.Add(item);
            }
        }
        return list;
    }

    public async Task DeleteSubmissionAsync(Guid formId, Guid submissionId)
    {
        await _submissions.DeleteItemAsync<FormSubmission>(submissionId.ToString(), new PartitionKey(formId.ToString()));
    }

    public async Task DeleteFormsByUserAsync(string userId)
    {
        var query = _forms.GetItemLinqQueryable<Form>().Where(f => f.UserId == userId).ToFeedIterator();
        while (query.HasMoreResults)
        {
            foreach (var item in await query.ReadNextAsync())
            {
                await DeleteFormAsync(item.Id);
            }
        }
    }
}
