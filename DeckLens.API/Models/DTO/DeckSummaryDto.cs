namespace DeckLens.API.Models.DTO
{
    public class DeckSummaryDto
    {
        //Overview
        public CardDto? Commander { get; set; }
        public int TotalCards { get; set; }

        //Metric Previews
        public Dictionary<int, int> ManaCurvePreview { get; set; } = new();
    }
}
