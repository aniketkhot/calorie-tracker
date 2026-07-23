using ApexApi.Options;

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
