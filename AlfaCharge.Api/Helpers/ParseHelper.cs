using System.Text.Json;

namespace AlfaCharge.Api.Helpers
{
    public static class ParseHelper
    {
        public static bool IsAny(string? value, params string[] allowed) =>
                    value != null && allowed.Any(a => string.Equals(a, value, StringComparison.OrdinalIgnoreCase));

        public static string NormalizeEnum(string value, params string[] allowed)
        {
            var match = allowed.FirstOrDefault(a => string.Equals(a, value, StringComparison.OrdinalIgnoreCase));
            return match ?? value;
        }

        public static object TryParseOrEcho(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new { };
            try { return JsonDocument.Parse(json!).RootElement; }
            catch { return new { raw = json }; }
        }
    }
}