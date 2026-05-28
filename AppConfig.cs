using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dog_Exploder;

internal static class AppConfig
{
    private static readonly Lazy<ConfigData> _data =
        new(Load, LazyThreadSafetyMode.ExecutionAndPublication);

    public static string DogApiBaseUrl      => _data.Value.DogApiBaseUrl;
    public static string DogImageApiBaseUrl => _data.Value.DogImageApiBaseUrl;

    private static ConfigData Load()
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
        if (!File.Exists(path))
            return new ConfigData();
        try
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<ConfigData>(json) ?? new ConfigData();
        }
        catch
        {
            return new ConfigData();
        }
    }

    private sealed class ConfigData
    {
        [JsonPropertyName("DogApiBaseUrl")]
        public string DogApiBaseUrl { get; set; } = "https://dogapi.dog/";

        [JsonPropertyName("DogImageApiBaseUrl")]
        public string DogImageApiBaseUrl { get; set; } = "https://dog.ceo/";
    }
}
