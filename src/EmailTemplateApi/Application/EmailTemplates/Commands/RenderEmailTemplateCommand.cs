using EmailTemplateApi.Application.Common.Messaging;
using EmailTemplateApi.Application.EmailTemplates.Models;

namespace EmailTemplateApi.Application.EmailTemplates.Commands;

public sealed record RenderEmailTemplateCommand(
    int TemplateId,
    IReadOnlyDictionary<string, string> MergeData) : IRequest<PreparedEmail>;
