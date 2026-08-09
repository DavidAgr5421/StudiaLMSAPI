using Studia.Application.Sections;

namespace Studia.Application.Tests.Sections;

public class FakeHtmlSanitizer : IHtmlSanitizer
{
    public string Sanitize(string html) => $"sanitized:{html}";
}
