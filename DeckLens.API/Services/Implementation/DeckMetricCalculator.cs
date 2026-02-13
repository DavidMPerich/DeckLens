using DeckLens.API.Models.DTO;
using DeckLens.API.Services.Interface;
using System.Reflection.Metadata.Ecma335;

namespace DeckLens.API.Services.Implementation
{
    public class DeckMetricCalculator : IDeckMetricCalculator
    {
        public DeckAnalysisDto Build(List<CardDto> cards)
        {
            var dto = new DeckAnalysisDto();

            dto.TotalCards = cards.Count;

            return dto;
        }

        private double CalculateAverageCmc(List<CardDto> cards) 
        { 
            throw new NotImplementedException();
        }

        private Dictionary<int, int> BuildManaCurve(List<CardDto> cards) 
        {
            throw new NotImplementedException();
        }

        private Dictionary<string, int> BuildColorDistribution(List<CardDto> cards)
        {
            throw new NotImplementedException();
        }

        private Dictionary<string, int> BuildTypeBreakdown(List<CardDto> cards)
        {
            throw new NotImplementedException();
        }

        private CardDto? DetectCommander(List<CardDto> cards)
        {
            throw new NotImplementedException();
        }
    }
}
