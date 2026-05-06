namespace EmailTemplateApi.Application.EmailTemplates.Exceptions;

public sealed class TemplateMergeDataMissingException : Exception
{
    public TemplateMergeDataMissingException(IReadOnlyCollection<string> missingKeys)
        : base($"Missing merge data for placeholders: {string.Join(", ", missingKeys)}")
    {
        MissingKeys = missingKeys;
    }

    public IReadOnlyCollection<string> MissingKeys { get; }
}
