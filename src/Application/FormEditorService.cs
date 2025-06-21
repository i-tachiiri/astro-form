using System;
using System.Threading.Tasks;
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

            CurrentForm = new Form { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, Status = FormStatus.Draft };
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
