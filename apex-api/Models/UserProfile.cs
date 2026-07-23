namespace ApexApi.Models;

// DynamoDB user profile. See CLAUDE.md Section 11:
//   PK: USER#{userId}   SK: PROFILE
public record UserProfile(
    string UserId,
    string Name,
    string Email,
    DateTimeOffset CreatedAt,
    int DailyCalorieGoal);
