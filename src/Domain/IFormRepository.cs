using System;
using System.Threading.Tasks;
using AstroForm.Domain.Entities;

namespace AstroForm.Domain.Repositories
{
    public interface IFormRepository
    {
        Task<IReadOnlyList<Form>> GetAllAsync();
        Task<Form?> GetByIdAsync(Guid id);
        Task SaveAsync(Form form);
        Task DeleteFormAsync(Guid id);
        Task<IReadOnlyList<FormSubmission>> GetSubmissionsAsync(Guid formId);
        Task DeleteSubmissionAsync(Guid formId, Guid submissionId);
        Task DeleteFormsByUserAsync(string userId);
    }
}
