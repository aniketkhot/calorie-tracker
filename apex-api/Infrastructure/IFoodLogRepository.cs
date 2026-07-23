using ApexApi.Models;

namespace ApexApi.Infrastructure;

// Repository Pattern (CLAUDE.md Section 4, Pattern 1): the app talks to this
// interface only. It has no idea the data lives in DynamoDB — swap the database
// by swapping the implementation, nothing above this interface changes.
public interface IFoodLogRepository
{
    Task<FoodLog?> GetByIdAsync(string userId, string logId, CancellationToken ct = default);
    Task<IReadOnlyList<FoodLog>> GetByDateAsync(string userId, DateOnly date, CancellationToken ct = default);
    Task SaveAsync(FoodLog log, CancellationToken ct = default);
    Task DeleteAsync(string userId, string logId, CancellationToken ct = default);
}
