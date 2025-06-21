using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using AstroForm.Domain.Entities;
using AstroForm.Domain.Repositories;

namespace AstroForm.Infra
{
    public class InMemoryFormRepository : IFormRepository
    {
        private readonly ConcurrentDictionary<Guid, Form> _store = new();

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
    }
}
