namespace ApexApi.Models;

// API response shape for POST /api/food/scan — matches CLAUDE.md Section 10.2 exactly.
public record FoodScanResult(
    string FoodName,
    double Confidence,
    double EstimatedGrams,
    double Calories,
    double Protein,
    double Carbohydrates,
    double Fat,
    string PortionDescription,
    string? ReferenceObjectDetected,
    string ProcessingMethod);
