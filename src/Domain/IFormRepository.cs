using System;
using System.Threading.Tasks;
using AstroForm.Domain.Entities;

namespace AstroForm.Domain.Repositories
{
    public interface IFormRepository
    {
        Task<Form?> GetByIdAsync(Guid id);
        Task SaveAsync(Form form);
        Task<IReadOnlyList<FormSubmission>> GetSubmissionsAsync(Guid formId);
        Task DeleteSubmissionAsync(Guid formId, Guid submissionId);
    }
}
