using DeckLens.API.Models.DTO;
using System.Text.Json;

namespace DeckLens.API.Mappers
{
    public static class ScryfallCardMapper
    {
        public static CardDto Map(JsonElement root)
        {
            var (power, toughness) = ScryfallFieldResolver.GetPowerToughness(root);

            return new CardDto
            {
                CardName = ScryfallFieldResolver.GetString(root, "name")!,
                ManaCost = ScryfallFieldResolver.GetStringOrJoinFaces(root, "mana_cost", " // "),
                ConvertedManaCost = GetDoubleOrNull(root, "cmc"),
                TypeLine = ScryfallFieldResolver.GetStringOrJoinFaces(root, "type_line", " // "),
                OracleText = ScryfallFieldResolver.GetStringOrJoinFaces(root, "oracle_text", "\n\n//\n\n"),
                Power = power,
                Toughness = toughness,
                Colors = ScryfallFieldResolver.GetStringListOrUnionFaces(root, "colors"),
                ColorIdentity = ScryfallFieldResolver.GetStringListOrUnionFaces(root, "color_identity"),
                Keywords = ScryfallFieldResolver.GetStringListOrUnionFaces(root, "keywords"),
                Rarity = GetStringOrNull(root, "rarity"),
                EDHRecRank = GetIntOrNull(root, "edhrec_rank"),
                ImageUri = ScryfallFieldResolver.GetNormalImageUri(root)
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
    }
}
