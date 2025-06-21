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
            var service = new FormAnswerService(repo, email);
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
            var service = new FormAnswerService(repo, email);
            await service.DeleteSubmissionAsync(form.Id, sub.Id);

            var list = await service.GetSubmissionsAsync(form.Id);
            Assert.Empty(list);
        }
    }
}
