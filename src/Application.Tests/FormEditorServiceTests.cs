using System;
using System.Linq;
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

        [Fact]
        public void AddItem_AppendsNewItem()
        {
            var repo = new InMemoryFormRepository();
            using var service = new FormEditorService(repo);

            var item = service.AddItem("text", "new");

            Assert.Contains(item, service.CurrentForm.FormItems);
            Assert.Equal(9, item.DisplayOrder);
            Assert.False(item.IsDefault);
        }

        [Fact]
        public void RemoveItem_DeletesItem()
        {
            var repo = new InMemoryFormRepository();
            using var service = new FormEditorService(repo);

            var item = service.AddItem("text", "temp");
            service.RemoveItem(item.Id);

            Assert.DoesNotContain(service.CurrentForm.FormItems, i => i.Id == item.Id);
        }

        [Fact]
        public void UpdateItem_ChangesValues()
        {
            var repo = new InMemoryFormRepository();
            using var service = new FormEditorService(repo);

            var item = service.AddItem("text", "old");
            service.UpdateItem(item.Id, "new", "ph");

            var updated = service.CurrentForm.FormItems.First(i => i.Id == item.Id);
            Assert.Equal("new", updated.Label);
            Assert.Equal("ph", updated.Placeholder);
        }

        [Fact]
        public async Task UpdateNavigationText_SavesValue()
        {
            var repo = new InMemoryFormRepository();
            using var service = new FormEditorService(repo);

            service.UpdateNavigationText("next");
            await service.SaveAsync();

            var saved = await repo.GetByIdAsync(service.CurrentForm.Id);
            Assert.Equal("next", saved?.NavigationText);
        }

        [Fact]
        public async Task UpdateThankYouPageUrl_SavesValue()
        {
            var repo = new InMemoryFormRepository();
            using var service = new FormEditorService(repo);

            service.UpdateThankYouPageUrl("https://example.com/thanks");
            await service.SaveAsync();

            var saved = await repo.GetByIdAsync(service.CurrentForm.Id);
            Assert.Equal("https://example.com/thanks", saved?.ThankYouPageUrl);
        }
    }
}
