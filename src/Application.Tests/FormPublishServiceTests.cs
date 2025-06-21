using System;
using System.IO;
using System.Threading.Tasks;
using AstroForm.Application;
using AstroForm.Domain.Entities;
using Xunit;

namespace AstroForm.Tests
{
    public class FormPublishServiceTests
    {
        private static Form CreateSampleForm()
        {
            var form = new Form
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                Description = "desc",
                Status = FormStatus.Draft
            };
            form.FormItems.Add(new FormItem
            {
                Id = Guid.NewGuid(),
                FormId = form.Id,
                Type = "text",
                Label = "Name",
                DisplayOrder = 1,
                IsDefault = true
            });
            return form;
        }

        [Fact]
        public async Task GeneratePreview_WritesFile()
        {
            var previewDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var service = new FormPublishService("unused", previewDir);
            var form = CreateSampleForm();

            var path = await service.GeneratePreviewAsync(form);

            Assert.True(File.Exists(path));
            var html = await File.ReadAllTextAsync(path);
            Assert.Contains("Test", html);
            Assert.Contains("localStorage.setItem", html);
        }

        [Fact]
        public async Task Publish_WritesFileAndUpdatesStatus()
        {
            var publishDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var service = new FormPublishService(publishDir, "unused");
            var form = CreateSampleForm();

            var path = await service.PublishAsync(form);

            Assert.True(File.Exists(path));
            Assert.Equal(FormStatus.Published, form.Status);
        }

        [Fact]
        public async Task Unpublish_DeletesFileAndSetsDraft()
        {
            var publishDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var service = new FormPublishService(publishDir, "unused");
            var form = CreateSampleForm();

            var path = await service.PublishAsync(form);
            Assert.True(File.Exists(path));

            await service.UnpublishAsync(form);
            Assert.False(File.Exists(path));
            Assert.Equal(FormStatus.Draft, form.Status);
        }
    }
}
