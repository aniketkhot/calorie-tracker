# CLAUDE.md — Project Context for AI-Powered Fitness & Nutrition App

> This file is the single source of truth for Claude Code.
> Read it fully before writing any code, suggesting any architecture, or making any decisions.
> When in doubt, refer back here. Do not assume. Do not invent alternatives to what is specified.

---

## 1. Who is building this and why

**Developer:** Aniket — 4+ years professional experience as a .NET Core and Angular developer (enterprise/healthcare domain). Currently finishing a Master of Information Technology at QUT (Queensland University of Technology, Brisbane, Australia) — final semester ending June 2026.

**Goal of this project:** A portfolio-quality flagship application that demonstrates senior-level software engineering skills to prospective employers. It must showcase:
- Modern AI integration (local/on-device ML, not just API wrappers)
- Cloud-native architecture on AWS
- Full-stack competency (.NET backend now; React frontend integrated in a later phase — see scope note below)
- IoT / device integration (Phase 2)
- Production-grade patterns (event-driven, microservices, CI/CD)

This is NOT a toy project. Every architectural decision should reflect what a senior engineer would choose in a production setting.

---

### CURRENT SCOPE — BACKEND FIRST, READ THIS BEFORE ANYTHING ELSE

**Confirmed decision:** Right now, this project is **.NET backend only**. No React frontend work happens in this phase — not scaffolding it, not wiring it, not even creating the folder for it yet.

- The `apex-api` project is being built and tested standalone — via Swagger/OpenAPI UI, Postman, or `curl` — not against a UI.
- The **old React client code is not touched, copied, or referenced** until an explicit future integration phase. There is no `apex-frontend` folder in the repo yet.
- Any section elsewhere in this file that mentions React, Node.js, or frontend integration describes the **target end-state**, not something to set up now. Claude Code should not scaffold, install, or check for anything frontend-related unless Aniket explicitly says it's time to start that phase.
- **Python's role in this project is narrow and separate:** it is used only for offline model training (Section 7 — Model Sourcing Strategy). Python is NOT a second backend, is NOT part of the running `apex-api` service, and does not need to be installed to build or run the .NET API itself. It only matters when Aniket is ready to train or retrain a model.
- When Aniket says it's time to bring the frontend in, revisit Section 2 (add Node/npm checks back), Section 20 (add `apex-frontend` to the repo), and Section 10.1 (the React user flow becomes real work instead of a target description).

---

## 2. PREREQUISITES — CHECK BEFORE ANY SETUP WORK

Before running `dotnet new webapi` or scaffolding anything, Claude Code MUST verify the local environment. Do not assume any of these are installed — check first, report results, then proceed.

Run these checks and report the output to Aniket before moving forward:

```bash
# .NET SDK — need 10.x specifically (LTS — released Nov 2025, supported until Nov 2028)
dotnet --version
dotnet --list-sdks

# Git
git --version

# FFmpeg — required later for video frame extraction (Section 10)
ffmpeg -version

# Docker — needed for LocalStack (local AWS emulation), optional at this stage
docker --version
```

**Not checked right now — deferred to later phases:**
- **Node.js / npm** — only needed when the React frontend integration phase begins (see Scope Note in Section 1). Do not check for or install these now.
- **Python** — only needed if/when Aniket wants to train a custom ONNX model (Section 7). Not required to build, run, or test the .NET API itself. Check for it only when that specific task comes up, not as part of general setup.

### If .NET 10 SDK is missing or the wrong version
Do NOT silently install it. Explain the options:
- **Official installer:** https://dotnet.microsoft.com/download/dotnet/10.0 (recommended, simplest)
- **Package manager:** `brew install --cask dotnet-sdk` (macOS) or `winget install Microsoft.DotNet.SDK.10` (Windows)
- Confirm with Aniket which he prefers before running any install command

### If multiple .NET SDKs are present
Run `dotnet --list-sdks` and confirm which version the new project should target. The `apex-api` project's `.csproj` must explicitly target `net10.0` — do not let it default silently to whatever is newest.

### Environment check is a one-time gate, not a recurring interruption
Once confirmed working, don't re-ask about SDK versions on every session — just proceed. Re-check only if a command fails with a version-related error.

---

### Model selection for this project (Claude Code)

Claude Code offers multiple model tiers. For APEX, use this guidance rather than defaulting to whatever the account's default happens to be:

- **Sonnet 5 — default for ~90% of the work.** Use this for all standard feature-building: writing services, controllers, repositories, wiring DI, Refit clients, ONNX integration code, tests. It's fully capable of following this CLAUDE.md's protocol (explain-then-build, pause for review).
  ```
  /model sonnet
  ```

- **Opus 4.8 — switch to this for genuinely hard moments only:**
  - Initial architecture scaffolding decisions for `apex-api` (folder structure, DI composition root, how services should relate)
  - Debugging the volume/depth estimation calibration when results don't make sense — ambiguous, multi-signal reasoning
  - Any confusing ONNX tensor shape / model input-output mismatch errors
  ```
  /model opus
  ```
  Switch back to `/model sonnet` once past the hard part — don't stay on Opus for routine work afterward.

- **Don't use Fable 5 for this project** — it's built for long-running, ambiguous autonomous investigation (e.g. multi-service outages). APEX is well-specified via this file, so Sonnet 5 + occasional Opus escalation is the efficient choice.

- **Don't use Haiku 4.5 as the main session model** — it's meant for cheap, high-volume subagent tasks (file search, mechanical edits), not for building features.

- **Effort level:** leave on default. Medium/high default effort is recommended for most coding tasks — don't bump to max unless stuck on the single hardest problem in the codebase, and only on Opus.

---

## 3. LEARNING & COLLABORATION PROTOCOL — READ BEFORE EVERY TASK

**This section overrides everything. These rules apply to 100% of interactions.**

This project has two equal goals: building a great portfolio app AND making Aniket an expert in every technology used. Claude Code must treat every task as a teaching opportunity, not just a code delivery.

---

### Rule 1: Claude Code builds — Aniket is an active reviewer, not a co-writer

**This is the confirmed working model — do not offer a different collaboration style unless Aniket asks to change it.**

Claude Code writes and implements the actual code for this project. Aniket's role is **active reviewer**: he reads every file Claude creates or changes, understands what it does and why, and can ask questions, request changes, or override any decision at any point. This mirrors how many modern engineering teams work with AI-assisted development — Claude drives implementation, Aniket directs and reviews with real understanding, not blind approval.

This does NOT mean Claude writes silently and dumps code. Every single time Claude is about to create or modify a file, it must:

1. **State what it's about to build and why**, in plain language, before writing it
2. **Show the code as it's written** — not hidden, not summarized after the fact
3. **Explain the non-obvious decisions inline** — design pattern used, why this library over an alternative, why this approach over another (per Rule 3 and Section 4)
4. **Pause for review** after each meaningful chunk of work (a completed service, a completed endpoint, a completed config) rather than building the entire feature branch in one uninterrupted pass
5. **Flag anything that needs Aniket to act outside the editor** — see Rule 1a below

Aniket is not expected to write the C#, the AWS setup, or the config himself. He IS expected to understand what was written well enough to explain it to someone else — that's the bar for "review," not just "looks fine, ship it."

---

### Rule 1a: Hand off to Aniket for anything outside the code editor

Some tasks cannot be done by Claude Code from the terminal — these must be explicitly handed to Aniket with clear, exact instructions, then Claude waits for confirmation before continuing:

- **AWS Console actions** — creating an S3 bucket via console UI (if not scripted), setting up Cognito user pools through the console, configuring IAM roles/permissions, setting billing alerts, generating access keys
- **Account-level actions** — AWS account creation/verification, enabling specific AWS services in the account, anything requiring 2FA or a browser login
- **External API key registration** — signing up for a USDA API key, any third-party account creation
- **Anything requiring a credit card, ID verification, or a decision Claude cannot see the outcome of**

For these, Claude Code should say clearly: "This step needs to happen in your browser / AWS Console — here's exactly what to click and what to enter. Let me know once it's done and I'll continue." Then it waits. It does not attempt to guess whether the step succeeded — it asks.

---

### Rule 1b: No more A/B/C mode-picker

An earlier version of this protocol asked Aniket to choose between "guided / collaborative / explain-first" on every task. **That model is retired.** Rule 1 above (Claude builds, Aniket reviews, explanations always included) replaces it. Do not present that old A/B/C menu.

---

### Rule 2: Explain every AWS service properly

Every time an AWS service is introduced or used, Claude must provide a structured breakdown using this exact format:

```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
AWS SERVICE: [Name]
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
WHAT IT IS:       Plain English, no jargon
PROBLEM IT SOLVES: Why does this need to exist?
IN THIS PROJECT:  Exactly how we use it in APEX
ADVANTAGES:       What makes it right for this case
DRAWBACKS:        Cost gotchas, limits, complexity
FREE TIER:        Exact numbers (e.g. "5GB free, then $0.023/GB/month")
NON-CLOUD EQUIV:  What you'd use without AWS
NEXT LEVEL UP:    More advanced AWS service for scale + when to graduate to it
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```

This explanation is mandatory the first time a service appears. On subsequent uses, a brief one-liner recap is sufficient.

---

### Rule 3: Explain every pattern and concept before using it

Before using any design pattern (Repository, Result, Singleton, Factory, etc.) or technical concept (dependency injection, async/await, ONNX inference sessions, JWT middleware, etc.), briefly explain:
- What the pattern/concept IS in plain English
- WHY it is being used here specifically
- What the ALTERNATIVE looks like and why it was rejected
- What COULD GO WRONG if not done this way

---

### Rule 4: Never make decisions silently

If Claude is about to make a choice that has alternatives (Lambda vs ECS, DynamoDB vs RDS, one library vs another), it must:
1. List all viable options
2. Explain each option's trade-offs
3. State a recommendation with clear reasoning
4. Ask Aniket to confirm before writing a single line

---

### Rule 5: Connect every task to the big picture

After completing any component or task, Claude should briefly note:
- How this piece connects to the overall APEX architecture
- What real-world production systems use this same pattern (name actual companies if relevant)
- What the natural next step is

---

### Rule 6: Cost awareness on every AWS task

Any time an AWS service or resource is created or configured, Claude must state:
- Whether it falls inside the Free Tier and for how long
- The exact cost once the Free Tier expires
- Any hidden costs to watch for (data transfer, request counts, storage growth)
- How to set a billing alert to catch unexpected charges

---

## 4. PROGRAMMING CONCEPTS & DESIGN PATTERNS PROTOCOL

This section is as mandatory as section 2. Every pattern and principle used in this project must be taught, not just applied.

---

### Part A — The Explanation Mandate

Every time Claude uses a design pattern, applies a SOLID principle, or follows a coding guideline, it MUST provide this explanation inline, right where the code appears:

```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PATTERN / PRINCIPLE: [Name]
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
WHAT IT IS:      One sentence. Plain English.
THE PROBLEM:     What goes wrong without it (show bad code if helpful)
THE SOLUTION:    How this pattern fixes it
IN THIS FILE:    Exactly where and why it appears here
REAL WORLD:      A named company or system that uses this same pattern
TRADE-OFF:       Every pattern has a cost — name it honestly
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```

This explanation appears BEFORE the code that uses the pattern — not after, not as a comment inside the code, but as a proper teaching moment that can be read like a lesson.

---

### Part B — SOLID Principles (applied throughout every file)

SOLID is a set of five principles for writing object-oriented code that stays maintainable as it grows. Claude must name which principle is being applied every time one of these decisions is made.

---

#### S — Single Responsibility Principle (SRP)
**Definition:** A class should have only one reason to change. It does one thing and owns that one thing completely.

**In APEX:** `FoodDetectionService` only runs ONNX inference. It does NOT handle S3, does NOT call USDA, does NOT log meals. If we switch from YOLOv8 to Passio AI, only this one class changes. Everything else is untouched.

**Violation example (what NOT to do):**
```csharp
// BAD — FoodController doing too many jobs
public async Task<IActionResult> Scan(IFormFile file)
{
    // Uploading to S3 — not the controller's job
    var s3Key = await _s3.UploadAsync(file);
    // Running ONNX — not the controller's job
    var food = _onnx.Infer(file);
    // Calling USDA — not the controller's job
    var nutrition = await _http.GetAsync($"usda.gov?q={food}");
    // Saving to DB — not the controller's job
    await _db.SaveAsync(new FoodLog(food, nutrition));
    return Ok(nutrition);
}
```

**Correct (each class has one job):**
```csharp
// FoodController's ONLY job: receive the request and coordinate
public async Task<IActionResult> Scan(IFormFile file)
{
    var result = await _foodScanService.ProcessAsync(file, userId);
    return Ok(result);
}
// FoodScanService orchestrates. FoodDetectionService detects.
// VolumeAnalysisService measures. NutritionLookupService looks up.
```

---

#### O — Open/Closed Principle (OCP)
**Definition:** A class should be open for extension but closed for modification. Add new behaviour by adding new code, not by changing existing working code.

**In APEX:** `FoodDetectionService` depends on `IFoodDetectionModel` interface. Today that interface is implemented by `OnnxFoodDetectionModel`. Tomorrow, if we want to try Passio AI, we create `PassioFoodDetectionModel` — we never touch `FoodDetectionService`. The service is closed for modification, open for extension through new implementations.

```csharp
// The service is closed — never changes regardless of which AI we use
public class FoodDetectionService
{
    private readonly IFoodDetectionModel _model; // depends on abstraction
    public FoodDetectionService(IFoodDetectionModel model) => _model = model;
    public Task<FoodDetectionResult> DetectAsync(byte[] imageBytes)
        => _model.InferAsync(imageBytes); // works with ANY implementation
}

// Extension: swap AI provider by adding a new class, zero existing code changed
public class PassioFoodDetectionModel : IFoodDetectionModel { ... }
public class OnnxFoodDetectionModel  : IFoodDetectionModel { ... }
```

---

#### L — Liskov Substitution Principle (LSP)
**Definition:** Any implementation of an interface must be substitutable for any other implementation without breaking the program. If something expects `IFoodDetectionModel`, any class implementing that interface must behave correctly.

**In APEX:** `OnnxFoodDetectionModel` and `PassioFoodDetectionModel` must both return a `FoodDetectionResult` with the same shape, same null-safety contract, and same async behaviour. If `OnnxFoodDetectionModel` returns `null` on failure but `PassioFoodDetectionModel` throws an exception, LSP is violated — swapping them breaks `FoodDetectionService`.

**Rule of thumb:** If you have to add `if (model is OnnxFoodDetectionModel)` in calling code, LSP is broken.

---

#### I — Interface Segregation Principle (ISP)
**Definition:** No class should be forced to implement methods it doesn't use. Prefer many small, focused interfaces over one large one.

**In APEX:** Rather than one giant `IStorageService` with methods for S3, DynamoDB, and Cognito, we have:
- `IS3StorageService` — upload, download, presigned URLs
- `IFoodLogRepository` — save, get, delete food log entries
- `IUserRepository` — get, update user profile

A class that only needs to read food logs depends on `IFoodLogRepository` — it has no knowledge that S3 or Cognito even exist. Tests for that class mock only `IFoodLogRepository`.

---

#### D — Dependency Inversion Principle (DIP)
**Definition:** High-level modules should not depend on low-level modules. Both should depend on abstractions (interfaces). This is what makes Dependency Injection meaningful.

**In APEX:** `FoodScanService` (high level) does NOT create `new FoodDetectionService()` inside itself. It receives `IFoodDetectionService` injected by the DI container. The container decides which concrete class to provide. `FoodScanService` never knows or cares.

```csharp
// DIP applied — FoodScanService depends on interfaces, not concrete classes
public class FoodScanService
{
    // All dependencies are interfaces — DI provides the implementations
    public FoodScanService(
        IFoodDetectionService foodDetection,    // not OnnxFoodDetectionService
        IVolumeAnalysisService volumeAnalysis,  // not OpenCvVolumeService
        INutritionLookupService nutritionLookup,// not UsdaNutritionService
        IS3StorageService storage,              // not AwsS3StorageService
        IFoodLogRepository repository)          // not DynamoDbRepository
    { ... }
}
```

---

### Part C — Design Patterns used in APEX

These are the specific patterns that appear in this codebase. Claude must introduce each one with the explanation template when it first appears.

---

#### 1. Repository Pattern
**Category:** Structural / Data Access
**Files:** `DynamoDbRepository.cs`, `IFoodLogRepository.cs`, `IUserRepository.cs`

Abstracts all database access behind an interface. The rest of the application talks to `IFoodLogRepository` — it has no idea whether the data is in DynamoDB, SQL Server, or a JSON file. Swap the database by swapping the implementation; nothing else changes.

```csharp
// The interface — all the app ever sees
public interface IFoodLogRepository
{
    Task<FoodLog?> GetByIdAsync(string logId, string userId);
    Task<IEnumerable<FoodLog>> GetByDateAsync(string userId, DateOnly date);
    Task SaveAsync(FoodLog log);
    Task DeleteAsync(string logId, string userId);
}

// The implementation — DynamoDB specifics hidden here
public class DynamoDbFoodLogRepository : IFoodLogRepository
{
    private readonly IAmazonDynamoDB _dynamo;
    // DynamoDB table names, key structures, marshalling — all in here
    // The rest of the app never sees any of this
}
```

**Real world:** Every enterprise .NET application. Entity Framework itself is a Repository implementation.
**Trade-off:** Adds an extra layer and interface. For very simple queries, it can feel like over-engineering.

---

#### 2. Singleton Pattern
**Category:** Creational
**Files:** `OnnxModelLoader.cs`

Ensures a class has exactly one instance for the entire application lifetime. Critical for ONNX models — loading a neural network from disk takes 2–5 seconds and uses significant memory. Doing it per-request would make the API unusable.

```csharp
// Registered as Singleton in Program.cs — created ONCE, shared forever
builder.Services.AddSingleton<OnnxModelLoader>();

// Every service that needs ONNX gets the SAME instance injected
// The models are loaded once at startup, then reused for millisecond-fast inference
```

**Real world:** Database connection pools, configuration objects, logging systems.
**Trade-off:** Singletons are global state. If the singleton holds mutable state, it becomes a concurrency problem. `OnnxModelLoader` avoids this by being read-only after construction — `InferenceSession` is thread-safe in ONNX Runtime.

---

#### 3. Strategy Pattern
**Category:** Behavioural
**Files:** `IVolumeEstimationStrategy.cs`, `SingleFrameVolumeStrategy.cs`, `MultiFrameVolumeStrategy.cs`

Defines a family of algorithms, encapsulates each one, and makes them interchangeable at runtime. The volume estimation problem has two distinct algorithms: single-image (one depth map) and multi-frame video (averaged depth across frames). Rather than an `if/else` in `VolumeAnalysisService`, each algorithm is its own class.

```csharp
public interface IVolumeEstimationStrategy
{
    Task<VolumeEstimate> EstimateAsync(IReadOnlyList<byte[]> frames);
}

public class SingleFrameVolumeStrategy  : IVolumeEstimationStrategy { ... }
public class MultiFrameVolumeStrategy   : IVolumeEstimationStrategy { ... }

// VolumeAnalysisService picks the right strategy based on input type
// Adding a third strategy (e.g. LiDAR) = add one new class, zero changes elsewhere
```

**Real world:** Payment processors (different strategy per card type), sorting algorithms (pick QuickSort vs MergeSort based on input size), compression (JPEG vs PNG strategy).
**Trade-off:** More classes, more files. Worth it when the algorithms genuinely vary and new ones are likely.

---

#### 4. Pipeline / Chain of Responsibility Pattern
**Category:** Behavioural
**Files:** `FoodScanService.cs`

Processes a request through a sequence of steps, where each step does one thing and passes its result to the next. The food scan pipeline is a textbook example: validate → store → extract frames → detect food → analyse volume → calculate portion → lookup nutrition → save → respond.

```csharp
public async Task<FoodScanResult> ProcessAsync(IFormFile file, string userId)
{
    // Each step is a focused method call — the pipeline is readable top-to-bottom
    var s3Key        = await StoreFileAsync(file);
    var frames       = await ExtractFramesAsync(file);
    var (detection,
         volume)     = await Task.WhenAll(DetectFoodAsync(frames),
                                          AnalyseVolumeAsync(frames));
    var portion      = CalculatePortion(detection, volume);
    var nutrition    = await LookupNutritionAsync(detection.FoodName, portion.Grams);
    var log          = await SaveResultAsync(s3Key, userId, detection, nutrition);
    return BuildResult(log, detection, nutrition, volume);
}
```

**Real world:** ASP.NET Core Middleware is literally a pipeline. CI/CD pipelines. ETL data processing.
**Trade-off:** If steps need to share a lot of state, the pipeline becomes a "context object" antipattern. Keep each step's output clean and minimal.

---

#### 5. Result Pattern
**Category:** Error Handling
**Files:** `Result.cs`, used by all service methods

Returns a typed `Result<T>` object instead of throwing exceptions for expected failure cases (food not detected, plate not found, USDA returned no match). Exceptions are for unexpected failures; Results are for expected ones.

```csharp
// The Result type — success or failure, both handled explicitly
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? ErrorMessage { get; }
    public ErrorCode? Error { get; }

    private Result(T value) { IsSuccess = true; Value = value; }
    private Result(ErrorCode error, string message) { IsSuccess = false; Error = error; ErrorMessage = message; }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(ErrorCode error, string message) => new(error, message);
}

// Usage — calling code MUST handle both cases. Cannot accidentally ignore a failure.
var detection = await _foodDetectionService.DetectAsync(frame);
if (!detection.IsSuccess)
    return Result<FoodScanResult>.Failure(detection.Error!, detection.ErrorMessage!);
```

**Real world:** Rust's `Result<T, E>`, Haskell's `Either`, functional programming in general. Used extensively in domain-driven design.
**Trade-off:** More verbose than try/catch for simple cases. Return type propagation can be repetitive. Worth it when a failure is a known business case (food not found) rather than an unexpected system error.

---

#### 6. Options Pattern
**Category:** Configuration
**Files:** `appsettings.json`, `AwsOptions.cs`, `MlOptions.cs`, `UsdaOptions.cs`

Strongly-typed configuration classes that are injected via DI. Never read raw strings from `IConfiguration` inside service code. Group related settings into a class; validate them at startup.

```csharp
// Options class — all ML settings in one place, typed
public class MlOptions
{
    public string ModelsPath    { get; set; } = "./ML/Models";
    public string FoodModelName { get; set; } = "food_detection.onnx";
    public string DepthModelName{ get; set; } = "midas_depth.onnx";
    public int    MaxImageSizeMb{ get; set; } = 10;
}

// Registered in Program.cs with validation
builder.Services.AddOptions<MlOptions>()
    .BindConfiguration("ML")
    .ValidateDataAnnotations()      // validates Required, Range etc.
    .ValidateOnStart();             // fails at startup if config is wrong, not at runtime

// Injected cleanly into services
public class OnnxModelLoader(IOptions<MlOptions> options) { ... }
```

**Real world:** Standard in every ASP.NET Core application. The Microsoft.Extensions.Options package is built for this pattern.
**Trade-off:** Extra classes for what could be a single `config["ML:ModelsPath"]` call. Worth it for settings that are used in multiple places or need validation.

---

#### 7. Adapter Pattern
**Category:** Structural
**Files:** `S3StorageService.cs`, `DynamoDbRepository.cs`

Wraps an external dependency (AWS SDK, ONNX Runtime, OpenCV) behind your own interface, so the rest of the app speaks your language, not the library's language. If AWS changes its SDK API, only the Adapter changes.

```csharp
// Your interface — defined by what YOUR app needs
public interface IS3StorageService
{
    Task<string> UploadFoodImageAsync(Stream imageStream, string fileName);
    Task<Stream> DownloadAsync(string s3Key);
    Task<string> GeneratePresignedUrlAsync(string s3Key, TimeSpan expiry);
}

// The Adapter — translates between your interface and the AWS SDK
public class AwsS3StorageService : IS3StorageService
{
    private readonly IAmazonS3 _s3Client; // AWS SDK — stays inside this class
    // The rest of the app has ZERO knowledge that AWS S3 even exists
}
```

**Real world:** Payment gateway wrappers (your `IPaymentService` adapts Stripe or PayPal), database ORMs (Entity Framework adapts SQL dialects), logging wrappers (Serilog adapts to multiple sinks).
**Trade-off:** An extra layer of indirection. If you know with certainty you'll never swap the external library, the adapter is pure overhead. For AWS SDK in a cloud-native app — worth it, because testability alone justifies it.

---

#### 8. Factory Pattern
**Category:** Creational
**Files:** `VolumeEstimationStrategyFactory.cs`

Creates objects without exposing the creation logic. The caller says "I need a volume estimation strategy for a video upload" — the factory decides which concrete class to instantiate based on the input type.

```csharp
public class VolumeEstimationStrategyFactory
{
    private readonly IServiceProvider _services;

    public IVolumeEstimationStrategy Create(MediaType mediaType) =>
        mediaType switch
        {
            MediaType.Image => _services.GetRequiredService<SingleFrameVolumeStrategy>(),
            MediaType.Video => _services.GetRequiredService<MultiFrameVolumeStrategy>(),
            _ => throw new ArgumentOutOfRangeException(nameof(mediaType))
        };
}
```

**Real world:** `HttpClientFactory` in .NET, `ILoggerFactory`, ORM connection factories.
**Trade-off:** Adding a factory for a single class is overkill. Justified when object creation is conditional, complex, or needs to vary at runtime.

---

#### 9. Observer / Event Pattern (Phase 2)
**Category:** Behavioural
**Files:** AWS EventBridge integration in Phase 2

When a meal is logged, multiple independent things need to happen: update macros, check goals, generate coaching suggestion, update weekly report. Instead of calling all of them in sequence (tight coupling), the meal logging service publishes a `MealLoggedEvent` and walks away. Each subscriber reacts independently.

```csharp
// Publisher — knows nothing about what happens next
await _eventBus.PublishAsync(new MealLoggedEvent
{
    UserId    = userId,
    FoodName  = result.FoodName,
    Calories  = result.Calories,
    LoggedAt  = DateTime.UtcNow
});
// MacroUpdateHandler, GoalCheckHandler, CoachingHandler each subscribe
// independently. Adding a new handler doesn't touch the publisher.
```

**Real world:** DOM events in browsers, .NET events/delegates, AWS EventBridge, Kafka consumers.
**Trade-off:** Harder to trace what happens after an event fires. Debugging requires checking every subscriber. Use only when decoupling is genuinely needed.

---

### Part D — Coding Principles (applied to every file)

These apply at the line-by-line and method level. Claude must call these out when a decision is made because of them.

---

**DRY — Don't Repeat Yourself**
If you write the same logic twice, extract it. The nutrient ID lookup logic (`food.FoodNutrients.FirstOrDefault(n => n.NutrientId == X)?.Value ?? 0`) appears for 5 nutrients — it becomes a private helper method `GetNutrientValue(int nutrientId)`, not 5 lines of identical code.

**KISS — Keep It Simple, Stupid**
The simplest solution that works is correct. Do not add abstraction layers until the problem demands them. A single `if` statement is better than a strategy pattern when there are only two cases and a third is unlikely.

**YAGNI — You Aren't Gonna Need It**
Do not build features "just in case." Phase 2 features (EventBridge, IoT, Bedrock) do NOT get scaffolded in Phase 1. The code written in Phase 1 is the code needed in Phase 1.

**Separation of Concerns**
Controllers handle HTTP. Services handle business logic. Repositories handle data. Never put a DynamoDB query in a controller. Never put an HTTP status code decision in a service.

**Fail Fast**
Validate inputs at the earliest possible point. If a file is too large, reject it in the controller before it enters the pipeline. If a configuration value is missing, crash at startup with a clear message — not silently at runtime during a user's meal scan.

**Composition over Inheritance**
Prefer injecting services into other services rather than inheriting from them. `FoodScanService` uses `IFoodDetectionService` via composition, not inheritance. Inheritance creates tight coupling; composition keeps dependencies explicit and swappable.

**Meaningful Names**
Names should reveal intent. `ProcessAsync` is weak. `ScanFoodFromUploadAsync` is clear. `data` is bad. `foodNutrientsPer100g` is clear. `x` is a loop variable — never a field or parameter. Names are free; use them to document intent.

**Constants over Magic Numbers**
```csharp
// BAD — what does 27 mean?
double pixelsPerCm = circleRadius * 2 / 27.0;

// GOOD — intent is self-documenting
private const double StandardDinnerPlateDiameterCm = 27.0;
double pixelsPerCm = circleRadius * 2 / StandardDinnerPlateDiameterCm;
```

---

### Part E — .NET-specific coding rules

These apply to every C# file in the project:

- **async all the way down** — if a method calls an async method, it must itself be async. Never `.Result` or `.Wait()` — these cause deadlocks in ASP.NET Core.
- **CancellationToken everywhere** — every public async method accepts `CancellationToken ct = default`. Pass it through to every awaitable call. This lets the framework cancel hung requests cleanly.
- **Record types for DTOs** — `FoodScanResult`, `NutritionData`, `VolumeEstimate` are all records. Immutable, value-equality, zero boilerplate.
- **Nullable reference types enabled** — `<Nullable>enable</Nullable>` in the `.csproj`. Every nullable must be explicitly marked with `?`. No silent `NullReferenceException` surprises.
- **No `var` for non-obvious types** — `var session = new InferenceSession(...)` is fine. `var result = GetResult()` is not — the type is unclear without looking at the method signature.
- **`ILogger<T>` in every service** — structured logging with `LogInformation`, `LogWarning`, `LogError`. Never `Console.WriteLine`.
- **Guard clauses before logic** — validate and return early at the top of a method, main logic flows naturally below without nesting.

```csharp
// Guard clause pattern — early exit, flat code
public async Task<Result<FoodScanResult>> ProcessAsync(IFormFile file, string userId)
{
    if (file is null || file.Length == 0)
        return Result<FoodScanResult>.Failure(ErrorCode.InvalidInput, "No file provided");
    if (file.Length > MaxFileSizeBytes)
        return Result<FoodScanResult>.Failure(ErrorCode.FileTooLarge, $"Max size is {MaxFileSizeMb}MB");
    if (!AllowedContentTypes.Contains(file.ContentType))
        return Result<FoodScanResult>.Failure(ErrorCode.InvalidFileType, "Only jpg, png, mp4, mov accepted");

    // Main logic — no nesting, no defensive checks cluttering the flow
    var s3Key = await StoreFileAsync(file);
    // ...
}
```

---

### Part F — Pattern-to-file mapping for APEX

Quick reference showing which pattern appears in which file:

| File | Pattern(s) Applied |
|---|---|
| `FoodScanService.cs` | Pipeline, SRP, DIP, Composition |
| `OnnxModelLoader.cs` | Singleton, Options Pattern |
| `FoodDetectionService.cs` | Adapter (wraps OnnxRuntime), SRP |
| `VolumeAnalysisService.cs` | Strategy (selects algorithm), SRP |
| `VolumeEstimationStrategyFactory.cs` | Factory Pattern |
| `NutritionLookupService.cs` | Adapter (wraps USDA API via Refit), SRP |
| `S3StorageService.cs` | Adapter (wraps AWS SDK), SRP |
| `DynamoDbRepository.cs` | Repository Pattern, Adapter |
| `IFoodDetectionService.cs` | DIP, OCP, LSP, ISP |
| `Result.cs` | Result Pattern |
| `MlOptions.cs` / `AwsOptions.cs` | Options Pattern |
| `FoodController.cs` | SRP (HTTP only), Guard Clauses |
| Phase 2 event handlers | Observer Pattern |

---

## 5. Application overview

The app is a **calorie and fitness intelligence platform** — working name **APEX** (open to change).

It has two major feature branches built sequentially:

### Feature Branch 1 — Food Tracking (BUILD THIS FIRST)
- Upload a photo OR a short video (5–10 seconds) of a meal
- AI identifies the food(s) in the image
- Computer vision estimates portion size using reference objects (plate, hand) and depth mapping
- Nutrition data (calories, protein, carbs, fat) returned from the USDA food database
- User logs the meal and views their daily/weekly nutrition summary

### Feature Branch 2 — Activity & Workout Tracking (BUILD AFTER BRANCH 1)
- iPhone sensor integration via Apple HealthKit (steps, calories burned, workouts, HR)
- AirPods Pro 2 heart rate data flows through HealthKit automatically
- Garmin Connect API integration for Garmin device users
- Event-driven microservices pipeline processes workout data
- RAG-based personalised nutrition coaching (Amazon Bedrock)
- Real-time dashboard with SignalR

**Do not start Feature Branch 2 until Feature Branch 1 is functionally complete.**

---

## 6. Confirmed tech stack — DO NOT change without explicit instruction

### Backend — CURRENT BUILD FOCUS
- **Framework:** ASP.NET Core 10 Web API
- **Language:** C# 14 (.NET 10)
- **Pattern:** Controllers → Services → Repositories (clean separation)
- **Replacing:** The existing Express.js backend (that repo's React stays as reference; Express itself is retired)
- **Compute:** AWS Lambda (serverless) preferred for cost; ECS if Lambda cold starts become unacceptable
- **Tested via:** Swagger/OpenAPI UI, Postman, or curl — no UI needed to build or validate this phase

### Frontend — NOT PART OF CURRENT SCOPE (later integration phase)
- **Framework:** React (existing codebase from the old repo — do NOT rewrite or replace when the time comes)
- **Language:** JavaScript or TypeScript (match existing codebase convention)
- **Video capture:** Browser MediaRecorder API
- **Future (Phase 2):** Capacitor wrapper for iOS native HealthKit access
- **Status:** Not being scaffolded, copied, or referenced yet. Revisit this section's Node/npm setup only when Aniket explicitly starts the integration phase.

### Model Training — SEPARATE, OFFLINE, OPTIONAL TRACK (not part of the running API)
- **Language:** Python + PyTorch — used ONLY to train/fine-tune the food recognition model (transfer learning on Food-101, see Section 7)
- **Output:** A `.onnx` file — that file is the only thing that crosses into the .NET world
- **Where it runs:** Google Colab (free GPU) or Aniket's own machine — entirely separate from the `apex-api` project and its dependencies
- **Not required to build the backend:** the MVP can use a pre-trained ONNX model downloaded from HuggingFace; Python only becomes relevant if/when Aniket wants to train his own model for the portfolio story

**CONFIRMED SEQUENCING DECISION — do not reorder without asking:**
1. **Now:** Build `FoodDetectionService` against a pre-trained ONNX model downloaded from Hugging Face (search "food-101 onnx" or similar). Zero training. Get the full pipeline working end-to-end first — upload, detect, volume estimate, nutrition lookup, response.
2. **Later:** Once the pipeline is proven working, train a custom model in Python/PyTorch (Colab, free GPU) and swap the `.onnx` file in `FoodDetectionService`. Because `FoodDetectionService` depends on the model through an interface (Adapter pattern, Section 4), swapping the underlying `.onnx` file at this point should require no changes to the rest of the pipeline — that's the whole point of building it that way.
- **Do NOT suggest starting model training before the Hugging Face version is working end-to-end.** If Aniket asks about training early, remind him of this sequencing and confirm before deviating.

### Database
- **Primary (NoSQL):** Amazon DynamoDB — user data, food logs, meal history
- **File storage:** Amazon S3 — uploaded photos/videos, ONNX model files
- **Future (relational):** Amazon RDS or Aurora — reporting/analytics in Phase 2

### Authentication
- **AWS Cognito** — JWT-based auth, supports social login (Google, Apple)

### AI / ML — LOCAL (no paid external AI API for core features)
- **Food detection:** YOLOv8 ONNX model trained on Food-101 (101 food categories)
- **Depth estimation:** MiDaS ONNX model (monocular depth from single image)
- **Runtime:** `Microsoft.ML.OnnxRuntime` NuGet — runs models inside .NET, no external call
- **Fallback:** Passio AI API if ONNX proves too complex initially

### Computer Vision
- **OpenCvSharp4** — plate/circle detection (Hough Circle Transform), image preprocessing

### Video Processing
- **FFMpegCore** — extract key frames from uploaded video files

### Image Processing
- **SixLabors.ImageSharp** — resizing, format conversion, preprocessing

### Nutrition Data
- **USDA FoodData Central API** — free, 700,000+ foods, no key required for dev
- Base URL: `https://api.nal.usda.gov/fdc/v1/`

### AWS Account
- **Account type:** Standard AWS account with Free Tier + $100 USD promotional credit
- **Free Tier highlights:** 750hrs/month EC2 t2.micro, 5GB S3, 25GB DynamoDB, 1M Lambda invocations/month, 1M API Gateway calls/month — all for 12 months
- **Region:** No restrictions — use `ap-southeast-2` (Sydney) for lower latency from Brisbane, or `us-east-1` for widest service availability
- **Cost discipline:** Set a billing alert at $10 USD immediately. Use serverless wherever possible.
- **Monitoring:** Check AWS Cost Explorer weekly during active development

---

## 7. Model Sourcing Strategy — Hugging Face First, Self-Trained Later

**Confirmed decision:** Build and ship the full pipeline using a pre-trained ONNX model downloaded from Hugging Face first. Training a custom model on Food-101 via Google Colab happens later, as a parallel/follow-up track — not a blocker to getting the API working end-to-end.

### Phase A — NOW: Hugging Face pre-trained model
- Search Hugging Face for an existing Food-101 (or similar) food classification model already exported to ONNX format
- Download the `.onnx` file directly — no training, no GPU, no Python required
- Drop it into `/ML/Models/food_detection.onnx` in the `apex-api` project
- This is what gets the full pipeline (upload → detect → volume → nutrition → response) working and testable end-to-end first

### Phase B — LATER: Self-trained model via Google Colab
- Train a model by fine-tuning EfficientNet-B0 on the Food-101 dataset using transfer learning, in PyTorch, on Google Colab's free T4 GPU
- Time-boxed to a single session (~90 min unattended runtime, ~45 min hands-on) — no open-ended hyperparameter tuning
- Export the trained model to ONNX (`torch.onnx.export`)
- This is a separate, offline, optional track — it does not block or slow down Phase A work

### The swap must be a drop-in replacement, not a redesign
Because `FoodDetectionService` depends on the model only through the ONNX Runtime `InferenceSession` interface (not tied to where the file came from), swapping Phase A's Hugging Face model for Phase B's self-trained model later should require ONLY:
- Replacing the file at `/ML/Models/food_detection.onnx`
- Confirming input/output tensor names and shapes still match (resize dimensions, normalization, class label list)
- No changes to `FoodDetectionService.cs`, `FoodScanService.cs`, or any calling code

If building the initial `OnnxModelLoader` / `FoodDetectionService` code in Phase A would make this swap harder later (e.g. hardcoding assumptions specific to one model's exact class list or preprocessing), flag that to Aniket before proceeding — the class label list and preprocessing constants should live in config/JSON, not hardcoded in the service, specifically to keep this swap cheap.

### Do not treat Phase B as urgent
Phase B has no deadline and is not required for Phase 1 (food tracking backend) to be considered complete. Suggest it only when Aniket has a free block of time, or when he brings it up himself.

---

## 8. AWS Services Guide — Full Reference

This section explains every AWS service used in APEX. Claude Code must use this as the teaching reference when introducing each service.

---

### 5.1 Amazon S3 (Simple Storage Service)

```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
AWS SERVICE: Amazon S3
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
WHAT IT IS:
  Object storage. Think of it as a hard drive in the cloud where you
  store files (called "objects") inside folders (called "buckets").
  Files can be anything: images, videos, JSON, ONNX models, CSVs.
  Each object gets a unique URL. There is no limit on file size (up to 5TB).

PROBLEM IT SOLVES:
  Your .NET server shouldn't store uploaded food photos on its own disk.
  If the server restarts, scales, or is replaced, those files disappear.
  S3 is persistent, durable (99.999999999% durability), and accessible
  from anywhere — your server, your React frontend, AWS Lambda, etc.

IN THIS PROJECT:
  - Stores uploaded food photos and videos from users
  - Stores the ONNX model files (food_detection.onnx, midas_depth.onnx)
  - Stores processed result thumbnails
  Two buckets: apex-food-uploads (private), apex-models (private)

ADVANTAGES:
  - Virtually unlimited storage
  - Extremely cheap at scale
  - Integrated with every other AWS service natively
  - Built-in versioning, lifecycle policies, encryption
  - Pre-signed URLs let React upload directly (bypassing your server)

DRAWBACKS:
  - Not a file system (no folders, no append, no partial reads)
  - Eventual consistency for overwrites (usually instant but not guaranteed)
  - Data transfer OUT of S3 costs money (ingress is free)
  - Cross-region transfer is expensive — keep S3 and your compute in same region

FREE TIER:
  5 GB storage, 20,000 GET requests, 2,000 PUT requests per month — free for 12 months.
  After: ~$0.025/GB/month storage + $0.09/GB data transfer out.

NON-CLOUD EQUIVALENT:
  Storing files on your server's local disk, or running your own
  MinIO (open-source S3-compatible server).

NEXT LEVEL UP:
  Amazon CloudFront in front of S3 — a CDN that caches your S3 files
  at 400+ edge locations globally. When users download food scan results,
  CloudFront serves from the nearest location instead of your S3 bucket.
  Graduate to this when you have users in multiple countries.
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```

---

### 5.2 Amazon DynamoDB

```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
AWS SERVICE: Amazon DynamoDB
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
WHAT IT IS:
  A fully managed NoSQL key-value and document database. You store items
  (like JSON objects) identified by a partition key and optional sort key.
  There are no joins. There is no SQL. Designed for single-digit millisecond
  reads at any scale — from 1 user to 1 billion users with zero changes.

PROBLEM IT SOLVES:
  Traditional relational databases (SQL) need you to manage servers,
  tune indexes, plan for traffic spikes. DynamoDB handles all of that
  automatically. You just store and retrieve items.

IN THIS PROJECT:
  - User profiles
  - Daily food logs (each meal entry)
  - Scan processing records (track status of in-progress scans)
  Single-table design: one table called "apex-main" holds everything.

ADVANTAGES:
  - Serverless — no server to manage, scales automatically
  - Millisecond reads even at millions of requests per second
  - Pay only for what you use (on-demand mode)
  - Built-in TTL (auto-delete old records after X days)
  - Streams feature lets you react to data changes in real-time

DRAWBACKS:
  - No SQL, no joins — you must design your access patterns upfront
  - Single-table design has a learning curve
  - Expensive for large analytical/reporting queries (use RDS for that)
  - Maximum item size: 400KB (not suitable for storing large blobs)
  - Querying patterns not planned at design time require full table scans (expensive)

FREE TIER:
  25 GB storage, 25 write capacity units, 25 read capacity units — FREE FOREVER
  (not just 12 months). For this project, you will stay in free tier for a very
  long time. On-demand pricing: $1.25 per million writes, $0.25 per million reads.

NON-CLOUD EQUIVALENT:
  MongoDB (document database), Redis (key-value), or even a simple JSON file
  on disk for small scale.

NEXT LEVEL UP:
  Amazon Aurora Serverless v2 — a fully managed relational (SQL) database
  that also auto-scales. Use this when you need complex queries, reporting,
  or relationships between data that DynamoDB can't express cleanly.
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```

---

### 5.3 AWS Lambda

```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
AWS SERVICE: AWS Lambda
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
WHAT IT IS:
  Serverless compute. You upload your code (in our case, the .NET 10 Web API),
  and AWS runs it on demand. There are no servers to manage, no OS to patch,
  no always-on cost. Lambda wakes up when a request arrives, runs your code,
  and shuts down. You pay only for the milliseconds it runs.

PROBLEM IT SOLVES:
  Running a traditional server 24/7 costs money even when no users are active.
  For a new app with unpredictable traffic, Lambda is far more cost-efficient.

IN THIS PROJECT:
  The .NET ASP.NET Core 10 Web API deployed as a Lambda function using
  Amazon.Lambda.AspNetCoreServer.Hosting. API Gateway sits in front and
  routes HTTP requests to Lambda.

ADVANTAGES:
  - Zero server management
  - Scales automatically to thousands of concurrent requests
  - Pay per invocation (essentially free at low traffic)
  - Integrated with every AWS service
  - Supports .NET 10 natively

DRAWBACKS:
  - Cold starts: first request after idle period takes 1–3 seconds extra
    (.NET is heavier than Node.js — cold starts are noticeable)
  - Maximum execution time: 15 minutes (food scan pipeline is well under this)
  - Maximum memory: 10 GB (ONNX models are large — test if models fit)
  - Stateless: cannot store files locally between invocations (use S3)
  - Debugging locally requires SAM CLI or similar tooling

FREE TIER:
  1 million invocations/month + 400,000 GB-seconds of compute time — FREE FOREVER.
  For this project, you will likely never pay for Lambda.
  After free tier: $0.20 per 1 million requests.

NON-CLOUD EQUIVALENT:
  Running a .NET app on your own machine or a VPS (like a $5/month DigitalOcean droplet).

NEXT LEVEL UP:
  Amazon ECS (Elastic Container Service) with Fargate — run your .NET app
  as a Docker container with no cold starts, persistent memory, and more
  control. Graduate to ECS when cold starts from Lambda hurt user experience
  or when ONNX models are too large for Lambda's memory limits.
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```

---

### 5.4 Amazon API Gateway

```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
AWS SERVICE: Amazon API Gateway
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
WHAT IT IS:
  A managed front door for your API. It receives HTTP requests from the
  React app, applies authentication checks, rate limiting, and routing,
  then forwards the request to your Lambda function. Returns the response
  back to the client.

PROBLEM IT SOLVES:
  Your Lambda function has no public URL by default. API Gateway gives it
  a stable HTTPS endpoint. It also handles: SSL certificates, CORS headers,
  request throttling, API keys, usage plans, and WebSocket connections.

IN THIS PROJECT:
  Routes all /api/* requests to the .NET Lambda function.
  Also manages the WebSocket connection for real-time features (Phase 2).

ADVANTAGES:
  - Managed SSL (HTTPS out of the box)
  - Built-in throttling to protect your Lambda from abuse
  - Caching layer to avoid redundant Lambda calls
  - WebSocket support for real-time dashboards
  - Integrated with Cognito for JWT validation before requests hit Lambda

DRAWBACKS:
  - Adds ~10–50ms latency to every request
  - 10MB maximum request/response payload (a food video could exceed this)
    → Solution: use pre-signed S3 URLs for file uploads, not API Gateway
  - Slightly complex configuration (REST API vs HTTP API vs WebSocket API)
  - Cost adds up at very high request volumes

FREE TIER:
  1 million API calls/month free for 12 months.
  After: $3.50 per million REST API calls.
  HTTP API (newer, cheaper): $1.00 per million calls.
  Use HTTP API for this project — it's simpler and cheaper.

NON-CLOUD EQUIVALENT:
  Nginx or Caddy as a reverse proxy in front of your .NET app.
  (You already know Caddy from IFN666.)

NEXT LEVEL UP:
  Amazon CloudFront with Lambda@Edge — moves your routing and auth logic
  to the edge (400+ global locations) for sub-10ms latency.
  Graduate to this when you have a global user base.
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```

---

### 5.5 AWS Cognito

```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
AWS SERVICE: AWS Cognito
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
WHAT IT IS:
  Fully managed user authentication and authorisation. Handles: user
  sign-up, sign-in, password reset, email verification, social login
  (Google, Apple, Facebook), MFA, and JWT token issuance.

PROBLEM IT SOLVES:
  Building auth from scratch is both hard and dangerous. Cognito handles
  the security-critical parts so you don't accidentally introduce a
  vulnerability. It issues industry-standard JWT tokens your .NET API
  validates with one middleware line.

IN THIS PROJECT:
  - User accounts for APEX (sign up, sign in)
  - Issues JWT access tokens that React sends with every API request
  - .NET validates tokens using Cognito's public keys (no DB lookup needed)

ADVANTAGES:
  - No server needed — fully managed
  - Social login (Google/Apple) with minimal configuration
  - MFA support built-in
  - Integrates natively with API Gateway (Cognito Authorizer)
  - Hosted UI available (pre-built login page)

DRAWBACKS:
  - Customising the hosted login UI is limited and frustrating
  - Migration to another auth system later is very painful
  - Token refresh flow requires careful implementation on the React side
  - Advanced features (custom auth flows) have a steep learning curve
  - Not ideal if you want fully custom login UI from day one

FREE TIER:
  50,000 Monthly Active Users (MAUs) free forever.
  After: $0.0055 per MAU (so 100,000 users costs ~$275/month).
  For a portfolio project, you will never leave the free tier.

NON-CLOUD EQUIVALENT:
  Auth0, Supabase Auth, Firebase Auth, or rolling your own with
  ASP.NET Core Identity + JWT.

NEXT LEVEL UP:
  Amazon Verified Permissions — adds fine-grained authorisation policies
  (role-based access control at a granular level) on top of Cognito.
  Graduate to this when you have multiple user roles with complex permissions.
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```

---

### 5.6 Amazon CloudFront (Phase 1 optional, Phase 2 recommended)

```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
AWS SERVICE: Amazon CloudFront
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
WHAT IT IS:
  A Content Delivery Network (CDN). It copies your static files (React app,
  images, CSS, JS) to 400+ servers around the world. When a user opens your
  app, they download from the nearest server instead of from Sydney or Virginia.

PROBLEM IT SOLVES:
  Without a CDN, every user downloads your React app and food result images
  from your single S3 bucket location. A user in London gets much slower
  load times than a user in Sydney. CloudFront fixes this by serving from
  the nearest edge location.

IN THIS PROJECT:
  Hosts the React frontend (S3 bucket + CloudFront = free static hosting).
  Also caches food result thumbnails closer to users.

ADVANTAGES:
  - Massively reduces load times globally
  - DDoS protection included (AWS Shield Standard)
  - Free SSL certificate (ACM)
  - Caches API responses too (reduces Lambda invocations)
  - One stable domain name for your entire app

DRAWBACKS:
  - Cache invalidation is needed after every React deploy (takes ~5 min)
  - Adds configuration complexity
  - Debugging cached vs live responses can be confusing
  - Minimum TTL cache rules require careful thought

FREE TIER:
  1 TB data transfer out/month, 10 million HTTP/HTTPS requests/month — free for 12 months.
  After: $0.0085/GB data transfer out (much cheaper than serving from S3 directly).

NON-CLOUD EQUIVALENT:
  Netlify, Vercel, or GitHub Pages for static hosting.
  Cloudflare CDN as a free alternative CDN layer.

NEXT LEVEL UP:
  CloudFront + Lambda@Edge — run custom code at the CDN edge itself
  (e.g. A/B testing, authentication checks, response transforms at the
  edge before content even reaches the user's browser).
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```

---

### 5.7 AWS EventBridge + SQS (Phase 2)

```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
AWS SERVICES: EventBridge + SQS (used together)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
WHAT THEY ARE:
  EventBridge is an event bus — a central router for events happening
  in your system ("user logged a meal", "workout completed").
  SQS (Simple Queue Service) is a message queue — a holding area that
  stores messages until a consumer processes them, with retry logic built in.

PROBLEM THEY SOLVE:
  Without events, your code is tightly coupled. "User logs a meal"
  triggers: update macros + send notification + update weekly report +
  check goals — all in the same function, all blocking each other.
  With EventBridge + SQS, each of those becomes a separate consumer that
  processes independently. One failure doesn't crash the rest.

IN THIS PROJECT (Phase 2):
  User logs a meal → EventBridge publishes "MealLogged" event →
  SQS queues it → four Lambda functions consume independently:
  (1) update daily macros, (2) check calorie goals, (3) update weekly report,
  (4) trigger Bedrock coaching suggestion

ADVANTAGES:
  - Decoupled services — change one without touching others
  - Natural resilience — SQS retries failed messages automatically
  - Scales independently — each consumer scales based on its own load
  - Dead letter queues catch permanently failed messages for debugging

DRAWBACKS:
  - Adds architectural complexity (harder to trace a single request end-to-end)
  - Eventual consistency — the "update macros" Lambda might run 2 seconds after the meal log
  - Local testing is harder (need LocalStack or SAM)
  - Debugging requires CloudWatch Logs across multiple services

FREE TIER:
  EventBridge: 1 million events/month free forever.
  SQS: 1 million requests/month free forever.

NON-CLOUD EQUIVALENT:
  RabbitMQ or Redis Pub/Sub running on your own server.

NEXT LEVEL UP:
  Amazon MSK (Managed Streaming for Kafka) — for truly massive event
  throughput (millions of events per second). Graduate to this when
  EventBridge's throughput limits become a concern at scale.
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```

---

### 5.8 Amazon Bedrock (Phase 2)

```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
AWS SERVICE: Amazon Bedrock
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
WHAT IT IS:
  A managed service that gives you API access to large language models
  (Claude, Llama, Titan, Mistral) and the tools to build RAG (Retrieval
  Augmented Generation) applications — without managing any AI infrastructure.

PROBLEM IT SOLVES:
  Calling OpenAI directly is simple but expensive and sends your users'
  data to a third party. Bedrock keeps everything inside AWS and lets you
  connect your own data sources (nutrition research, user food history)
  to the LLM so it gives grounded, personalised answers rather than
  hallucinated generic advice.

IN THIS PROJECT (Phase 2):
  RAG nutrition coach: Bedrock Knowledge Base indexes nutrition research +
  USDA data + user's meal history. When a user asks "why am I not losing
  weight?", Bedrock retrieves relevant context and passes it to Claude
  (the model) which generates a personalised, grounded response.

ADVANTAGES:
  - No AI infrastructure to manage
  - Multiple models available (choose Claude, Llama, etc.)
  - RAG built-in with Knowledge Bases (no need to build vector DB yourself)
  - Stays inside AWS security boundary (HIPAA eligible)
  - Pay per token (no monthly subscription)

DRAWBACKS:
  - Not in Free Tier — costs per token from day one
  - Knowledge Base setup and ingestion has a learning curve
  - Response latency is higher than a direct DB query (2–10 seconds)
  - Claude model on Bedrock lags slightly behind the latest Claude versions

FREE TIER:
  No Free Tier. Pricing varies by model.
  Claude Haiku (fast, cheap): ~$0.00025 per 1K input tokens.
  Claude Sonnet: ~$0.003 per 1K input tokens.
  For a portfolio demo with light usage: expect $1–5/month.

NON-CLOUD EQUIVALENT:
  Calling the Anthropic API directly, or running a local Llama model via Ollama.

NEXT LEVEL UP:
  Amazon Bedrock Agents — LLM that can autonomously call your APIs and
  tools (e.g. "look up the user's last 7 days of meals, calculate their
  macro average, then suggest a meal plan"). Graduate to Agents when you
  want the AI to take multi-step actions, not just answer questions.
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```

---

### 5.9 AWS SageMaker (Phase 2)

```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
AWS SERVICE: AWS SageMaker
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
WHAT IT IS:
  A fully managed ML platform covering the entire ML lifecycle:
  data labelling, model training, hyperparameter tuning, model evaluation,
  deployment, and retraining pipelines (MLOps).

PROBLEM IT SOLVES:
  Training a food recognition model on your laptop takes hours and you
  can't store large datasets locally. SageMaker provides GPU instances
  on demand, tracks your experiments, versions your models, and
  automates the retrain-evaluate-deploy cycle.

IN THIS PROJECT (Phase 2):
  SageMaker Pipeline: when users correct a wrong food detection ("this
  was rice, not quinoa"), that labelled correction goes into a training
  dataset. SageMaker retrains the food detection model periodically,
  evaluates accuracy, and if it improves, automatically deploys the
  new ONNX model to S3 (which your .NET service then picks up).

ADVANTAGES:
  - Managed GPU instances for training (no GPU needed locally)
  - Experiment tracking (compare model versions)
  - Automated MLOps pipelines
  - Built-in model registry and versioning
  - Directly exports to ONNX format

DRAWBACKS:
  - Complex — steep learning curve for beginners
  - Expensive if GPU instances are left running (always clean up)
  - Overkill for initial setup — start with a pre-trained ONNX model first
  - Free Tier is very limited for training jobs

FREE TIER:
  SageMaker Studio: free.
  Training jobs: 250 hours of ml.t3.medium free for first 2 months.
  After free tier: GPU instances (ml.g4dn.xlarge) cost ~$0.74/hour.
  Always stop training instances when done — they bill by the second.

NON-CLOUD EQUIVALENT:
  Training locally with PyTorch on your machine, or using Google Colab
  (free GPU time for model training).

NEXT LEVEL UP:
  SageMaker Feature Store + SageMaker Clarify — adds a centralised
  feature repository and model bias/explainability analysis.
  Graduate to these when model fairness and production monitoring matter.
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```

---

### 5.10 AWS IoT Core (Phase 2)

```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
AWS SERVICE: AWS IoT Core
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
WHAT IT IS:
  A managed broker for IoT (Internet of Things) device messages.
  Devices connect using MQTT (a lightweight publish/subscribe protocol)
  or HTTPS, and IoT Core routes their messages to other AWS services
  (Lambda, DynamoDB, S3, etc.).

PROBLEM IT SOLVES:
  A Garmin watch or a BLE fitness device can't directly call your REST API.
  They speak MQTT, a protocol designed for low-power, low-bandwidth devices.
  IoT Core acts as the translator — devices talk MQTT to IoT Core, and IoT
  Core routes the data into your standard AWS pipeline.

IN THIS PROJECT (Phase 2):
  Receives real-time heart rate, step, and accelerometer data from
  compatible wearables. Also handles the Apple HealthKit sync bridge
  (data flows from iPhone → IoT Core → DynamoDB).

ADVANTAGES:
  - Handles millions of concurrent device connections
  - Message routing rules can filter, transform, and fan out data
  - Device shadows (store last known device state even when offline)
  - Certificate-based device authentication (very secure)

DRAWBACKS:
  - MQTT is not HTTP — requires different client libraries
  - Certificate management adds complexity
  - Not necessary if you only use Apple HealthKit (which has its own sync)
  - Debugging device connectivity issues is harder than standard HTTP

FREE TIER:
  2 million messages published/subscribed per month free for 12 months.
  After: $1.00 per million messages.

NON-CLOUD EQUIVALENT:
  Running your own Mosquitto MQTT broker on a server.

NEXT LEVEL UP:
  AWS IoT Greengrass — runs AWS Lambda functions directly ON the device
  (edge computing). Process sensor data on the phone/device before
  sending to cloud. Graduate to this for real-time local processing
  without latency to the cloud.
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```

---

### 5.11 AWS CodePipeline + CodeBuild + ECR (Phase 2 — CI/CD)

```
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
AWS SERVICES: CodePipeline + CodeBuild + ECR (CI/CD trinity)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
WHAT THEY ARE:
  CodePipeline: orchestrates the release pipeline (source → build → test → deploy)
  CodeBuild: managed build server that compiles your .NET code and runs tests
  ECR (Elastic Container Registry): Docker image registry (like DockerHub but private and in AWS)

PROBLEM THEY SOLVE:
  Without CI/CD, deploying means manually building, uploading, and restarting.
  One mistake and production is broken. This trio automates it:
  push to GitHub → CodePipeline triggers → CodeBuild compiles and tests →
  Docker image pushed to ECR → ECS/Lambda updated automatically.

IN THIS PROJECT (Phase 2):
  push to main branch → build .NET API → run unit tests → push Docker image
  to ECR → deploy new image to ECS → zero-downtime rolling deployment

ADVANTAGES:
  - Fully automated — push code and forget
  - Failed tests block the deployment automatically
  - Rollback to previous version in one click
  - Audit trail of every deployment
  - Integrated with GitHub, Bitbucket, CodeCommit

DRAWBACKS:
  - Non-trivial setup — buildspec.yml has a learning curve
  - CodeBuild charges per build minute (can add up)
  - Debugging failed builds requires reading CloudWatch Logs
  - Overkill for solo development early on (GitHub Actions is simpler to start)

FREE TIER:
  CodePipeline: 1 pipeline free for 12 months.
  CodeBuild: 100 build minutes/month free for 12 months.
  ECR: 500MB storage free for 12 months.

NON-CLOUD EQUIVALENT:
  GitHub Actions (free for public repos, simpler to configure, and
  recommended as a starting point before moving to CodePipeline).

NEXT LEVEL UP:
  AWS CDK (Cloud Development Kit) — define your entire infrastructure as
  TypeScript or C# code. Your pipeline, Lambda config, DynamoDB tables,
  S3 buckets — all as code, version controlled, reproducible.
  Graduate to CDK when you want Infrastructure as Code done properly.
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```

---

## 9. Key NuGet packages for the .NET backend

```xml
<!-- Core -->
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="10.*" />

<!-- ONNX inference -->
<PackageReference Include="Microsoft.ML.OnnxRuntime" Version="1.18.*" />

<!-- Computer vision -->
<PackageReference Include="OpenCvSharp4" Version="4.*" />
<PackageReference Include="OpenCvSharp4.runtime.ubuntu.22.04-x64" Version="4.*" />

<!-- Video frame extraction -->
<PackageReference Include="FFMpegCore" Version="5.*" />

<!-- Image processing -->
<PackageReference Include="SixLabors.ImageSharp" Version="3.*" />

<!-- AWS SDK -->
<PackageReference Include="AWSSDK.S3" Version="3.*" />
<PackageReference Include="AWSSDK.DynamoDBv2" Version="3.*" />
<PackageReference Include="AWSSDK.CognitoIdentityProvider" Version="3.*" />
<PackageReference Include="Amazon.Lambda.AspNetCoreServer.Hosting" Version="1.*" />

<!-- Auth -->
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="10.*" />

<!-- HTTP client for USDA API -->
<PackageReference Include="Refit" Version="7.*" />
```

---

## 10. Food tracking pipeline — DETAILED (Feature Branch 1)

### 9.1 User flow — TARGET END-STATE DESCRIPTION, not current work

> This describes what the feature looks like once the React frontend is eventually wired up. Right now, in the backend-only phase, this same flow is validated by calling the API directly (Swagger UI, Postman, or curl with a sample image file) — no UI exists yet and none is being built in this phase.

1. User opens food log screen
2. User taps "Scan food" — two options:
   - **Take photo** — single image
   - **Record video** — 5–10 seconds, slowly circle phone around plate
3. File uploads as multipart/form-data to .NET API
4. Loading state shown (3–8 seconds expected)
5. Result displayed: food name, grams, calories, protein, carbs, fat
6. User adjusts portion size if needed (slider/text input)
7. User confirms → logged to daily diary

### 9.2 API endpoint contract

```
POST /api/food/scan
Content-Type: multipart/form-data
Body: { file: [image or video], userId: string }

Response 200:
{
  "foodName": "Grilled Chicken Breast",
  "confidence": 0.91,
  "estimatedGrams": 185,
  "calories": 285,
  "protein": 53.4,
  "carbohydrates": 0,
  "fat": 6.2,
  "portionDescription": "~185g (about 3/4 of a standard plate portion)",
  "referenceObjectDetected": "plate",
  "processingMethod": "video-multiframe"
}
```

### 9.3 Backend processing pipeline

```
Step 1: File received
  → Validate file type (jpg, png, mp4, mov, webm accepted)
  → Upload raw file to S3 bucket: apex-food-uploads
  → Store S3 key in DynamoDB with status "processing"

Step 2: Frame extraction (video only)
  → If video: use FFMpegCore to extract 6 evenly spaced frames
  → If image: treat as single-frame array
  → Select sharpest frame as "primary frame" for food detection

Step 3: Parallel processing — Task.WhenAll()
  → Task A: Food Detection
      → Preprocess primary frame: resize to 640×640, normalise to [0,1]
      → Run YOLOv8 ONNX model inference
      → Parse bounding boxes and class labels
      → Return: foodName, confidence, boundingBox

  → Task B: Volume Analysis
      → Sub-step B1: Plate detection (OpenCV Hough Circle)
          → Convert to greyscale, Gaussian blur
          → Detect circular plate → get diameter in pixels
          → Known reference: standard plate = 27cm diameter
          → Calculate pixel-to-cm ratio
          → Fallback: if no plate, use default ratio
      → Sub-step B2: Depth estimation (MiDaS ONNX)
          → Preprocess frame for MiDaS (384×384, normalise)
          → Run MiDaS inference → relative depth map
          → Scale using pixel-to-cm ratio from B1
          → Estimate food region height (cm)
      → Sub-step B3 (video only):
          → Run B1+B2 across all frames
          → Average depth estimates for better accuracy
      → Return: estimatedHeightCm, pixelPerCm, referenceDetected

Step 4: Portion calculation
  → Food 2D area (cm²) = bounding box area ÷ pixelPerCm²
  → Volume (cm³) = area × estimatedHeightCm
  → Weight (g) = volume × density (looked up from food_density_table.json)

Step 5: Nutrition lookup
  → GET https://api.nal.usda.gov/fdc/v1/foods/search?query={foodName}
  → Extract per-100g: calories, protein, carbs, fat, fibre
  → Scale by estimatedGrams / 100

Step 6: Response + store
  → Assemble result object
  → Update DynamoDB record: status "complete", store result
  → Return JSON to React
```

### 9.4 Project folder structure

```
/ApexApi
  /Controllers
    FoodController.cs         ← POST /api/food/scan, GET /api/food/log
    UserController.cs
    AuthController.cs
  /Services
    FoodScanService.cs        ← pipeline orchestrator (Steps 1–6)
    FoodDetectionService.cs   ← ONNX YOLOv8 inference
    VolumeAnalysisService.cs  ← plate detection + MiDaS
    PortionCalculationService.cs
    NutritionLookupService.cs ← USDA API
    VideoFrameService.cs      ← FFMpegCore frame extraction
  /Infrastructure
    S3StorageService.cs
    DynamoDbRepository.cs
    CognitoAuthService.cs
  /Models
    FoodScanRequest.cs
    FoodScanResult.cs
    NutritionData.cs
    FoodLog.cs
    UserProfile.cs
  /ML
    /Models
      food_detection.onnx
      midas_depth.onnx
    /Data
      food_density_table.json
    OnnxModelLoader.cs        ← singleton, loads models on startup
  /Middleware
    ErrorHandlingMiddleware.cs
    AuthMiddleware.cs
  Program.cs
  appsettings.json
  appsettings.Development.json
```

---

## 11. DynamoDB schema (single-table design)

```
Table name: apex-main
Partition key (PK): string
Sort key (SK): string

User profile:
  PK: USER#{userId}
  SK: PROFILE
  Attributes: name, email, createdAt, dailyCalorieGoal

Food log entry:
  PK: USER#{userId}
  SK: FOODLOG#{date}#{timestamp}
  Attributes: foodName, estimatedGrams, calories, protein, carbs, fat,
              confidence, referenceObjectDetected, s3ImageKey, processingMethod, loggedAt

Scan status record:
  PK: SCAN#{scanId}
  SK: STATUS
  Attributes: status (processing|complete|failed), userId, s3Key, result (JSON), createdAt, completedAt
  TTL: 7 days (auto-delete after processing complete)
```

---

## 12. API routes (Feature Branch 1)

```
POST   /api/food/scan              Upload image/video, trigger pipeline
GET    /api/food/log               Get food log (query: userId, date)
POST   /api/food/log               Manually log a food item
DELETE /api/food/log/{logId}       Remove log entry
GET    /api/food/nutrition/{name}  Lookup nutrition for named food
GET    /api/user/profile           Get user profile
PUT    /api/user/profile           Update profile (calorie goals, etc.)
POST   /api/auth/validate          Validate Cognito JWT
```

---

## 13. Environment configuration

```json
{
  "AWS": {
    "Region": "ap-southeast-2",
    "S3BucketName": "apex-food-uploads",
    "DynamoDbTableName": "apex-main"
  },
  "Cognito": {
    "UserPoolId": "",
    "ClientId": "",
    "Authority": "https://cognito-idp.ap-southeast-2.amazonaws.com/{UserPoolId}"
  },
  "USDA": {
    "ApiKey": "DEMO_KEY",
    "BaseUrl": "https://api.nal.usda.gov/fdc/v1"
  },
  "ML": {
    "ModelsPath": "./ML/Models",
    "FoodModelName": "food_detection.onnx",
    "DepthModelName": "midas_depth.onnx"
  }
}
```

Never commit real values. Use AWS Secrets Manager or environment variables in production.

---

## 14. ONNX model loading pattern

```csharp
// OnnxModelLoader.cs — register as Singleton in DI
// Models loaded ONCE at startup, reused for every request
public class OnnxModelLoader
{
    public InferenceSession FoodDetectionSession { get; }
    public InferenceSession DepthEstimationSession { get; }

    public OnnxModelLoader(IConfiguration config)
    {
        var path = config["ML:ModelsPath"];
        var options = new SessionOptions();
        options.GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL;

        FoodDetectionSession = new InferenceSession(
            Path.Combine(path, config["ML:FoodModelName"]), options);
        DepthEstimationSession = new InferenceSession(
            Path.Combine(path, config["ML:DepthModelName"]), options);
    }
}

// Program.cs
builder.Services.AddSingleton<OnnxModelLoader>();
```

### Where to get the models
- **YOLOv8 food model:** HuggingFace — search "food-101 yolov8 onnx". Or export from Ultralytics: `yolo export model=yolov8n-cls.pt format=onnx` after training on Food-101.
- **MiDaS depth model:** https://github.com/isl-org/MiDaS — download `MiDaS_small.onnx` directly.

---

## 15. OpenCV plate detection parameters

```csharp
Mat grey = new Mat();
Cv2.CvtColor(frame, grey, ColorConversionCodes.BGR2GRAY);
Cv2.GaussianBlur(grey, grey, new Size(9, 9), 2);

CircleSegment[] circles = Cv2.HoughCircles(
    grey,
    HoughModes.Gradient,
    dp: 1.2,
    minDist: grey.Rows / 4,
    param1: 100,
    param2: 30,
    minRadius: grey.Rows / 6,
    maxRadius: grey.Rows / 2
);
// Largest circle = plate. Diameter assumed = 27cm.
// pixelsPerCm = (circle.Radius * 2) / 27.0
```

---

## 16. Food density table

Store as `/ML/Data/food_density_table.json`:

```json
{
  "default": 0.9,
  "chicken": 1.05, "beef": 1.10, "fish": 1.00, "pork": 1.05,
  "rice_cooked": 0.85, "pasta_cooked": 0.80, "bread": 0.30,
  "salad_leafy": 0.20, "potato": 0.95, "vegetables": 0.60,
  "fruit": 0.85, "soup_liquid": 1.00, "cheese": 1.10,
  "eggs": 1.03, "yogurt": 1.05, "nuts": 0.60, "legumes_cooked": 0.90
}
```

---

## 17. USDA API nutrient IDs

```
1008 = Energy (kcal)    1003 = Protein (g)
1005 = Carbohydrates (g) 1004 = Total fat (g)
1079 = Fibre (g)         1093 = Sodium (mg)
```
All values per 100g. Scale by `estimatedGrams / 100`.

---

## 18. Development phases and checklist

### Phase 1 — Food tracking backend (CURRENT — .NET ONLY, no frontend work)
- [ ] ASP.NET Core 10 project setup, folder structure
- [ ] DI configuration, middleware, CORS (CORS config can be written now, tested once frontend exists)
- [ ] S3StorageService
- [ ] DynamoDbRepository
- [ ] Cognito JWT middleware
- [ ] OnnxModelLoader singleton
- [ ] VideoFrameService (FFMpegCore)
- [ ] FoodDetectionService (YOLOv8 ONNX)
- [ ] VolumeAnalysisService (OpenCV + MiDaS)
- [ ] PortionCalculationService
- [ ] NutritionLookupService (USDA API)
- [ ] FoodScanService (orchestrator)
- [ ] FoodController
- [ ] End-to-end test with a real photo via Swagger UI or Postman — NOT via a UI
- [ ] Deploy to AWS (Lambda + API Gateway)

### Phase 1.5 — Frontend integration (LATER — explicit go-ahead needed before starting)
- [ ] Revisit Section 1's scope note and Section 2's environment checks — add Node/npm back in
- [ ] Copy old React client code into `apex-frontend/` per Section 20
- [ ] Wire React frontend to the now-working .NET API (replace old Express calls)
- [ ] Confirm CORS settings work against the real frontend origin

### Phase 2 — Activity tracking (AFTER Phase 1.5)
- [ ] Capacitor wrapper for React
- [ ] HealthKit plugin integration
- [ ] Garmin Connect API
- [ ] WorkoutController + WorkoutService
- [ ] EventBridge + SQS event-driven pipeline
- [ ] AWS IoT Core wearable data ingestion
- [ ] SignalR real-time dashboard
- [ ] Amazon Bedrock RAG nutrition coach
- [ ] SageMaker MLOps retraining pipeline
- [ ] CI/CD with CodePipeline or GitHub Actions

---

## 19. Important constraints

- **Current phase is .NET backend only** — do not scaffold, install, or reference anything React/Node.js/frontend until Aniket explicitly starts Phase 1.5
- **Do NOT replace React** when that phase does arrive — only the old Express backend is being replaced
- **Do NOT use paid AI APIs** for core food detection — use ONNX local models
- **Passio AI is acceptable fallback** if ONNX proves too complex, but document the trade-off
- **Always async/await** — no blocking I/O in the .NET backend
- **ONNX inference in Task.Run()** — offload CPU-heavy inference from the ASP.NET thread pool
- **Resilient pipeline** — a failed plate detection must NOT fail the whole scan; fall back to default ratio
- **ILogger<T> everywhere** — log inference times, error details with structured logging
- **Interface-based services** — all services injectable as interfaces for testability
- **Set AWS billing alert at $10 USD immediately** on account creation

---

## 20. Git and project structure — FRESH REPOSITORY, BACKEND ONLY FOR NOW

**Decision made:** This is a brand new, standalone GitHub repository — NOT a folder added inside the old fitness-tracker repo. The old repo (React + Express) stays untouched as a reference/backup and is never modified as part of this project.

**Current scope reminder:** Per Section 1's scope note, the frontend does NOT get copied into this repo yet. `apex-frontend/` does not exist until the Phase 1.5 integration step. Building it in now — even as an empty placeholder — adds nothing and creates a stale folder that sits untouched for weeks. Add it when it's actually needed.

**Why a fresh repo (for context, don't re-litigate this with Aniket):**
- A clean commit history tells a better story to interviewers than old JS-learning commits mixed with new .NET work
- The Express backend is being fully retired, not extended — this is a new system, not a feature branch
- Structuring it as `apex-api/` inside the repo root (rather than the repo root itself being the API project) leaves room to add `apex-frontend/` later without restructuring anything

### Target folder structure — RIGHT NOW (Phase 1)

```
apex-nutrition-platform/       ← repo root (name confirmed with Aniket, or similar)
  ├── apex-api/                 ← ASP.NET Core 10 backend — built from scratch here
  ├── CLAUDE.md                 ← this file
  ├── README.md
  ├── .gitignore
  └── docker-compose.yml        ← LocalStack for local AWS emulation
```

### Target folder structure — LATER (once Phase 1.5 begins)

```
apex-nutrition-platform/
  ├── apex-frontend/            ← added at Phase 1.5 — FRESH COPY of old client code, git history stripped
  ├── apex-api/
  ├── CLAUDE.md
  ├── README.md
  ├── .gitignore
  └── docker-compose.yml
```

### THIS IS A SETUP TASK — Claude Code should perform it, not just describe it

When Aniket asks to begin the project, Claude Code's first job (after the Section 2 environment checks) is to actually set up the Phase 1 repository structure on disk. This includes:

1. Confirm the repo name and target parent directory with Aniket (don't assume a path)
2. Run `git init` at the new repo root
3. Create `apex-api/` as an empty folder (Phase 1 scaffolding happens separately, see Section 10)
4. Create a `.gitignore` appropriate for a .NET project (bin/, obj/, appsettings.*.json secrets, etc.) — a React-specific `.gitignore` entry (node_modules, build/) can be added later at Phase 1.5 rather than now
5. Create a first-pass `README.md` stub (can be expanded later)
6. Make the initial commit
7. Ask Aniket whether to connect a GitHub remote now or later — do NOT push to GitHub without explicit confirmation of the remote URL

**Do NOT copy the old React client code into this repo during Phase 1.** That step (with the `.git` stripping, etc.) only happens at Phase 1.5, described in Section 18.

**Follow the Learning Protocol (Section 3) for all of this** — per Rule 1, explain what each step is about to do before doing it (e.g. why `.gitignore` matters before the first commit rather than after), then proceed. This is a good candidate for a Rule 1a handoff if GitHub remote creation is involved — that step happens in the browser, not the terminal.

---

## 21. Local development setup

```yaml
# docker-compose.yml — LocalStack for S3 + DynamoDB locally
services:
  localstack:
    image: localstack/localstack
    ports:
      - "4566:4566"
    environment:
      - SERVICES=s3,dynamodb
      - DEFAULT_REGION=ap-southeast-2
```

FFMpeg must be installed locally:
- macOS: `brew install ffmpeg`
- Ubuntu/WSL: `apt install ffmpeg`

---

## 22. Development style and code conventions

- **C# naming:** PascalCase for types/methods/properties, camelCase for locals/parameters
- **Record types** for immutable DTOs and response models
- **Result pattern** (not exceptions) for expected failure cases
- **Inline comments** on all ONNX inference code, OpenCV parameters, and DynamoDB access patterns
- **No magic numbers** — named constants for plate diameter, model input sizes, etc.
- **Prefer simple and readable** over clever — this codebase will be read by interviewers

---

## 23. Quick reference — external resources

| Resource | URL | Notes |
|---|---|---|
| USDA FoodData Central | https://api.nal.usda.gov/fdc/v1 | Free, DEMO_KEY for dev |
| USDA API docs | https://fdc.nal.usda.gov/api-guide.html | Full reference |
| MiDaS GitHub | https://github.com/isl-org/MiDaS | Download MiDaS_small.onnx |
| Passio AI (fallback) | https://www.passio.ai | Food recognition API |
| ONNX Runtime .NET docs | https://onnxruntime.ai/docs/get-started/with-csharp.html | Official C# guide |
| OpenCvSharp4 | https://github.com/shimat/opencvsharp | .NET OpenCV wrapper |
| FFMpegCore | https://github.com/rosenbjerg/FFMpegCore | Video frame extraction |
| LocalStack | https://github.com/localstack/localstack | Local AWS emulation |
| AWS Free Tier details | https://aws.amazon.com/free | Exact limits per service |
| AWS Cost Explorer | https://console.aws.amazon.com/cost-management | Monitor spend |
| AWS Billing Alerts | https://console.aws.amazon.com/billing/home#/preferences | Set $10 alert first |

---

*Last updated: July 2026. This file reflects all decisions made across the full planning conversation, including: the fresh standalone repository decision (Section 20), the Model Sourcing Strategy (Section 7), .NET 10 LTS / C# 14 as the confirmed target framework, the confirmed collaboration model (Section 3, Rule 1), Claude Code model selection guidance (Section 2), and the current backend-first scope — no React/Node.js work happens until Aniket explicitly starts the Phase 1.5 frontend integration step (see scope note in Section 1). Always check with Aniket before changing any fundamental architectural decision.*
