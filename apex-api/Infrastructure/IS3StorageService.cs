namespace ApexApi.Infrastructure;

// Interface Segregation Principle (CLAUDE.md Section 4, Part B): this interface
// only exposes what the app needs from S3 — upload and download for food media.
// A service that only reads food logs never sees this interface at all.
public interface IS3StorageService
{
    Task<string> UploadFoodMediaAsync(Stream fileStream, string contentType, CancellationToken ct = default);
    Task<Stream> DownloadAsync(string s3Key, CancellationToken ct = default);
}
