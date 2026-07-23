namespace ApexApi.Models;

// Output of VolumeAnalysisService.EstimateAsync — plate-relative depth analysis
// used by PortionCalculationService to convert a 2D bounding box into grams.
// See CLAUDE.md Section 10.3 Step 3B and Section 15 for how PixelsPerCm is derived.
public record VolumeEstimate(
    double EstimatedHeightCm,
    double PixelsPerCm,
    string? ReferenceObjectDetected);
