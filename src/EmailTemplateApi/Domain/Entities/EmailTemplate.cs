namespace EmailTemplateApi.Domain.Entities;

public sealed class EmailTemplate
{
    public int Id { get; set; }

    public required string TemplateName { get; set; }

    public required string Subject { get; set; }

    public required string Body { get; set; }
}
