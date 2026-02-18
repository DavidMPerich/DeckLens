using DeckLens.API.Models.DTO;
using DeckLens.API.Services.Interface;

namespace DeckLens.API.Services.Implementation
{
    public class DeckAnalysisService : IDeckAnalysisService
    {
        private readonly IScryfallService _scryfallService;
        private readonly IDeckMetricCalculator _metrics;
        private readonly ILogger<DeckAnalysisService> _logger;
        private readonly SemaphoreSlim _semaphore = new(10);

        public DeckAnalysisService(IScryfallService scryfallService, IDeckMetricCalculator metrics, ILogger<DeckAnalysisService> logger)
        {
            _scryfallService = scryfallService;
            _metrics = metrics;
            _logger = logger;
        }

        public async Task<DeckAnalysisDto> AnalyzeAsync(List<string> cardNames)
        {
            var (cards, notFound) = await _scryfallService.GetCardCollectionAsync(
                cardNames,
                TimeSpan.FromDays(7)
            );

            if (notFound.Count > 0)
            {
                foreach (var name in notFound)
                {
                    _logger.LogWarning("Card not found: {CardName}", name);
                }
            }

            return _metrics.Build(cards);
        }

        public async Task<List<CardDto>> VerifyAsync(List<string> cardNames)
        {
            var (cards, notFound) = await _scryfallService.GetCardCollectionAsync(
                cardNames,
                TimeSpan.FromDays(7)
            );

            if (notFound.Count > 0)
            {
                foreach (var name in notFound)
                {
                    //_logger.LogWarning("Card not found: {CardName}", name);
                }
            }

            return cards;
        }
    }
}
