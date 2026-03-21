namespace DeckLens.API.Models.DTO
{
    public class ManaCurveAnalysisDto
    {
        public ManaCurveMetricsDto Metrics { get; set; } = new();
        public ManaCurveChartBreakdownsDto Charts { get; set; } = new();

    }
}
