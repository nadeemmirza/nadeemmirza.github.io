using EmailTemplateApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmailTemplateApi.Infrastructure.Persistence;

public static class EmailTemplateSeeder
{
    public static async Task SeedAsync(IServiceProvider services, ILogger logger)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EmailTemplateDbContext>();

        await dbContext.Database.EnsureCreatedAsync();

        if (await dbContext.EmailTemplates.AnyAsync())
        {
            return;
        }

        dbContext.EmailTemplates.AddRange(
            new EmailTemplate
            {
                Id = 1,
                TemplateName = "WelcomeEmail",
                Subject = "Welcome, {{FirstName}}!",
                Body = "<h1>Hello {{FirstName}}</h1><p>Thanks for joining {{CompanyName}}. Your onboarding starts on {{StartDate}}.</p>"
            },
            new EmailTemplate
            {
                Id = 2,
                TemplateName = "PasswordReset",
                Subject = "Reset your password, {{FirstName}}",
                Body = "<p>Hi {{FirstName}},</p><p>Use the following code to reset your password: <strong>{{ResetCode}}</strong></p>"
            });

        await dbContext.SaveChangesAsync();
        logger.LogInformation("Seeded sample email templates.");
    }
}
