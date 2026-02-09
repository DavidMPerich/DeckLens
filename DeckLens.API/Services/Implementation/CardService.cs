using DeckLens.API.Models.DTO;
using DeckLens.API.Services.Interface;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace DeckLens.API.Services.Implementation
{
    public class CardService : ICardService
    {
        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _cache;

        public CardService(HttpClient httpClient, IDistributedCache cache)
        {
            this._httpClient = httpClient;
            _cache = cache;
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

            var response = new CardDto
            {
                CardName = root.GetProperty("name").GetString()!,
                ManaCost = root.GetProperty("mana_cost").GetString()!,
                ConvertedManaCost = root.GetProperty("cmc").GetDouble()!,
                TypeLine = root.GetProperty("type_line").GetString()!,
                OracleText = root.GetProperty("oracle_text").GetString()!,
                Power = root.GetProperty("power").GetString()!,
                Toughness = root.GetProperty("toughness").GetString()!,
                Colors = root.TryGetProperty("colors", out var colorsElement) ? colorsElement.EnumerateArray().Select(k => k.GetString()!).ToList() : new List<string>(),
                ColorIdentity = root.TryGetProperty("color_identity", out var colorIdentityElement) ? colorIdentityElement.EnumerateArray().Select(k => k.GetString()!).ToList() : new List<string>(),
                Keywords = root.TryGetProperty("keywords", out var keywordsElement) ? keywordsElement.EnumerateArray().Select(k => k.GetString()!).ToList() : new List<string>(),
                Rarity = root.GetProperty("rarity").GetString()!,
                EDHRecRank = root.GetProperty("edhrec_rank").GetInt32()!,
            };

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
