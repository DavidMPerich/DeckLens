namespace DeckLens.API.Models.DTO
{
    public class ManaCurveAnalysisDto
    {
        //Metrics
        public double AverageCmc { get; set; }

        //Chart Distribution Types
        public Dictionary<int, int> ByCmc { get; set; } = new();

        //Insights
    }
}
