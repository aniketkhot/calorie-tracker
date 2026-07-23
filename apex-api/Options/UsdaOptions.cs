using System.ComponentModel.DataAnnotations;

namespace ApexApi.Options;

// Bound from the "USDA" section of appsettings.json. See CLAUDE.md Section 13/17.
public class UsdaOptions
{
    public const string SectionName = "USDA";

    [Required]
    public string ApiKey { get; set; } = "DEMO_KEY";

    [Required, Url]
    public string BaseUrl { get; set; } = "https://api.nal.usda.gov/fdc/v1";
}
