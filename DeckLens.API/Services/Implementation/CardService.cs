using DeckLens.API.Models.DTO;
using DeckLens.API.Services.Interface;
using System.Text.Json;

namespace DeckLens.API.Services.Implementation
{
    public class CardService : ICardService
    {
        private readonly HttpClient _httpClient;

        public CardService(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }

        public async Task<CardDto?> GetByNameAsync(string name)
        {
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
                TypeLine = root.GetProperty("type_line").GetString()!,
                OracleText = root.GetProperty("oracle_text").GetString()!,
            };

            return response;
        }
    }
}
