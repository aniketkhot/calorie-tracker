using System.ComponentModel.DataAnnotations;

namespace ApexApi.Options;

// Bound from the "AWS" section of appsettings.json. See CLAUDE.md Section 13.
public class AwsOptions
{
    public const string SectionName = "AWS";

    [Required]
    public string Region { get; set; } = "ap-southeast-2";

    [Required]
    public string S3BucketName { get; set; } = string.Empty;

    [Required]
    public string DynamoDbTableName { get; set; } = string.Empty;
}
