using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dog_Exploder.Models;

namespace Dog_Exploder.Services;

internal static class DogApiService
{
    private static readonly HttpClient _http = new()
    {
        BaseAddress = new Uri(AppConfig.DogApiBaseUrl),
        Timeout = TimeSpan.FromSeconds(30)
    };

    public static async Task<List<Breed>> GetAllBreedsAsync(CancellationToken ct = default)
    {
        var all = new List<Breed>();
        int page = 1;
        while (true)
        {
            ct.ThrowIfCancellationRequested();
            var url = $"api/v2/breeds?page[number]={page}";
            var resp = await _http.GetFromJsonAsync<JsonApiResponse<BreedDto>>(url, ct)
                       ?? throw new InvalidOperationException("Empty response");
            foreach (var d in resp.Data) all.Add(MapBreed(d));
            int last = resp.Meta?.Pagination?.Last ?? page;
            if (page >= last) break;
            page++;
        }
        return all;
    }

    public static async Task<List<Group>> GetGroupsAsync(CancellationToken ct = default)
    {
        var all = new List<Group>();
        int page = 1;
        while (true)
        {
            ct.ThrowIfCancellationRequested();
            var url = $"api/v2/groups?page[number]={page}";
            var resp = await _http.GetFromJsonAsync<JsonApiResponse<GroupDto>>(url, ct)
                       ?? throw new InvalidOperationException("Empty response");
            foreach (var d in resp.Data)
                all.Add(new Group { Id = d.Id, Name = d.Attributes?.Name ?? "" });
            int last = resp.Meta?.Pagination?.Last ?? page;
            if (page >= last) break;
            page++;
        }
        return all;
    }

    public static void ResolveGroupNames(List<Breed> breeds, List<Group> groups)
    {
        var byId = groups.ToDictionary(g => g.Id, g => g.Name);
        foreach (var b in breeds)
            if (b.GroupId != null && byId.TryGetValue(b.GroupId, out var name))
                b.GroupName = name;
    }

    private static Breed MapBreed(BreedDto d)
    {
        var a = d.Attributes ?? new BreedAttrDto();
        return new Breed
        {
            Id = d.Id,
            Name = a.Name ?? "",
            Description = a.Description ?? "",
            Life = ToRange(a.Life),
            MaleWeight = ToRange(a.MaleWeight),
            FemaleWeight = ToRange(a.FemaleWeight),
            MaleHeight = ToRange(a.MaleHeight),
            FemaleHeight = ToRange(a.FemaleHeight),
            Hypoallergenic = a.Hypoallergenic,
            GroupId = d.Relationships?.Group?.Data?.Id
        };
    }

    private static BreedRange ToRange(RangeDto? r) =>
        r is null ? new BreedRange() : new BreedRange { Min = r.Min, Max = r.Max };

    // --- DTOs matching JSON:API shape ---
    private class JsonApiResponse<T> { [JsonPropertyName("data")] public List<T> Data { get; set; } = new(); [JsonPropertyName("meta")] public MetaDto? Meta { get; set; } }
    private class MetaDto             { [JsonPropertyName("pagination")] public PaginationDto? Pagination { get; set; } }
    private class PaginationDto       { [JsonPropertyName("current")] public int Current { get; set; } [JsonPropertyName("last")] public int Last { get; set; } }
    private class BreedDto            { [JsonPropertyName("id")] public string Id { get; set; } = ""; [JsonPropertyName("attributes")] public BreedAttrDto? Attributes { get; set; } [JsonPropertyName("relationships")] public BreedRelDto? Relationships { get; set; } }
    private class GroupDto            { [JsonPropertyName("id")] public string Id { get; set; } = ""; [JsonPropertyName("attributes")] public GroupAttrDto? Attributes { get; set; } }
    private class GroupAttrDto        { [JsonPropertyName("name")] public string? Name { get; set; } }
    private class BreedAttrDto
    {
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("description")] public string? Description { get; set; }
        [JsonPropertyName("life")] public RangeDto? Life { get; set; }
        [JsonPropertyName("male_weight")] public RangeDto? MaleWeight { get; set; }
        [JsonPropertyName("female_weight")] public RangeDto? FemaleWeight { get; set; }
        [JsonPropertyName("male_height")] public RangeDto? MaleHeight { get; set; }
        [JsonPropertyName("female_height")] public RangeDto? FemaleHeight { get; set; }
        [JsonPropertyName("hypoallergenic")] public bool Hypoallergenic { get; set; }
    }
    private class RangeDto { [JsonPropertyName("min")] public int Min { get; set; } [JsonPropertyName("max")] public int Max { get; set; } }
    private class BreedRelDto { [JsonPropertyName("group")] public RelLinkDto? Group { get; set; } }
    private class RelLinkDto { [JsonPropertyName("data")] public RelDataDto? Data { get; set; } }
    private class RelDataDto { [JsonPropertyName("id")] public string Id { get; set; } = ""; }
}
