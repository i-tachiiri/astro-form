using System;
using System.Threading.Tasks;
using AstroForm.Application;
using AstroForm.Domain.Entities;
using AstroForm.Infra;
using Xunit;

namespace AstroForm.Tests
{
    public class FormEditorServiceTests
    {
        [Fact]
        public async Task ManualSave_PersistsForm()
        {
            var repo = new InMemoryFormRepository();
            using var service = new FormEditorService(repo, TimeSpan.FromMilliseconds(1));
            service.UpdateName("test");

            await service.SaveAsync();

            var saved = await repo.GetByIdAsync(service.CurrentForm.Id);
            Assert.Equal("test", saved?.Name);
        }

        [Fact]
        public async Task Blur_SavesImmediately()
        {
            var repo = new InMemoryFormRepository();
            using var service = new FormEditorService(repo, TimeSpan.FromSeconds(10));
            service.UpdateDescription("desc");

            await service.BlurAsync();

            var saved = await repo.GetByIdAsync(service.CurrentForm.Id);
            Assert.Equal("desc", saved?.Description);
        }

        [Fact]
        public async Task AutoSave_AfterInterval()
        {
            var repo = new InMemoryFormRepository();
            using var service = new FormEditorService(repo, TimeSpan.FromMilliseconds(200));
            service.UpdateName("auto");

            await Task.Delay(300);

            var saved = await repo.GetByIdAsync(service.CurrentForm.Id);
            Assert.Equal("auto", saved?.Name);
        }

        [Fact]
        public void Constructor_AddsDefaultItems()
        {
            var repo = new InMemoryFormRepository();
            using var service = new FormEditorService(repo);

            Assert.Equal(8, service.CurrentForm.FormItems.Count);
            Assert.All(service.CurrentForm.FormItems, i => Assert.True(i.IsDefault));
        }
    }
}
