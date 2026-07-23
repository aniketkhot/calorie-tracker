using Amazon.DynamoDBv2;
using Amazon.S3;
using ApexApi.Infrastructure;
using ApexApi.Options;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Options Pattern (CLAUDE.md Section 4, Pattern 6) — ValidateOnStart() means a
// missing/invalid config value (e.g. no S3BucketName) crashes at startup with a
// clear error instead of surfacing as a null-reference deep inside a user's scan.
builder.Services.AddOptions<MlOptions>()
    .BindConfiguration(MlOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<AwsOptions>()
    .BindConfiguration(AwsOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<UsdaOptions>()
    .BindConfiguration(UsdaOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<CognitoOptions>()
    .BindConfiguration(CognitoOptions.SectionName)
    .ValidateOnStart();

// AWS SDK clients — registered once, region driven by AwsOptions rather than
// left to the SDK's default credential/region resolution chain, so the region
// we actually configured in appsettings.json is the single source of truth.
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var region = sp.GetRequiredService<IOptions<AwsOptions>>().Value.Region;
    return new AmazonS3Client(Amazon.RegionEndpoint.GetBySystemName(region));
});

builder.Services.AddSingleton<IAmazonDynamoDB>(sp =>
{
    var region = sp.GetRequiredService<IOptions<AwsOptions>>().Value.Region;
    return new AmazonDynamoDBClient(Amazon.RegionEndpoint.GetBySystemName(region));
});

// Repository Pattern (Section 4, Pattern 1) — interfaces registered so every
// consumer depends on IFoodLogRepository/IUserRepository/IS3StorageService,
// never the concrete DynamoDB/S3 classes (Dependency Inversion, Part B).
builder.Services.AddScoped<IS3StorageService, S3StorageService>();
builder.Services.AddScoped<IFoodLogRepository, DynamoDbFoodLogRepository>();
builder.Services.AddScoped<IUserRepository, DynamoDbUserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
