using DeckLens.API.Models.DTO;
using DeckLens.API.Services.Interface;

namespace DeckLens.API.Services.Implementation
{
    public class DeckAnalysisService : IDeckAnalysisService
    {
        private readonly IScryfallService _scryfallService;
        private readonly IDeckMetricCalculator _metrics;

        public DeckAnalysisService(IScryfallService scryfallService, IDeckMetricCalculator metrics)
        {
            _scryfallService = scryfallService;
            _metrics = metrics;
        }

        public async Task<DeckAnalysisDto> AnalyzeAsync(List<string> cardNames)
        {
            var cards = new List<CardDto>();

            foreach (var name in cardNames)
            {
                var card = await _scryfallService.GetCardByNameAsync(name);
                if (card != null)
                {
                    cards.Add(card);
                }
            }

            return _metrics.Build(cards);
        }

        public async Task<List<CardDto>> VerifyAsync(List<string> cardNames)
        {
            var cards = new List<CardDto>();

            foreach (var name in cardNames)
            {
                var card = await _scryfallService.GetCardByNameAsync(name);
                if (card != null)
                {
                    cards.Add(card);
                }
            }

            return cards;
        }
    }
}
