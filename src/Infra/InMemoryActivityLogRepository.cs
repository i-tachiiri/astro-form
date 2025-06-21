using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AstroForm.Domain.Entities;
using AstroForm.Domain.Repositories;

namespace AstroForm.Infra
{
    public class InMemoryActivityLogRepository : IActivityLogRepository
    {
        private readonly ConcurrentBag<ActivityLog> _logs = new();

        public Task AddLogAsync(ActivityLog log)
        {
            _logs.Add(log);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<ActivityLog>> GetLogsAsync(string? userId = null, Guid? formId = null)
        {
            IEnumerable<ActivityLog> query = _logs;
            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(l => l.UserId == userId);
            }
            if (formId.HasValue)
            {
                query = query.Where(l => l.FormId == formId);
            }
            return Task.FromResult((IReadOnlyList<ActivityLog>)query.OrderByDescending(l => l.Timestamp).ToList());
        }
    }
}
