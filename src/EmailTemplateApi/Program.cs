using EmailTemplateApi.Application.Common.Messaging;
using EmailTemplateApi.Application.EmailTemplates.Commands;
using EmailTemplateApi.Application.EmailTemplates.Exceptions;
using EmailTemplateApi.Application.EmailTemplates.Models;
using EmailTemplateApi.Application.EmailTemplates.Services;
using EmailTemplateApi.Contracts;
using EmailTemplateApi.Domain.Repositories;
using EmailTemplateApi.Infrastructure.Mediation;
using EmailTemplateApi.Infrastructure.Persistence;
using EmailTemplateApi.Infrastructure.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddDbContext<EmailTemplateDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("EmailTemplateDatabase")));
builder.Services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
builder.Services.AddScoped<ITemplateRenderer, TemplateRenderer>();
builder.Services.AddScoped<IMediator, SimpleMediator>();
builder.Services.AddScoped<IRequestHandler<RenderEmailTemplateCommand, PreparedEmail>, RenderEmailTemplateCommandHandler>();

var app = builder.Build();

app.UseExceptionHandler();

await EmailTemplateSeeder.SeedAsync(app.Services, app.Logger);

app.MapPost("/api/email-templates/render", async Task<IResult>
    (RenderEmailTemplateRequest request, IMediator mediator, CancellationToken cancellationToken) =>
{
    if (request.TemplateId <= 0)
    {
        return TypedResults.ValidationProblem(new Dictionary<string, string[]>
        {
            [nameof(request.TemplateId)] = ["TemplateId must be greater than zero."]
        });
    }

    var mergeData = request.MergeData ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    var normalizedMergeData = new Dictionary<string, string>(mergeData, StringComparer.OrdinalIgnoreCase);

    try
    {
        var preparedEmail = await mediator.Send(
            new RenderEmailTemplateCommand(request.TemplateId, normalizedMergeData),
            cancellationToken);

        return TypedResults.Ok(new RenderEmailTemplateResponse(
            preparedEmail.TemplateId,
            preparedEmail.TemplateName,
            preparedEmail.Subject,
            preparedEmail.Body,
            normalizedMergeData));
    }
    catch (TemplateMergeDataMissingException exception)
    {
        return TypedResults.ValidationProblem(new Dictionary<string, string[]>
        {
            [nameof(request.MergeData)] = exception.MissingKeys
                .Select(key => $"Missing value for placeholder '{key}'.")
                .ToArray()
        });
    }
    catch (EmailTemplateNotFoundException exception)
    {
        return TypedResults.NotFound(new ProblemDetails
        {
            Title = "Email template not found",
            Detail = exception.Message,
            Status = StatusCodes.Status404NotFound
        });
    }
});

app.MapGet("/api/email-templates/{id:int}", async Task<IResult>
    (int id, IEmailTemplateRepository repository, CancellationToken cancellationToken) =>
{
    var template = await repository.GetByIdAsync(id, cancellationToken);

    if (template is null)
    {
        return TypedResults.NotFound(new ProblemDetails
        {
            Title = "Email template not found",
            Detail = $"Email template with id '{id}' was not found.",
            Status = StatusCodes.Status404NotFound
        });
    }

    return TypedResults.Ok(new
    {
        template.Id,
        template.TemplateName,
        template.Subject,
        template.Body
    });
});

app.Run();

public partial class Program;
