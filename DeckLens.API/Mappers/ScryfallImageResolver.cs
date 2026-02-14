using System.Text.Json;

namespace DeckLens.API.Mappers
{
    public static class ScryfallImageResolver
    {
        public static string? GetNormalImageUri(JsonElement root)
        {
            if (TryGetImageUri(root, out var uri))
                return uri;

            if (root.TryGetProperty("card_faces", out var faces) &&
                faces.ValueKind == JsonValueKind.Array)
            {
                foreach (var face in faces.EnumerateArray())
                {
                    if (TryGetImageUri(face, out uri))
                        return uri;
                }
            }

            return null;
        }

        private static bool TryGetImageUri(JsonElement element, out string? uri)
        {
            uri = null;

            if (!element.TryGetProperty("image_uris", out var imageUris) ||
                imageUris.ValueKind != JsonValueKind.Object)
                return false;

            if (!imageUris.TryGetProperty("normal", out var normal) ||
                normal.ValueKind == JsonValueKind.Null)
                return false;

            uri = normal.GetString();
            return uri != null;
        }
    }
}
