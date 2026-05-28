using System.Drawing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Dog_Exploder.Services;

internal static class DogImageService
{
    private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(15) };
    private static readonly Dictionary<string, Image?> _cache = new(StringComparer.OrdinalIgnoreCase);
    private static readonly SemaphoreSlim _lock = new(1, 1);

    public static async Task<Image?> GetImageAsync(string breedName, CancellationToken ct = default)
    {
        var slug = ToSlug(breedName);
        if (string.IsNullOrEmpty(slug)) return null;

        await _lock.WaitAsync(ct);
        try { if (_cache.TryGetValue(slug, out var cached)) return cached; }
        finally { _lock.Release(); }

        Image? img = null;
        try
        {
            var meta = await _http.GetFromJsonAsync<DogCeoResponse>(
                $"{AppConfig.DogImageApiBaseUrl}api/breed/{slug}/images/random", ct);
            if (meta?.Status == "success" && !string.IsNullOrEmpty(meta.Message))
            {
                using var stream = await _http.GetStreamAsync(meta.Message, ct);
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms, ct);
                ms.Position = 0;
                img = Image.FromStream(ms);
            }
        }
        catch { img = null; }

        await _lock.WaitAsync(ct);
        try { _cache[slug] = img; }
        finally { _lock.Release(); }

        return img;
    }

    public static void ClearCache()
    {
        _lock.Wait();
        try
        {
            foreach (var v in _cache.Values) v?.Dispose();
            _cache.Clear();
        }
        finally { _lock.Release(); }
    }

    private static string ToSlug(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "";
        var first = name.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
        return first.ToLowerInvariant();
    }

    private class DogCeoResponse
    {
        [JsonPropertyName("message")] public string? Message { get; set; }
        [JsonPropertyName("status")] public string? Status { get; set; }
    }
}
