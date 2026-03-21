namespace DeckLens.API.Models.DTO
{
    public class ManaCurveMetricsDto
    {
        public double AverageCmc { get; set; }
        public double MedianCmc { get; set; }
        public int CurvePeak { get; set; }
        public double EarlyGameDensity { get; set; }
    }
}
