using Studia.Application.Sections;

namespace Studia.Infrastructure.Content;

public class AllowListHtmlSanitizer : IHtmlSanitizer
{
    private readonly Ganss.Xss.HtmlSanitizer _sanitizer = new();

    public string Sanitize(string html) => _sanitizer.Sanitize(html);
}
