using System.Collections.Concurrent;
using System.Threading.Tasks;
using AstroForm.Domain.Entities;
using AstroForm.Domain.Repositories;

namespace AstroForm.Infra
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<string, User> _store = new();

        public Task<User?> GetByIdAsync(string id)
        {
            _store.TryGetValue(id, out var user);
            return Task.FromResult(user);
        }

        public Task SaveAsync(User user)
        {
            _store[user.Id] = user;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id)
        {
            _store.TryRemove(id, out _);
            return Task.CompletedTask;
        }
    }
}
