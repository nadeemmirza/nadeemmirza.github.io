namespace EmailTemplateApi.Contracts;

public sealed record RenderEmailTemplateResponse(
    int TemplateId,
    string TemplateName,
    string Subject,
    string Body,
    IReadOnlyDictionary<string, string> MergeData);
