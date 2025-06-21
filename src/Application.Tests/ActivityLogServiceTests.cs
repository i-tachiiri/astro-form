using System;
using System.Threading.Tasks;
using AstroForm.Application;
using AstroForm.Domain.Entities;
using AstroForm.Infra;
using Xunit;

namespace AstroForm.Tests
{
    public class ActivityLogServiceTests
    {
        [Fact]
        public async Task AddAndRetrieve_Log()
        {
            var repo = new InMemoryActivityLogRepository();
            var service = new ActivityLogService(repo);
            var log = new ActivityLog { ActionType = "TEST", Details = "detail", UserId = "u1" };

            await service.AddLogAsync(log);
            var logs = await service.GetLogsAsync("u1", null);

            Assert.Single(logs);
            Assert.Equal("TEST", logs[0].ActionType);
        }
    }
}
