using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AstroForm.Domain.Entities;
using AstroForm.Domain.Repositories;
using AstroForm.Domain.Services;

namespace AstroForm.Application
{
    public class FormAnswerService
    {
        private readonly IFormRepository _repository;
        private readonly IEmailService _email;
        private readonly IUserRepository _users;

        public FormAnswerService(IFormRepository repository, IEmailService email, IUserRepository users)
        {
            _repository = repository;
            _email = email;
            _users = users;
        }

        public async Task SubmitAsync(Guid formId, Dictionary<string, string> answers, DateTime consentGivenAt)
        {
            var form = await _repository.GetByIdAsync(formId) ?? throw new InvalidOperationException("Form not found");
            var submission = new FormSubmission
            {
                Id = Guid.NewGuid(),
                FormId = formId,
                Answers = JsonSerializer.Serialize(answers),
                SubmittedAt = DateTime.UtcNow,
                ConsentGivenAt = consentGivenAt
            };
            form.FormSubmissions.Add(submission);
            await _repository.SaveAsync(form);
            var user = await _users.GetByIdAsync(form.UserId);
            if (user != null)
            {
                await SendSubmissionEmailAsync(formId, submission.Id, user.Email);
            }
        }

        public async Task<IReadOnlyList<FormSubmission>> GetSubmissionsAsync(Guid formId)
        {
            var form = await _repository.GetByIdAsync(formId);
            return form?.FormSubmissions.ToList() ?? new List<FormSubmission>();
        }

        public async Task<string> ExportCsvAsync(Guid formId)
        {
            var form = await _repository.GetByIdAsync(formId) ?? throw new InvalidOperationException("Form not found");
            var items = form.FormItems.OrderBy(i => i.DisplayOrder).ToList();
            var sb = new StringBuilder();
            sb.Append("SubmittedAt");
            foreach (var item in items)
            {
                sb.Append(',').Append(Escape(item.Label));
            }
            sb.AppendLine();

            foreach (var sub in form.FormSubmissions.OrderBy(s => s.SubmittedAt))
            {
                sb.Append(sub.SubmittedAt.ToString("o"));
                var answers = JsonSerializer.Deserialize<Dictionary<string, string>>(sub.Answers) ?? new();
                foreach (var item in items)
                {
                    answers.TryGetValue(item.Id.ToString(), out var value);
                    sb.Append(',').Append(Escape(value));
                }
                sb.AppendLine();
            }

            return sb.ToString();

            static string Escape(string? value)
            {
                if (string.IsNullOrEmpty(value))
                {
                    return string.Empty;
                }
                if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
                {
                    return $"\"{value.Replace("\"", "\"\"")}\"";
                }
                return value;
            }
        }

        public Task DeleteSubmissionAsync(Guid formId, Guid submissionId)
        {
            return _repository.DeleteSubmissionAsync(formId, submissionId);
        }

        public async Task PurgeOldSubmissionsAsync(Guid formId, TimeSpan retention)
        {
            var form = await _repository.GetByIdAsync(formId) ?? throw new InvalidOperationException("Form not found");
            var threshold = DateTime.UtcNow - retention;
            var toRemove = form.FormSubmissions.Where(s => s.SubmittedAt < threshold).ToList();
            foreach (var sub in toRemove)
            {
                form.FormSubmissions.Remove(sub);
            }
            await _repository.SaveAsync(form);
        }

        public async Task SendSubmissionEmailAsync(Guid formId, Guid submissionId, string to)
        {
            var form = await _repository.GetByIdAsync(formId) ?? throw new InvalidOperationException("Form not found");
            var submission = form.FormSubmissions.FirstOrDefault(s => s.Id == submissionId) ?? throw new InvalidOperationException("Submission not found");
            var items = form.FormItems.OrderBy(i => i.DisplayOrder).ToList();
            var answers = JsonSerializer.Deserialize<Dictionary<string, string>>(submission.Answers) ?? new();

            var body = new StringBuilder();
            body.AppendLine($"<h1>{WebUtility.HtmlEncode(form.Name)}</h1>");
            body.AppendLine("<table>");
            foreach (var item in items)
            {
                answers.TryGetValue(item.Id.ToString(), out var value);
                body.AppendLine($"<tr><th>{WebUtility.HtmlEncode(item.Label)}</th><td>{WebUtility.HtmlEncode(value)}</td></tr>");
            }
            body.AppendLine("</table>");

            await _email.SendHtmlEmailAsync(to, $"{form.Name} submission", body.ToString());
        }
    }
}
