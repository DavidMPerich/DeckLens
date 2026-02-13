using StackExchange.Redis;

namespace DeckLens.API.Models.DTO
{
    public class DeckAnalysisDto
    {
        public CardDto? Commander {  get; set; }
        public int TotalCards { get; set; }
        public double AverageCmc { get; set; }
        public Dictionary<int, int> ManaCurve { get; set; } = new();
        public Dictionary<string, int> ColorDistribution { get; set; } = new();
        public Dictionary<string, int> CardTypeBreakdown { get; set; } = new();
    }
}
