namespace ApexApi.Models;

// Per-100g nutrition values as returned by the USDA lookup, before scaling
// to the estimated portion size. See CLAUDE.md Section 17 for nutrient IDs.
public record NutritionData(
    double CaloriesPer100g,
    double ProteinPer100g,
    double CarbohydratesPer100g,
    double FatPer100g,
    double FibrePer100g);
