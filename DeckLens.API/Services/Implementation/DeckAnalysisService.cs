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
            //TODO: Implmement Hybrid approach to scryfall service (get collection on missed requests)

            var tasks = cardNames.Select(async name =>
            {
                await _semaphore.WaitAsync();
                try
                {
                    return await _scryfallService.GetCardByNameAsync(name);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Failed to fetch card {name}");
                    return null;
                }
                finally
                {
                    _semaphore.Release();
                }
            });
                
            var results = await Task.WhenAll(tasks);

            var cards = results
                .Where(c =>  c != null)
                .ToList();

            return _metrics.Build(cards);
        }

        public async Task<List<CardDto>> VerifyAsync(List<string> cardNames)
        {
            var tasks = cardNames.Select(async name =>
            {
                await _semaphore.WaitAsync();
                try
                {
                    return await _scryfallService.GetCardByNameAsync(name);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Failed to fetch card {name}");
                    return null;
                }
                finally
                {
                    _semaphore.Release();
                }
            });

            var results = await Task.WhenAll(tasks);

            var cards = results
                .Where(c => c != null)
                .ToList();

            return cards;
        }
    }
}
