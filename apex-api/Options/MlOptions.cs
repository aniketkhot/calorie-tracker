using System.ComponentModel.DataAnnotations;

namespace ApexApi.Options;

// Bound from the "ML" section of appsettings.json. See CLAUDE.md Section 13/14.
public class MlOptions
{
    public const string SectionName = "ML";

    [Required]
    public string ModelsPath { get; set; } = "./ML/Models";

    [Required]
    public string FoodModelName { get; set; } = "food_detection.onnx";

    [Required]
    public string DepthModelName { get; set; } = "midas_depth.onnx";

    [Range(1, 100)]
    public int MaxImageSizeMb { get; set; } = 10;
}
