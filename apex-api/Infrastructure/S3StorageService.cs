using Amazon.S3;
using Amazon.S3.Model;
using ApexApi.Options;
using Microsoft.Extensions.Options;

namespace ApexApi.Infrastructure;

// Adapter Pattern (CLAUDE.md Section 4, Pattern 7): translates between our own
// IS3StorageService interface and the AWS SDK's IAmazonS3. If AWS changes its
// SDK surface, or we ever swap S3 for another blob store, only this class changes
// — FoodScanService and everything above it never references IAmazonS3 directly.
public class S3StorageService : IS3StorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly ILogger<S3StorageService> _logger;

    public S3StorageService(IAmazonS3 s3Client, IOptions<AwsOptions> awsOptions, ILogger<S3StorageService> logger)
    {
        _s3Client = s3Client;
        _bucketName = awsOptions.Value.S3BucketName;
        _logger = logger;
    }

    public async Task<string> UploadFoodMediaAsync(Stream fileStream, string contentType, CancellationToken ct = default)
    {
        var s3Key = $"food-uploads/{Guid.NewGuid()}";

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = s3Key,
            InputStream = fileStream,
            ContentType = contentType,
        };

        await _s3Client.PutObjectAsync(request, ct);
        _logger.LogInformation("Uploaded food media to S3 key {S3Key}", s3Key);

        return s3Key;
    }

    public async Task<Stream> DownloadAsync(string s3Key, CancellationToken ct = default)
    {
        var response = await _s3Client.GetObjectAsync(_bucketName, s3Key, ct);
        return response.ResponseStream;
    }
}
