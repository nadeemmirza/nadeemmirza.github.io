using EmailTemplateApi.Infrastructure.Rendering;

namespace EmailTemplateApi.Tests;

public sealed class TemplateRendererTests
{
    private readonly TemplateRenderer _renderer = new();

    [Fact]
    public void Render_ReplacesTokens_CaseInsensitively()
    {
        var mergeData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["firstname"] = "Nadeem",
            ["CompanyName"] = "Contoso"
        };

        var result = _renderer.Render("Hello {{FirstName}}, welcome to {{companyname}}.", mergeData);

        Assert.Equal("Hello Nadeem, welcome to Contoso.", result);
    }

    [Fact]
    public void GetMissingKeys_ReturnsDistinctSortedMissingPlaceholders()
    {
        var missingKeys = _renderer.GetMissingKeys(
            ["Subject {{FirstName}}", "Body {{ResetCode}} {{firstname}} {{AccountId}}"],
            new Dictionary<string, string> { ["FirstName"] = "Nadeem" });

        Assert.Equal(["AccountId", "ResetCode"], missingKeys);
    }
}
