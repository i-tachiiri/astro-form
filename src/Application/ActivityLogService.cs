using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AstroForm.Domain.Entities;
using AstroForm.Domain.Repositories;

namespace AstroForm.Application
{
    public class ActivityLogService
    {
        private readonly IActivityLogRepository _repository;

        public ActivityLogService(IActivityLogRepository repository)
        {
            _repository = repository;
        }

        public Task<IReadOnlyList<ActivityLog>> GetLogsAsync(string? userId = null, Guid? formId = null)
        {
            return _repository.GetLogsAsync(userId, formId);
        }

        public Task AddLogAsync(ActivityLog log)
        {
            log.Timestamp = DateTime.UtcNow;
            return _repository.AddLogAsync(log);
        }
    }
}
