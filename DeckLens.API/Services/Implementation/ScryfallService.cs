using DeckLens.API.Mappers;
using DeckLens.API.Models.DTO;
using DeckLens.API.Services.Interface;
using Microsoft.Extensions.Caching.Distributed;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace DeckLens.API.Services.Implementation
{
    public sealed class ScryfallService : IScryfallService
    {
        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _cache;
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
        

        public ScryfallService(HttpClient httpClient, IDistributedCache cache)
        {
            this._httpClient = httpClient;
            _cache = cache;
        }

        public async Task<(List<CardDto> Cards, List<string> NotFound)> GetCardCollectionAsync(IEnumerable<string> names, TimeSpan ttl)
        {
            //Normalize and Preserve Original Order
            var requested = names
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => new
                {
                    Original = n.Trim(),
                    Normalized = NormalizeName(n),
                    CacheKey = BuildCacheKey(NormalizeName(n))
                })
                .ToList();

            //Remove Duplicates
            var unique = requested
                .GroupBy(x => x.Normalized)
                .Select(g => g.First())
                .ToList();

            var resultsByNormalized = new Dictionary<string, CardDto>(StringComparer.OrdinalIgnoreCase);
            var misses = new List<(string Original, string Normalized)>();

            //Check Cache
            foreach (var item in unique)
            {
                var cachedJson = await _cache.GetStringAsync(item.CacheKey);

                if (!string.IsNullOrWhiteSpace(cachedJson))
                {
                    var dto = JsonSerializer.Deserialize<CardDto>(cachedJson, JsonOptions);
                    if (dto != null)
                    {
                        resultsByNormalized[item.Normalized] = dto;
                        continue;
                    }
                }

                misses.Add((item.Original, item.Normalized));
            }

            var notFound = new List<string>();

            //Fetch Cards Not Found in Cache
            foreach (var batch in misses.Chunk(75)) //Scryfall Limit is 75
            {
                var payload = new
                {
                    identifiers = batch.Select(b => new { name = b.Original }).ToList()
                };

                using var response = await _httpClient.PostAsJsonAsync("/cards/collection", payload);
                if (!response.IsSuccessStatusCode)
                {
                    notFound.AddRange(batch.Select(b => b.Original));
                    continue;
                }

                using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                var root = doc.RootElement;

                //Data
                if (root.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array)
                {
                    foreach (var cardElement in data.EnumerateArray())
                    {
                        var cardDto = ScryfallCardMapper.Map(cardElement);
                        var cardDtoNorm = NormalizeName(cardDto.CardName);
                        resultsByNormalized[cardDtoNorm] = cardDto;

                        var cacheKey = BuildCacheKey(cardDtoNorm);
                        var json = JsonSerializer.Serialize(cardDto, JsonOptions);

                        await _cache.SetStringAsync(
                            cacheKey,
                            json,
                            new DistributedCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = ttl
                            });
                    }
                }

                //Not Found
                if (root.TryGetProperty("not_found", out var nf) && nf.ValueKind == JsonValueKind.Array)
                {
                    foreach (var nfElement in nf.EnumerateArray())
                    {
                        if (nfElement.TryGetProperty("name", out var nameElement))
                        {
                            var name = nameElement.GetString();
                            if (!string.IsNullOrWhiteSpace(name))
                            {
                                notFound.Add(name);
                            }
                        }
                    }
                }
            }


            //Return Card List in Correct Order
            var ordered = requested
                .Select(r => resultsByNormalized.TryGetValue(r.Normalized, out var cardDto) ? cardDto : null)
                .Where(cardDto =>  cardDto != null)
                .Cast<CardDto>()
                .ToList();

            return (ordered, notFound);
        }

        private static string NormalizeName(string name)
            => name.Trim().ToLowerInvariant();

        private static string BuildCacheKey(string normalizedName)
            => $"card:named:{normalizedName}";

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
