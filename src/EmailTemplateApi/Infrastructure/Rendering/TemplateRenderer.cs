using System.Text.RegularExpressions;
using EmailTemplateApi.Application.EmailTemplates.Services;

namespace EmailTemplateApi.Infrastructure.Rendering;

public sealed partial class TemplateRenderer : ITemplateRenderer
{
    public IReadOnlyCollection<string> GetMissingKeys(IEnumerable<string> templates, IReadOnlyDictionary<string, string> mergeData)
    {
        ArgumentNullException.ThrowIfNull(templates);
        ArgumentNullException.ThrowIfNull(mergeData);

        var normalizedMergeData = new HashSet<string>(mergeData.Keys, StringComparer.OrdinalIgnoreCase);

        return templates
            .SelectMany(GetPlaceholders)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(placeholder => !normalizedMergeData.Contains(placeholder))
            .OrderBy(static placeholder => placeholder, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public string Render(string template, IReadOnlyDictionary<string, string> mergeData)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(mergeData);

        var normalizedMergeData = new Dictionary<string, string>(mergeData, StringComparer.OrdinalIgnoreCase);

        return PlaceholderRegex().Replace(template, match =>
        {
            var key = match.Groups["key"].Value;
            return normalizedMergeData.TryGetValue(key, out var value) ? value : match.Value;
        });
    }

    private static IEnumerable<string> GetPlaceholders(string template) =>
        PlaceholderRegex().Matches(template)
            .Select(match => match.Groups["key"].Value);

    [GeneratedRegex("""\{\{\s*(?<key>[A-Za-z0-9_.-]+)\s*\}\}""", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex PlaceholderRegex();
}
