using EmailTemplateApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmailTemplateApi.Infrastructure.Persistence;

public sealed class EmailTemplateDbContext(DbContextOptions<EmailTemplateDbContext> options) : DbContext(options)
{
    public DbSet<EmailTemplate> EmailTemplates => Set<EmailTemplate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmailTemplate>(entity =>
        {
            entity.ToTable("EmailTemplates");
            entity.HasKey(template => template.Id);
            entity.Property(template => template.TemplateName)
                .HasMaxLength(200)
                .IsRequired();
            entity.Property(template => template.Subject)
                .HasMaxLength(250)
                .IsRequired();
            entity.Property(template => template.Body)
                .IsRequired();
        });
    }
}
