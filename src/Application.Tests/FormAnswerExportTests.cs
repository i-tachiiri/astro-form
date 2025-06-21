using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using AstroForm.Application;
using AstroForm.Domain.Entities;
using AstroForm.Infra;
using Xunit;

namespace AstroForm.Tests
{
    public class FormAnswerExportTests
    {
        [Fact]
        public async Task ExportCsv_BuildsCsv()
        {
            var repo = new InMemoryFormRepository();
            var email = new InMemoryEmailService();
            var users = new InMemoryUserRepository();
            var form = new Form { Id = Guid.NewGuid(), Name = "Test", Status = FormStatus.Draft };
            var item = new FormItem { Id = Guid.NewGuid(), FormId = form.Id, Type = "text", Label = "Name", DisplayOrder = 1, IsDefault = true };
            form.FormItems.Add(item);
            form.FormSubmissions.Add(new FormSubmission
            {
                Id = Guid.NewGuid(),
                FormId = form.Id,
                Answers = JsonSerializer.Serialize(new Dictionary<string, string> { { item.Id.ToString(), "Taro" } }),
                SubmittedAt = DateTime.UtcNow
            });
            await repo.SaveAsync(form);

            var service = new FormAnswerService(repo, email, users);
            var csv = await service.ExportCsvAsync(form.Id);

            Assert.Contains("Name", csv);
            Assert.Contains("Taro", csv);
        }

        [Fact]
        public async Task SendSubmissionEmail_SendsEmail()
        {
            var repo = new InMemoryFormRepository();
            var email = new InMemoryEmailService();
            var users = new InMemoryUserRepository();
            var form = new Form { Id = Guid.NewGuid(), Name = "Test", Status = FormStatus.Draft };
            var item = new FormItem { Id = Guid.NewGuid(), FormId = form.Id, Type = "text", Label = "Name", DisplayOrder = 1, IsDefault = true };
            form.FormItems.Add(item);
            var submission = new FormSubmission
            {
                Id = Guid.NewGuid(),
                FormId = form.Id,
                Answers = JsonSerializer.Serialize(new Dictionary<string, string> { { item.Id.ToString(), "Taro" } }),
                SubmittedAt = DateTime.UtcNow
            };
            form.FormSubmissions.Add(submission);
            await repo.SaveAsync(form);

            var service = new FormAnswerService(repo, email, users);
            await service.SendSubmissionEmailAsync(form.Id, submission.Id, "to@example.com");

            Assert.Single(email.Messages);
            Assert.Equal("to@example.com", email.Messages[0].To);
            Assert.Contains("Taro", email.Messages[0].HtmlBody);
        }
    }
}
