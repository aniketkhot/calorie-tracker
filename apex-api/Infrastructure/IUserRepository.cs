using ApexApi.Models;

namespace ApexApi.Infrastructure;

// Repository Pattern (CLAUDE.md Section 4, Pattern 1) — separate interface from
// IFoodLogRepository (Interface Segregation, Part B): a class that only needs
// user profiles never depends on, or even sees, food log methods.
public interface IUserRepository
{
    Task<UserProfile?> GetByIdAsync(string userId, CancellationToken ct = default);
    Task SaveAsync(UserProfile profile, CancellationToken ct = default);
}
