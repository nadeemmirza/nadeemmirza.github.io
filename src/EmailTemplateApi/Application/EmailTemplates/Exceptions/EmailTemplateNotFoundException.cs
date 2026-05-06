namespace EmailTemplateApi.Application.EmailTemplates.Exceptions;

public sealed class EmailTemplateNotFoundException(int templateId)
    : Exception($"Email template with id '{templateId}' was not found.");
