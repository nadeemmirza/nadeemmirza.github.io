namespace EmailTemplateApi.Application.EmailTemplates.Services;

public interface ITemplateRenderer
{
    IReadOnlyCollection<string> GetMissingKeys(IEnumerable<string> templates, IReadOnlyDictionary<string, string> mergeData);

    string Render(string template, IReadOnlyDictionary<string, string> mergeData);
}
