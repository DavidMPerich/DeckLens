using StackExchange.Redis;

namespace DeckLens.API.Models.DTO
{
    public class DeckAnalysisDto
    {
        //Dashboard Metrics
        public CardDto? Commander {  get; set; }
        public int TotalCards { get; set; }
        public DeckSummaryDto Summary { get; set; } = new(); 

        //Metric Details
        public ManaCurveAnalysisDto ManaCurveAnalysis { get; set; } = new();
        //public CardTypeAnalysisDto CardTypeAnalysis { get; set; } = new();
        //public RoleAnalysisDto RoleAnalysis { get; set; } = new();
        //public SynergyAnalysisDto SynergyAnalysis { get; set; } = new();
        //public TempoAnalysisDto TempoAnalysis { get; set; } = new();
        //public WinConAnalysisDto WinConAnalysis { get; set; } = new();
    }
}
