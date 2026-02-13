using DeckLens.API.Models.DTO;

namespace DeckLens.API.Services.Interface
{
    public interface IDeckMetricCalculator
    {
        public DeckAnalysisDto Build(List<CardDto> cards);
    }
}
