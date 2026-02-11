using DeckLens.API.Mappers;
using DeckLens.API.Models.DTO;
using DeckLens.API.Services.Interface;
using Microsoft.Extensions.Caching.Distributed;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace DeckLens.API.Services.Implementation
{
    public class ScryfallService : IScryfallService
    {
        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _cache;
        

        public ScryfallService(HttpClient httpClient, IDistributedCache cache)
        {
            this._httpClient = httpClient;
            _cache = cache;
        }

        public async Task<CardDto?> GetCardByNameAsync(string name)
        {
            //Create Redis Key
            var normalized = name.Trim().ToLowerInvariant();
            var cacheKey = $"card:named:{normalized}";

            //Check Cache
            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
            {
                return JsonSerializer.Deserialize<CardDto>(cached);
            }

            //Send Request To Scryfall API
            var encodedName = Uri.EscapeDataString(name ?? "");
            var card = await _httpClient.GetAsync($"/cards/named?exact={encodedName}");

            //Check Status
            if (!card.IsSuccessStatusCode)
            {
                return null;
            }

            //Parse Response
            var content = await card.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            //Map Response To DTO
            var response = ScryfallCardMapper.Map(root);

            //Cache
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
