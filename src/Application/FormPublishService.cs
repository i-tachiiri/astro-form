using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AstroForm.Domain.Entities;

namespace AstroForm.Application
{
    public class FormPublishService
    {
        private readonly string _publicDir;
        private readonly string _previewDir;

        public FormPublishService(string publicDir, string previewDir)
        {
            _publicDir = publicDir;
            _previewDir = previewDir;
        }

        public async Task<string> GeneratePreviewAsync(Form form)
        {
            Directory.CreateDirectory(_previewDir);
            var path = Path.Combine(_previewDir, $"{form.Id}.html");
            var html = BuildHtml(form);
            await File.WriteAllTextAsync(path, html);
            return path;
        }

        public async Task<string> PublishAsync(Form form)
        {
            Directory.CreateDirectory(_publicDir);
            var path = Path.Combine(_publicDir, $"{form.Id}.html");
            var html = BuildHtml(form);
            await File.WriteAllTextAsync(path, html);
            form.Status = FormStatus.Published;
            return path;
        }

        private static string BuildHtml(Form form)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html><body>");
            sb.AppendLine($"<h1>{WebUtility.HtmlEncode(form.Name)}</h1>");
            if (!string.IsNullOrWhiteSpace(form.Description))
            {
                sb.AppendLine($"<p>{WebUtility.HtmlEncode(form.Description)}</p>");
            }
            sb.AppendLine("<form>");
            foreach (var item in form.FormItems.OrderBy(i => i.DisplayOrder))
            {
                var label = WebUtility.HtmlEncode(item.Label);
                var placeholder = WebUtility.HtmlEncode(item.Placeholder ?? string.Empty);
                sb.AppendLine($"<label>{label}<input name=\"{item.Id}\" placeholder=\"{placeholder}\" /></label><br/>");
            }
            sb.AppendLine("<button type=\"submit\">Submit</button>");
            sb.AppendLine("</form>");
            sb.AppendLine("</body></html>");
            return sb.ToString();
        }
    }
}
