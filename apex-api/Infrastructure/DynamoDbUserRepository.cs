using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using ApexApi.Models;
using ApexApi.Options;
using Microsoft.Extensions.Options;

namespace ApexApi.Infrastructure;

// Repository Pattern + Adapter Pattern (CLAUDE.md Section 4, Patterns 1 & 7).
// Single-table design (Section 11): PK = USER#{userId}, SK = PROFILE (fixed).
public class DynamoDbUserRepository : IUserRepository
{
    private readonly Table _table;
    private readonly ILogger<DynamoDbUserRepository> _logger;

    public DynamoDbUserRepository(
        IAmazonDynamoDB dynamoDb,
        IOptions<AwsOptions> awsOptions,
        ILogger<DynamoDbUserRepository> logger)
    {
        _table = Table.LoadTable(dynamoDb, awsOptions.Value.DynamoDbTableName);
        _logger = logger;
    }

    public async Task<UserProfile?> GetByIdAsync(string userId, CancellationToken ct = default)
    {
        var document = await _table.GetItemAsync(PartitionKey(userId), "PROFILE", ct);
        return document is null ? null : ToUserProfile(document);
    }

    public async Task SaveAsync(UserProfile profile, CancellationToken ct = default)
    {
        var document = new Document
        {
            ["PK"] = PartitionKey(profile.UserId),
            ["SK"] = "PROFILE",
            ["name"] = profile.Name,
            ["email"] = profile.Email,
            ["createdAt"] = profile.CreatedAt.ToString("O"),
            ["dailyCalorieGoal"] = profile.DailyCalorieGoal,
        };

        await _table.PutItemAsync(document, ct);
        _logger.LogInformation("Saved profile for user {UserId}", profile.UserId);
    }

    private static string PartitionKey(string userId) => $"USER#{userId}";

    private static UserProfile ToUserProfile(Document document) => new(
        UserId: ((string)document["PK"]!).Replace("USER#", string.Empty),
        Name: document["name"],
        Email: document["email"],
        CreatedAt: DateTimeOffset.Parse(document["createdAt"]),
        DailyCalorieGoal: document["dailyCalorieGoal"].AsInt());
}
