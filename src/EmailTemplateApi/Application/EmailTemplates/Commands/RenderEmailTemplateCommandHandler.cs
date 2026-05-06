using EmailTemplateApi.Application.Common.Messaging;
using EmailTemplateApi.Application.EmailTemplates.Exceptions;
using EmailTemplateApi.Application.EmailTemplates.Models;
using EmailTemplateApi.Application.EmailTemplates.Services;
using EmailTemplateApi.Domain.Repositories;

namespace EmailTemplateApi.Application.EmailTemplates.Commands;

public sealed class RenderEmailTemplateCommandHandler(
    IEmailTemplateRepository repository,
    ITemplateRenderer templateRenderer) : IRequestHandler<RenderEmailTemplateCommand, PreparedEmail>
{
    public async Task<PreparedEmail> Handle(RenderEmailTemplateCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var template = await repository.GetByIdAsync(request.TemplateId, cancellationToken);
        if (template is null)
        {
            throw new EmailTemplateNotFoundException(request.TemplateId);
        }

        var missingKeys = templateRenderer.GetMissingKeys(
            [template.Subject, template.Body],
            request.MergeData);

        if (missingKeys.Count > 0)
        {
            throw new TemplateMergeDataMissingException(missingKeys);
        }

        var renderedSubject = templateRenderer.Render(template.Subject, request.MergeData);
        var renderedBody = templateRenderer.Render(template.Body, request.MergeData);

        return new PreparedEmail(
            template.Id,
            template.TemplateName,
            renderedSubject,
            renderedBody);
    }
}
