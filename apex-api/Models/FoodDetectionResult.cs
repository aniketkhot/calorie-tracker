namespace ApexApi.Models;

// Output of FoodDetectionService.DetectAsync — a YOLOv8 bounding box plus its
// class label and confidence. BoundingBoxArea is in pixels^2 of the source frame.
public record FoodDetectionResult(
    string FoodName,
    double Confidence,
    double BoundingBoxArea);
