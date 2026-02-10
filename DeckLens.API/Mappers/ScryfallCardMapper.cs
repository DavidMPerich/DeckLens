using DeckLens.API.Models.DTO;
using System.Text.Json;

namespace DeckLens.API.Mappers
{
    public static class ScryfallCardMapper
    {
        public static CardDto Map(JsonElement root)
        {
            return new CardDto
            {
                CardName = GetStringOrNull(root, "name")!,
                ManaCost = GetStringOrNull(root, "mana_cost"),
                ConvertedManaCost = GetDoubleOrNull(root, "cmc"),
                TypeLine = GetStringOrNull(root, "type_line"),
                OracleText = GetStringOrNull(root, "oracle_text"),

                Power = GetStringOrNull(root, "power"),
                Toughness = GetStringOrNull(root, "toughness"),

                Colors = GetStringList(root, "colors"),
                ColorIdentity = GetStringList(root, "color_identity"),
                Keywords = GetStringList(root, "keywords"),

                Rarity = GetStringOrNull(root, "rarity"),
                EDHRecRank = GetIntOrNull(root, "edhrec_rank")
            };
        }

        private static string? GetStringOrNull(JsonElement root, string propertyName)
        {
            return root.TryGetProperty(propertyName, out var prop) &&
                   prop.ValueKind != JsonValueKind.Null
                ? prop.GetString()
                : null;
        }

        private static int? GetIntOrNull(JsonElement root, string propertyName)
        {
            return root.TryGetProperty(propertyName, out var prop) &&
                   prop.ValueKind == JsonValueKind.Number
                ? prop.GetInt32()
                : null;
        }

        private static double? GetDoubleOrNull(JsonElement root, string propertyName)
        {
            return root.TryGetProperty(propertyName, out var prop) &&
                   prop.ValueKind == JsonValueKind.Number
                ? prop.GetDouble()
                : null;
        }

        private static List<string> GetStringList(JsonElement root, string propertyName)
        {
            return root.TryGetProperty(propertyName, out var prop) &&
                   prop.ValueKind == JsonValueKind.Array
                ? prop.EnumerateArray()
                      .Select(e => e.GetString())
                      .Where(s => s != null)
                      .ToList()!
                : new List<string>();
        }
    }
}
