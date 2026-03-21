namespace DeckLens.API.Models.DTO
{
    public class ManaCurveChartBreakdownsDto
    {
        public ManaCurveChartDto ByCmc { get; set; } = new();
        public ManaCurveChartDto ByColor { get; set; } = new();
        public ManaCurveChartDto ByType { get; set; } = new();
        public ManaCurveChartDto ByCreatureSplit { get; set; } = new();
    }
}
