namespace EmailTemplateApi.Contracts;

public sealed record RenderEmailTemplateRequest(
    int TemplateId,
    IDictionary<string, string>? MergeData);
