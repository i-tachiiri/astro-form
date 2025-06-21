using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AstroForm.Domain.Entities;
using AstroForm.Domain.Repositories;

namespace AstroForm.Infra
{
    public class InMemoryFormRepository : IFormRepository
    {
        private readonly ConcurrentDictionary<Guid, Form> _store = new();

        public Task<IReadOnlyList<Form>> GetAllAsync()
        {
            return Task.FromResult((IReadOnlyList<Form>)_store.Values.ToList());
        }

        public Task<Form?> GetByIdAsync(Guid id)
        {
            _store.TryGetValue(id, out var form);
            return Task.FromResult(form);
        }

        public Task SaveAsync(Form form)
        {
            _store[form.Id] = form;
            return Task.CompletedTask;
        }

        public Task DeleteFormAsync(Guid id)
        {
            _store.TryRemove(id, out _);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<FormSubmission>> GetSubmissionsAsync(Guid formId)
        {
            if (_store.TryGetValue(formId, out var form))
            {
                return Task.FromResult((IReadOnlyList<FormSubmission>)form.FormSubmissions.ToList());
            }
            return Task.FromResult((IReadOnlyList<FormSubmission>)new List<FormSubmission>());
        }

        public Task DeleteSubmissionAsync(Guid formId, Guid submissionId)
        {
            if (_store.TryGetValue(formId, out var form))
            {
                var sub = form.FormSubmissions.FirstOrDefault(s => s.Id == submissionId);
                if (sub != null)
                {
                    form.FormSubmissions.Remove(sub);
                }
            }
            return Task.CompletedTask;
        }

        public Task DeleteFormsByUserAsync(string userId)
        {
            foreach (var form in _store.Values.Where(f => f.UserId == userId).ToList())
            {
                _store.TryRemove(form.Id, out _);
            }
            return Task.CompletedTask;
        }
    }
}
