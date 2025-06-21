using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AstroForm.Domain.Entities;

namespace AstroForm.Domain.Repositories
{
    public interface IActivityLogRepository
    {
        Task<IReadOnlyList<ActivityLog>> GetLogsAsync(string? userId = null, Guid? formId = null);
        Task AddLogAsync(ActivityLog log);
    }
}
