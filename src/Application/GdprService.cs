using System.Collections.Concurrent;
using System.Threading.Tasks;
using AstroForm.Domain.Repositories;

namespace AstroForm.Application;

public class GdprService
{
    private readonly ConcurrentQueue<string> _deleteQueue = new();
    private readonly IUserRepository _users;
    private readonly IFormRepository _forms;

    public GdprService(IUserRepository users, IFormRepository forms)
    {
        _users = users;
        _forms = forms;
    }

    public void RequestUserDeletion(string userId)
    {
        _deleteQueue.Enqueue(userId);
    }

    public async Task ProcessDeletionQueueAsync()
    {
        while (_deleteQueue.TryDequeue(out var id))
        {
            await _users.DeleteAsync(id);
            await _forms.DeleteFormsByUserAsync(id);
        }
    }
}
