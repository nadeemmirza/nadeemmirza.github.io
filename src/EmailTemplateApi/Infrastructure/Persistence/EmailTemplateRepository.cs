using EmailTemplateApi.Domain.Entities;
using EmailTemplateApi.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EmailTemplateApi.Infrastructure.Persistence;

public sealed class EmailTemplateRepository(EmailTemplateDbContext dbContext) : IEmailTemplateRepository
{
    public Task<EmailTemplate?> GetByIdAsync(int templateId, CancellationToken cancellationToken) =>
        dbContext.EmailTemplates
            .AsNoTracking()
            .SingleOrDefaultAsync(template => template.Id == templateId, cancellationToken);
}
