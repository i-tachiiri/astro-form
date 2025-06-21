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
            form.FormSubmissions.Add(new FormSubmission { Id = Guid.NewGuid(), FormId = form.Id, Answers = "ans" });
            await repo.SaveAsync(form);

            var email = new InMemoryEmailService();
            var service = new FormAnswerService(repo, email);
            var list = await service.GetSubmissionsAsync(form.Id);

            Assert.Single(list);
            Assert.Equal("ans", list[0].Answers);
        }
    }
}
