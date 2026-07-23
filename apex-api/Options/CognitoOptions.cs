namespace ApexApi.Options;

// Bound from the "Cognito" section of appsettings.json. See CLAUDE.md Section 13.
// UserPoolId/ClientId are intentionally not [Required] here — they're blank until
// the Cognito user pool is created (Rule 1a handoff), and JWT middleware that
// consumes this isn't wired up until Section 18's Cognito JWT middleware step.
public class CognitoOptions
{
    public const string SectionName = "Cognito";

    public string UserPoolId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string Authority { get; set; } = string.Empty;
}
