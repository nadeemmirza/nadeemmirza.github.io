using EmailTemplateApi.Application.EmailTemplates.Commands;
using EmailTemplateApi.Application.EmailTemplates.Exceptions;
using EmailTemplateApi.Application.EmailTemplates.Services;
using EmailTemplateApi.Domain.Entities;
using EmailTemplateApi.Domain.Repositories;
using EmailTemplateApi.Infrastructure.Rendering;

namespace EmailTemplateApi.Tests;

public sealed class RenderEmailTemplateCommandHandlerTests
{
    private readonly ITemplateRenderer _renderer = new TemplateRenderer();

    [Fact]
    public async Task Handle_ReturnsPreparedEmail_WhenTemplateExistsAndDataIsComplete()
    {
        var repository = new FakeEmailTemplateRepository(new EmailTemplate
        {
            Id = 7,
            TemplateName = "WelcomeEmail",
            Subject = "Welcome {{FirstName}}",
            Body = "<p>Your start date is {{StartDate}}</p>"
        });
        var handler = new RenderEmailTemplateCommandHandler(repository, _renderer);

        var result = await handler.Handle(
            new RenderEmailTemplateCommand(
                7,
                new Dictionary<string, string>
                {
                    ["FirstName"] = "Nadeem",
                    ["StartDate"] = "2026-05-12"
                }),
            CancellationToken.None);

        Assert.Equal(7, result.TemplateId);
        Assert.Equal("WelcomeEmail", result.TemplateName);
        Assert.Equal("Welcome Nadeem", result.Subject);
        Assert.Equal("<p>Your start date is 2026-05-12</p>", result.Body);
    }

    [Fact]
    public async Task Handle_Throws_WhenTemplateDoesNotExist()
    {
        var handler = new RenderEmailTemplateCommandHandler(new FakeEmailTemplateRepository(null), _renderer);

        await Assert.ThrowsAsync<EmailTemplateNotFoundException>(() =>
            handler.Handle(
                new RenderEmailTemplateCommand(99, new Dictionary<string, string>()),
                CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Throws_WhenMergeDataIsMissing()
    {
        var repository = new FakeEmailTemplateRepository(new EmailTemplate
        {
            Id = 3,
            TemplateName = "Reset",
            Subject = "Reset for {{FirstName}}",
            Body = "Code {{ResetCode}}"
        });
        var handler = new RenderEmailTemplateCommandHandler(repository, _renderer);

        var exception = await Assert.ThrowsAsync<TemplateMergeDataMissingException>(() =>
            handler.Handle(
                new RenderEmailTemplateCommand(3, new Dictionary<string, string> { ["FirstName"] = "Nadeem" }),
                CancellationToken.None));

        Assert.Equal(["ResetCode"], exception.MissingKeys);
    }

    private sealed class FakeEmailTemplateRepository(EmailTemplate? template) : IEmailTemplateRepository
    {
        public Task<EmailTemplate?> GetByIdAsync(int templateId, CancellationToken cancellationToken) =>
            Task.FromResult(template?.Id == templateId ? template : null);
    }
}
