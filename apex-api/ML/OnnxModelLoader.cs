using ApexApi.Options;
using Microsoft.Extensions.Options;
using Microsoft.ML.OnnxRuntime;

namespace ApexApi.ML;

// Singleton Pattern (CLAUDE.md Section 4, Pattern 2): loading an ONNX model from
// disk into an InferenceSession takes 2-5 seconds and holds significant memory —
// doing that per-request would make the API unusable. One instance is shared for
// the app's whole lifetime; InferenceSession is thread-safe in ONNX Runtime, so
// this doesn't introduce a concurrency problem the way mutable singleton state would.
//
// Lazy-loaded rather than loaded in the constructor: the model files
// (food_detection.onnx, midas_depth.onnx) aren't sourced yet (CLAUDE.md Section 7
// Phase A is still pending). A normal eager singleton would throw at DI-container
// build time and crash the ENTIRE API, not just the food-scan feature — violating
// the Resilient Pipeline principle (Section 19). Deferring construction to first
// use means the app starts fine today; only FoodDetectionService itself fails
// until the model file exists, which is the correct blast radius.
public class OnnxModelLoader : IDisposable
{
    private readonly Lazy<InferenceSession> _foodDetectionSession;
    private readonly Lazy<InferenceSession> _depthEstimationSession;

    public OnnxModelLoader(IOptions<MlOptions> mlOptions, ILogger<OnnxModelLoader> logger)
    {
        var options = mlOptions.Value;

        _foodDetectionSession = new Lazy<InferenceSession>(() =>
            CreateSession(options.ModelsPath, options.FoodModelName, logger));

        _depthEstimationSession = new Lazy<InferenceSession>(() =>
            CreateSession(options.ModelsPath, options.DepthModelName, logger));
    }

    public InferenceSession FoodDetectionSession => _foodDetectionSession.Value;
    public InferenceSession DepthEstimationSession => _depthEstimationSession.Value;

    private static InferenceSession CreateSession(string modelsPath, string modelFileName, ILogger logger)
    {
        var fullPath = Path.Combine(modelsPath, modelFileName);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException(
                $"ONNX model not found at '{fullPath}'. See CLAUDE.md Section 7 Phase A: " +
                "download a pre-trained model from Hugging Face and place it here.",
                fullPath);
        }

        // Fully qualified: ASP.NET Core's own SessionOptions (HTTP session state)
        // collides by name with ONNX Runtime's via this project's implicit usings.
        var sessionOptions = new Microsoft.ML.OnnxRuntime.SessionOptions
        {
            GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL,
        };

        logger.LogInformation("Loading ONNX model from {ModelPath}", fullPath);
        return new InferenceSession(fullPath, sessionOptions);
    }

    public void Dispose()
    {
        if (_foodDetectionSession.IsValueCreated)
            _foodDetectionSession.Value.Dispose();

        if (_depthEstimationSession.IsValueCreated)
            _depthEstimationSession.Value.Dispose();
    }
}
