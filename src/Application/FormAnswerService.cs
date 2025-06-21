using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AstroForm.Domain.Entities;
using AstroForm.Domain.Repositories;

namespace AstroForm.Application
{
    public class FormAnswerService
    {
        private readonly IFormRepository _repository;

        public FormAnswerService(IFormRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<FormSubmission>> GetSubmissionsAsync(Guid formId)
        {
            var form = await _repository.GetByIdAsync(formId);
            return form?.FormSubmissions.ToList() ?? new List<FormSubmission>();
        }
    }
}
