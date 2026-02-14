using System.Text.Json;

namespace DeckLens.API.Mappers
{
    public static class ScryfallFieldResolver
    {
        public static string? GetString(JsonElement root, string prop)
            => TryGetNonNullString(root, prop, out var value) ? value : null;

        public static string? GetStringOrJoinFaces(JsonElement root, string prop, string separator)
        {
            if (TryGetNonNullString(root, prop, out var top))
                return top;

            var faces = GetFaces(root);
            if (faces == null) return null;

            var parts = faces.Value
                .EnumerateArray()
                .Select(face => TryGetNonNullString(face, prop, out var v) ? v : null)
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .ToList();

            return parts.Count == 0 ? null : string.Join(separator, parts);
        }

        public static List<string> GetStringListOrUnionFaces(JsonElement root, string prop)
        {
            var top = TryGetStringArray(root, prop, out var list) ? list : null;

            
            if (top is { Count: > 0 })
                return top;

            
            var faces = GetFaces(root);
            if (faces == null) return new List<string>();

            return faces.Value
                .EnumerateArray()
                .SelectMany(face => TryGetStringArray(face, prop, out var arr) ? arr : Enumerable.Empty<string>())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .ToList();
        }

        public static (string? Power, string? Toughness) GetPowerToughness(JsonElement root)
        {
            var pTop = GetString(root, "power");
            var tTop = GetString(root, "toughness");
            if (!string.IsNullOrWhiteSpace(pTop) || !string.IsNullOrWhiteSpace(tTop))
                return (pTop, tTop);

            var faces = GetFaces(root);
            if (faces == null) return (null, null);

            var pParts = new List<string>();
            var tParts = new List<string>();

            foreach (var face in faces.Value.EnumerateArray())
            {
                var p = GetString(face, "power");
                var t = GetString(face, "toughness");

                if (!string.IsNullOrWhiteSpace(p)) pParts.Add(p);
                if (!string.IsNullOrWhiteSpace(t)) tParts.Add(t);
            }

            return (
                pParts.Count == 0 ? null : string.Join(" // ", pParts),
                tParts.Count == 0 ? null : string.Join(" // ", tParts)
            );
        }

        public static string? GetNormalImageUri(JsonElement root)
        {
            if (TryGetImageUri(root, out var uri))
                return uri;

            var faces = GetFaces(root);
            if (faces == null) return null;

            foreach (var face in faces.Value.EnumerateArray())
                if (TryGetImageUri(face, out uri))
                    return uri;

            return null;
        }

        private static JsonElement? GetFaces(JsonElement root)
            => root.TryGetProperty("card_faces", out var faces) && faces.ValueKind == JsonValueKind.Array
                ? faces
                : (JsonElement?)null;

        private static bool TryGetNonNullString(JsonElement root, string prop, out string? value)
        {
            value = null;
            if (!root.TryGetProperty(prop, out var el) || el.ValueKind == JsonValueKind.Null)
                return false;

            value = el.GetString();
            return value != null;
        }

        private static bool TryGetStringArray(JsonElement root, string prop, out List<string> list)
        {
            list = new List<string>();

            if (!root.TryGetProperty(prop, out var el) || el.ValueKind != JsonValueKind.Array)
                return false;

            foreach (var item in el.EnumerateArray())
            {
                var s = item.GetString();
                if (!string.IsNullOrWhiteSpace(s))
                    list.Add(s);
            }

            return true;
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
