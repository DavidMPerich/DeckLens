using DeckLens.API.Mappers;
using DeckLens.API.Models.DTO;
using DeckLens.API.Services.Interface;
using Microsoft.Extensions.Caching.Distributed;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace DeckLens.API.Services.Implementation
{
    public class CardService : ICardService
    {
        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _cache;
        private readonly IWebHostEnvironment _env;

        public CardService(HttpClient httpClient, IDistributedCache cache, IWebHostEnvironment env)
        {
            this._httpClient = httpClient;
            _cache = cache;
            _env = env;
        }

        public async Task<List<CardDto>> GetDeckMetrics(string deck)
        {
            var path = Path.Combine(_env.ContentRootPath, "Data", "deck.txt");
            var lines = await File.ReadAllLinesAsync(path);

            var cards = lines.ToList();

            //Format List
            cards.RemoveAt(3);
            cards.RemoveAt(2);
            cards.RemoveAt(0);
            for (int i = 0; i < cards.Count; i++)
            {
                cards[i] = cards[i].Split("(")[0].Trim();
                var quantity = int.Parse(cards[i].Split(" ")[0].Trim());
                var name = cards[i].Split(" ", 2)[1].Trim();
                if (quantity > 1)
                {
                    cards.RemoveAt(i);
                    for (int j = 0; j < quantity; j++)
                    {
                        cards.Insert(i, $"1 {name}");
                    }
                }
                cards[i] = name;
            }

            //Compile Deck
            List<CardDto> compiledDeck = new List<CardDto>();

            foreach (var card in cards)
            {
                compiledDeck.Add(await GetByNameAsync(card));
            }

            //Get Metrics

            return compiledDeck;
        }

        public async Task<CardDto?> GetByNameAsync(string name)
        {
            var normalized = name.Trim().ToLowerInvariant();
            var cacheKey = $"card:named:{normalized}";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
            {
                return JsonSerializer.Deserialize<CardDto>(cached);
            }

            var encodedName = Uri.EscapeDataString(name ?? "");
            var card = await _httpClient.GetAsync($"/cards/named?exact={encodedName}");

            if (!card.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await card.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            var response = ScryfallCardMapper.Map(root);

            var serialized = JsonSerializer.Serialize(response);
            await _cache.SetStringAsync(
                cacheKey,
                serialized,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
                });

            return response;
        }
    }
}
