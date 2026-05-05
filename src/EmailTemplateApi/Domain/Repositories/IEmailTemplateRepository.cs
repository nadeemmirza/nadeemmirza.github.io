using EmailTemplateApi.Domain.Entities;

namespace EmailTemplateApi.Domain.Repositories;

public interface IEmailTemplateRepository
{
    Task<EmailTemplate?> GetByIdAsync(int templateId, CancellationToken cancellationToken);
}
