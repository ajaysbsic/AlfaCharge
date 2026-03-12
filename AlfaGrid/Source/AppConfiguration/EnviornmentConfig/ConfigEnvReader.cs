namespace AlfaGrid.Source.AppConfiguration.EnviornmentConfig
{
    internal static class ConfigEnvReader
    {
        public static string Get(string key, string fallback = "")
            => Environment.GetEnvironmentVariable(key) ?? fallback;

        public static string[] GetCsv(string key, string fallbackCsv)
        {
            var value = Environment.GetEnvironmentVariable(key);
            var effective = string.IsNullOrWhiteSpace(value) ? fallbackCsv : value;

            return effective
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }
    }
}
