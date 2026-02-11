using DeckLens.API.Models.DTO;
using DeckLens.API.Services.Interface;

namespace DeckLens.API.Services.Implementation
{
    public class DeckAnalysisService : IDeckAnalysisService
    {
        private readonly IScryfallService _scryfallService;

        public DeckAnalysisService(IScryfallService scryfallService)
        {
            _scryfallService = scryfallService;
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

            //Analysis


            return new DeckAnalysisDto();
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
