using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AstroForm.Domain.Entities;
using AstroForm.Domain.Repositories;
using AstroForm.Domain.Security;

namespace AstroForm.Infra
{
    public class EncryptedFormRepository : IFormRepository
    {
        private readonly IFormRepository _inner;
        private readonly IEncryptionService _encryption;

        public EncryptedFormRepository(IFormRepository inner, IEncryptionService encryption)
        {
            _inner = inner;
            _encryption = encryption;
        }

        public async Task<Form?> GetByIdAsync(Guid id)
        {
            var form = await _inner.GetByIdAsync(id);
            if (form != null)
            {
                foreach (var sub in form.FormSubmissions)
                {
                    if (!string.IsNullOrEmpty(sub.Answers))
                    {
                        sub.Answers = _encryption.Decrypt(sub.Answers);
                    }
                }
            }
            return form;
        }

        public async Task<IReadOnlyList<Form>> GetAllAsync()
        {
            var list = await _inner.GetAllAsync();
            foreach (var form in list)
            {
                foreach (var sub in form.FormSubmissions)
                {
                    if (!string.IsNullOrEmpty(sub.Answers))
                    {
                        sub.Answers = _encryption.Decrypt(sub.Answers);
                    }
                }
            }
            return list;
        }

        public async Task SaveAsync(Form form)
        {
            foreach (var sub in form.FormSubmissions)
            {
                if (!string.IsNullOrEmpty(sub.Answers))
                {
                    sub.Answers = _encryption.Encrypt(sub.Answers);
                }
            }
            await _inner.SaveAsync(form);
        }

        public async Task<IReadOnlyList<FormSubmission>> GetSubmissionsAsync(Guid formId)
        {
            var list = await _inner.GetSubmissionsAsync(formId);
            foreach (var sub in list)
            {
                if (!string.IsNullOrEmpty(sub.Answers))
                {
                    sub.Answers = _encryption.Decrypt(sub.Answers);
                }
            }
            return list;
        }

        public Task DeleteSubmissionAsync(Guid formId, Guid submissionId)
        {
            return _inner.DeleteSubmissionAsync(formId, submissionId);
        }

        public Task DeleteFormAsync(Guid id)
        {
            return _inner.DeleteFormAsync(id);
        }
    }
}
