namespace EmailTemplateApi.Application.EmailTemplates.Models;

public sealed record PreparedEmail(
    int TemplateId,
    string TemplateName,
    string Subject,
    string Body);
