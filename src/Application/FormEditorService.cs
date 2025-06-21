using System;
using System.Threading.Tasks;
using System.Collections.Generic;
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
