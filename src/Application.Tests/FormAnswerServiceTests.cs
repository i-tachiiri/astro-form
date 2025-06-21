using System;
using System.Threading.Tasks;
using AstroForm.Application;
using AstroForm.Domain.Entities;
using AstroForm.Infra;
using Xunit;

namespace AstroForm.Tests
{
    public class FormAnswerServiceTests
    {
        [Fact]
        public async Task GetSubmissions_ReturnsSavedSubmissions()
        {
            var repo = new InMemoryFormRepository();
            var form = new Form { Id = Guid.NewGuid(), Status = FormStatus.Draft };
            form.FormSubmissions.Add(new FormSubmission
            {
                Id = Guid.NewGuid(),
                FormId = form.Id,
                Answers = "ans",
                SubmittedAt = DateTime.UtcNow,
                ConsentGivenAt = DateTime.UtcNow
            });
            await repo.SaveAsync(form);

            var email = new InMemoryEmailService();
            var users = new InMemoryUserRepository();
            var service = new FormAnswerService(repo, email, users);
            var list = await service.GetSubmissionsAsync(form.Id);

            Assert.Single(list);
            Assert.Equal("ans", list[0].Answers);
        }

        [Fact]
        public async Task DeleteSubmission_RemovesSubmission()
        {
            var repo = new InMemoryFormRepository();
            var form = new Form { Id = Guid.NewGuid(), Status = FormStatus.Draft };
            var sub = new FormSubmission
            {
                Id = Guid.NewGuid(),
                FormId = form.Id,
                Answers = "ans",
                SubmittedAt = DateTime.UtcNow,
                ConsentGivenAt = DateTime.UtcNow
            };
            form.FormSubmissions.Add(sub);
            await repo.SaveAsync(form);

            var email = new InMemoryEmailService();
            var users = new InMemoryUserRepository();
            var service = new FormAnswerService(repo, email, users);
            await service.DeleteSubmissionAsync(form.Id, sub.Id);

            var list = await service.GetSubmissionsAsync(form.Id);
            Assert.Empty(list);
        }

        [Fact]
        public async Task SubmitAsync_SendsEmail()
        {
            var repo = new InMemoryFormRepository();
            var email = new InMemoryEmailService();
            var users = new InMemoryUserRepository();
            var user = new User { Id = "u1", Email = "owner@example.com", DisplayName = "Owner", Role = UserRole.FortuneTeller, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, ConsentGivenAt = DateTime.UtcNow };
            await users.SaveAsync(user);
            var form = new Form { Id = Guid.NewGuid(), UserId = user.Id, Status = FormStatus.Draft };
            await repo.SaveAsync(form);

            var service = new FormAnswerService(repo, email, users);
            await service.SubmitAsync(form.Id, new Dictionary<string, string>(), DateTime.UtcNow);

            Assert.Single(email.Messages);
            Assert.Equal("owner@example.com", email.Messages[0].To);
        }

        [Fact]
        public async Task PurgeOldSubmissions_RemovesOldEntries()
        {
            var repo = new InMemoryFormRepository();
            var email = new InMemoryEmailService();
            var users = new InMemoryUserRepository();
            var form = new Form { Id = Guid.NewGuid(), Status = FormStatus.Draft };
            form.FormSubmissions.Add(new FormSubmission { Id = Guid.NewGuid(), FormId = form.Id, Answers = "a", SubmittedAt = DateTime.UtcNow.AddDays(-10), ConsentGivenAt = DateTime.UtcNow.AddDays(-10) });
            form.FormSubmissions.Add(new FormSubmission { Id = Guid.NewGuid(), FormId = form.Id, Answers = "b", SubmittedAt = DateTime.UtcNow, ConsentGivenAt = DateTime.UtcNow });
            await repo.SaveAsync(form);

            var service = new FormAnswerService(repo, email, users);
            await service.PurgeOldSubmissionsAsync(form.Id, TimeSpan.FromDays(7));

            var list = await service.GetSubmissionsAsync(form.Id);
            Assert.Single(list);
        }
    }
}
