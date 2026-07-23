using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using ApexApi.Models;
using ApexApi.Options;
using Microsoft.Extensions.Options;

namespace ApexApi.Infrastructure;

// Repository Pattern + Adapter Pattern (CLAUDE.md Section 4, Patterns 1 & 7).
// Single-table design (Section 11): PK = USER#{userId}, SK = FOODLOG#{date}#{timestamp}.
// Uses the low-level Document/Table API rather than the object-persistence model —
// the composite sort key format here doesn't map cleanly onto POCO attributes,
// which is the standard approach for single-table DynamoDB designs in .NET.
public class DynamoDbFoodLogRepository : IFoodLogRepository
{
    private readonly Table _table;
    private readonly ILogger<DynamoDbFoodLogRepository> _logger;

    public DynamoDbFoodLogRepository(
        IAmazonDynamoDB dynamoDb,
        IOptions<AwsOptions> awsOptions,
        ILogger<DynamoDbFoodLogRepository> logger)
    {
        _table = Table.LoadTable(dynamoDb, awsOptions.Value.DynamoDbTableName);
        _logger = logger;
    }

    public async Task<FoodLog?> GetByIdAsync(string userId, string logId, CancellationToken ct = default)
    {
        // logId embeds the SK suffix (date#timestamp) so a single item can be
        // fetched directly without a query scan across the user's whole log.
        var document = await _table.GetItemAsync(PartitionKey(userId), SortKey(logId), ct);
        return document is null ? null : ToFoodLog(document);
    }

    public async Task<IReadOnlyList<FoodLog>> GetByDateAsync(string userId, DateOnly date, CancellationToken ct = default)
    {
        var search = _table.Query(new QueryOperationConfig
        {
            KeyExpression = new Expression
            {
                ExpressionStatement = "PK = :pk AND begins_with(SK, :skPrefix)",
                ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>
                {
                    [":pk"] = PartitionKey(userId),
                    [":skPrefix"] = $"FOODLOG#{date:yyyy-MM-dd}",
                },
            },
        });

        var results = new List<FoodLog>();
        do
        {
            var page = await search.GetNextSetAsync(ct);
            results.AddRange(page.Select(ToFoodLog));
        } while (!search.IsDone);

        return results;
    }

    public async Task SaveAsync(FoodLog log, CancellationToken ct = default)
    {
        var document = new Document
        {
            ["PK"] = PartitionKey(log.UserId),
            ["SK"] = SortKey(log.LogId),
            ["foodName"] = log.FoodName,
            ["estimatedGrams"] = log.EstimatedGrams,
            ["calories"] = log.Calories,
            ["protein"] = log.Protein,
            ["carbohydrates"] = log.Carbohydrates,
            ["fat"] = log.Fat,
            ["confidence"] = log.Confidence,
            ["referenceObjectDetected"] = log.ReferenceObjectDetected,
            ["s3ImageKey"] = log.S3ImageKey,
            ["processingMethod"] = log.ProcessingMethod,
            ["loggedAt"] = log.LoggedAt.ToString("O"),
        };

        await _table.PutItemAsync(document, ct);
        _logger.LogInformation("Saved food log {LogId} for user {UserId}", log.LogId, log.UserId);
    }

    public async Task DeleteAsync(string userId, string logId, CancellationToken ct = default)
    {
        await _table.DeleteItemAsync(PartitionKey(userId), SortKey(logId), ct);
    }

    private static string PartitionKey(string userId) => $"USER#{userId}";
    private static string SortKey(string logId) => $"FOODLOG#{logId}";

    private static FoodLog ToFoodLog(Document document) => new(
        UserId: ((string)document["PK"]!).Replace("USER#", string.Empty),
        LogId: ((string)document["SK"]!).Replace("FOODLOG#", string.Empty),
        FoodName: document["foodName"],
        EstimatedGrams: document["estimatedGrams"].AsDouble(),
        Calories: document["calories"].AsDouble(),
        Protein: document["protein"].AsDouble(),
        Carbohydrates: document["carbohydrates"].AsDouble(),
        Fat: document["fat"].AsDouble(),
        Confidence: document["confidence"].AsDouble(),
        ReferenceObjectDetected: document.ContainsKey("referenceObjectDetected") ? document["referenceObjectDetected"] : null,
        S3ImageKey: document["s3ImageKey"],
        ProcessingMethod: document["processingMethod"],
        LoggedAt: DateTimeOffset.Parse(document["loggedAt"]));
}
