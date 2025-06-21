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

        public Task DeletePreviewAsync(Guid formId)
        {
            var path = Path.Combine(_previewDir, $"{formId}.html");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            return Task.CompletedTask;
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

        public Task UnpublishAsync(Form form)
        {
            var path = Path.Combine(_publicDir, $"{form.Id}.html");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            form.Status = FormStatus.Draft;
            return Task.CompletedTask;
        }

        private static string BuildHtml(Form form)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html><body>");
            sb.AppendLine($"<h1>{WebUtility.HtmlEncode(form.Name)}</h1>");
            if (!string.IsNullOrWhiteSpace(form.NavigationText))
            {
                sb.AppendLine($"<nav>{WebUtility.HtmlEncode(form.NavigationText)}</nav>");
            }
            if (!string.IsNullOrWhiteSpace(form.Description))
            {
                sb.AppendLine($"<p>{WebUtility.HtmlEncode(form.Description)}</p>");
            }
            sb.AppendLine("<form>");
            var hasPlace = false;
            foreach (var item in form.FormItems.OrderBy(i => i.DisplayOrder))
            {
                var label = WebUtility.HtmlEncode(item.Label);
                var placeholder = WebUtility.HtmlEncode(item.Placeholder ?? string.Empty);
                var type = item.Type switch
                {
                    "date" => "date",
                    "time" => "time",
                    "place" => "text",
                    _ => "text"
                };
                var cls = item.Type == "place" ? " class=\"place-autocomplete\"" : string.Empty;
                if (item.Type == "place") hasPlace = true;
                sb.AppendLine($"<label>{label}<input type=\"{type}\" name=\"{item.Id}\" placeholder=\"{placeholder}\"{cls} /></label><br/>");
            }
            sb.AppendLine("<button type=\"submit\">Submit</button>");
            sb.AppendLine("</form>");

            if (hasPlace)
            {
                sb.AppendLine("<script src=\"https://maps.googleapis.com/maps/api/js?key=YOUR_API_KEY&libraries=places\"></script>");
            }
            sb.AppendLine("<script>");
            sb.AppendLine("document.addEventListener('DOMContentLoaded', function () {");
            sb.AppendLine("  fetch('/api/warmup').catch(() => {});");
            sb.AppendLine($"  var formId = '{form.Id}';");
            sb.AppendLine("  document.querySelectorAll('form input').forEach(function(input) {");
            sb.AppendLine("    var key = 'form-' + formId + '-' + input.name;");
            sb.AppendLine("    var saved = localStorage.getItem(key);");
            sb.AppendLine("    if (saved) { input.value = saved; }");
            sb.AppendLine("    input.addEventListener('blur', function () {");
            sb.AppendLine("      localStorage.setItem(key, input.value);");
            sb.AppendLine("    });");
            sb.AppendLine("  });");
            if (hasPlace)
            {
                sb.AppendLine("  document.querySelectorAll('.place-autocomplete').forEach(function(el){ new google.maps.places.Autocomplete(el); });");
            }
            sb.AppendLine("  document.querySelector('form').addEventListener('submit', async function(e) {");
            sb.AppendLine("    e.preventDefault();");
            sb.AppendLine("    var data = { answers: {}, consentGivenAt: new Date().toISOString() };");
            sb.AppendLine("    document.querySelectorAll('form input').forEach(function(input){ data.answers[input.name]=input.value; });");
            sb.AppendLine($"    await fetch('/api/forms/{form.Id}/answers', {{ method: 'POST', headers: {{ 'Content-Type': 'application/json' }}, body: JSON.stringify(data) }});");
            var thanks = string.IsNullOrWhiteSpace(form.ThankYouPageUrl) ? "/post-submit" : form.ThankYouPageUrl;
            sb.AppendLine($"    window.location.href = '{WebUtility.HtmlEncode(thanks)}';");
            sb.AppendLine("  });");
            sb.AppendLine("});");
            sb.AppendLine("</script>");

            sb.AppendLine("</body></html>");
            return sb.ToString();
        }
    }
}
