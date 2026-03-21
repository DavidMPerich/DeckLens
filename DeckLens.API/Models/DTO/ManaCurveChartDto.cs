namespace DeckLens.API.Models.DTO
{
    public class ManaCurveChartDto
    {
        public List<int> Categories { get; set; } = new();
        public List<StackedSeriesDto> Series { get; set; } = new();
    }
}
