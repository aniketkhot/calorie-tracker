namespace ApexApi.Models;

// DynamoDB food log entry. See CLAUDE.md Section 11:
//   PK: USER#{userId}   SK: FOODLOG#{date}#{timestamp}
public record FoodLog(
    string UserId,
    string LogId,
    string FoodName,
    double EstimatedGrams,
    double Calories,
    double Protein,
    double Carbohydrates,
    double Fat,
    double Confidence,
    string? ReferenceObjectDetected,
    string S3ImageKey,
    string ProcessingMethod,
    DateTimeOffset LoggedAt);
