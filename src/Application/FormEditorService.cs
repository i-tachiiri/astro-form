using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Timer = System.Timers.Timer;
using AstroForm.Domain.Entities;
using AstroForm.Domain.Repositories;

namespace AstroForm.Application
{
    public class FormEditorService : IDisposable
    {
        private readonly IFormRepository _repository;
        private readonly Timer _autoSaveTimer;

        public Form CurrentForm { get; private set; }

        public TimeSpan AutoSaveInterval { get; }

        public FormEditorService(IFormRepository repository, TimeSpan? autoSaveInterval = null)
        {
            _repository = repository;
            AutoSaveInterval = autoSaveInterval ?? TimeSpan.FromSeconds(10);
            _autoSaveTimer = new Timer(AutoSaveInterval.TotalMilliseconds);
            _autoSaveTimer.AutoReset = false;
            _autoSaveTimer.Elapsed += async (_, _) => await AutoSaveAsync();

            CurrentForm = new Form
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = FormStatus.Draft
            };
            foreach (var item in CreateDefaultItems(CurrentForm.Id))
            {
                CurrentForm.FormItems.Add(item);
            }
        }

        public void Load(Form form)
        {
            CurrentForm = form;
        }

        public void UpdateName(string name)
        {
            CurrentForm.Name = name;
            RestartTimer();
        }

        public void UpdateDescription(string description)
        {
            CurrentForm.Description = description;
            RestartTimer();
        }

        public void UpdateNavigationText(string? text)
        {
            CurrentForm.NavigationText = text;
            RestartTimer();
        }

        public void UpdateThankYouPageUrl(string? url)
        {
            CurrentForm.ThankYouPageUrl = url;
            RestartTimer();
        }

        public FormItem AddItem(string type, string label, string? placeholder = null)
        {
            var item = new FormItem
            {
                Id = Guid.NewGuid(),
                FormId = CurrentForm.Id,
                Type = type,
                Label = label,
                Placeholder = placeholder,
                DisplayOrder = CurrentForm.FormItems.Count + 1,
                IsDefault = false
            };
            CurrentForm.FormItems.Add(item);
            RestartTimer();
            return item;
        }

        public void RemoveItem(Guid itemId)
        {
            var item = CurrentForm.FormItems.FirstOrDefault(i => i.Id == itemId);
            if (item == null || item.IsDefault)
            {
                return;
            }
            CurrentForm.FormItems.Remove(item);
            var order = 1;
            foreach (var i in CurrentForm.FormItems.OrderBy(i => i.DisplayOrder))
            {
                i.DisplayOrder = order++;
            }
            RestartTimer();
        }

        public void UpdateItem(Guid itemId, string label, string? placeholder = null)
        {
            var item = CurrentForm.FormItems.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
            {
                return;
            }
            item.Label = label;
            item.Placeholder = placeholder;
            RestartTimer();
        }

        public async Task BlurAsync()
        {
            await SaveAsync();
        }

        private void RestartTimer()
        {
            _autoSaveTimer.Stop();
            _autoSaveTimer.Interval = AutoSaveInterval.TotalMilliseconds;
            _autoSaveTimer.Start();
        }

        private async Task AutoSaveAsync()
        {
            await SaveAsync();
        }

        private static IEnumerable<FormItem> CreateDefaultItems(Guid formId)
        {
            return new[]
            {
                new FormItem { Id = Guid.NewGuid(), FormId = formId, Type = "text", Label = "氏", DisplayOrder = 1, IsDefault = true },
                new FormItem { Id = Guid.NewGuid(), FormId = formId, Type = "text", Label = "名", DisplayOrder = 2, IsDefault = true },
                new FormItem { Id = Guid.NewGuid(), FormId = formId, Type = "email", Label = "メールアドレス", DisplayOrder = 3, IsDefault = true },
                new FormItem { Id = Guid.NewGuid(), FormId = formId, Type = "date", Label = "生まれた年・月・日", DisplayOrder = 4, IsDefault = true },
                new FormItem { Id = Guid.NewGuid(), FormId = formId, Type = "time", Label = "生まれた時・分", DisplayOrder = 5, IsDefault = true },
                new FormItem { Id = Guid.NewGuid(), FormId = formId, Type = "text", Label = "生まれた国", DisplayOrder = 6, IsDefault = true },
                new FormItem { Id = Guid.NewGuid(), FormId = formId, Type = "text", Label = "生まれた都道府県", DisplayOrder = 7, IsDefault = true },
                new FormItem { Id = Guid.NewGuid(), FormId = formId, Type = "text", Label = "生まれた市町村", DisplayOrder = 8, IsDefault = true }
            };
        }

        public async Task SaveAsync()
        {
            CurrentForm.UpdatedAt = DateTime.UtcNow;
            await _repository.SaveAsync(CurrentForm);
        }

        public void Dispose()
        {
            _autoSaveTimer.Dispose();
        }
    }
}
