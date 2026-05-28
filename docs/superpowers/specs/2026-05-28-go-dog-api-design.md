---
name: go-dog-api-design
description: Design spec for a local Go API server cloning dogapi.dog endpoints, plus appsettings.json config for the Dog-Exploder app
metadata:
  type: project
---

# Go Dog API — Design Spec

**Date:** 2026-05-28  
**Status:** Approved

## Problem

`dogapi.dog` (the public API backing breed and group data) is permanently inaccessible. The app needs a local replacement that serves the same JSON:API shape so zero app logic changes are required, except swapping the base URL.

## Scope

- **In scope:** Clone `GET /api/v2/breeds` and `GET /api/v2/groups` from dogapi.dog.
- **Out of scope:** `dog.ceo` image API — it remains called directly from the app.
- **Out of scope:** Any write endpoints, auth, or real-time data sync.

---

## Part 1 — Go API Server

### Location

`../go-dog-api/` (sibling directory to `Dog-Exploder/`)

### Directory structure

```
go-dog-api/
├── main.go           # HTTP server, routing, handlers, pagination helper
├── data/
│   ├── breeds.json   # ~20 breed records in JSON:API item format
│   └── groups.json   # ~8 group records in JSON:API item format
└── go.mod            # module: go-dog-api, go 1.21, zero external dependencies
```

### Endpoints

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/v2/breeds?page[number]={n}` | Paginated list of breeds |
| GET | `/api/v2/groups?page[number]={n}` | Paginated list of groups |

- **Page size:** 10 records per page (matches dogapi.dog behaviour).
- **Default page:** 1 if `page[number]` is absent or invalid.
- **404:** Any other path returns `404 Not Found`.

### Response shape

Identical to dogapi.dog JSON:API format — the C# DTOs in `DogApiService.cs` must parse without changes.

**Breeds response:**
```json
{
  "data": [
    {
      "id": "<uuid>",
      "type": "breed",
      "attributes": {
        "name": "Golden Retriever",
        "description": "...",
        "life":           { "min": 10, "max": 12 },
        "male_weight":    { "min": 29, "max": 34 },
        "female_weight":  { "min": 25, "max": 29 },
        "male_height":    { "min": 58, "max": 61 },
        "female_height":  { "min": 55, "max": 57 },
        "hypoallergenic": false
      },
      "relationships": {
        "group": { "data": { "id": "<group-uuid>", "type": "group" } }
      }
    }
  ],
  "meta": {
    "pagination": { "current": 1, "last": 2 }
  }
}
```

**Groups response:**
```json
{
  "data": [
    {
      "id": "<uuid>",
      "type": "group",
      "attributes": { "name": "Sporting" }
    }
  ],
  "meta": {
    "pagination": { "current": 1, "last": 1 }
  }
}
```

### Data files

`data/breeds.json` and `data/groups.json` contain raw arrays of JSON:API resource objects (no pagination wrapper — the server adds that at runtime). Files are read once at startup into memory.

**Groups (~8 records):** Sporting, Hound, Working, Terrier, Toy, Non-Sporting, Herding, Miscellaneous

**Breeds (~20 records):** Golden Retriever, Labrador Retriever, German Shepherd, French Bulldog, Bulldog, Poodle (Standard), Beagle, Rottweiler, German Shorthaired Pointer, Dachshund, Pembroke Welsh Corgi, Australian Shepherd, Yorkshire Terrier, Boxer, Siberian Husky, Great Dane, Doberman Pinscher, Shih Tzu, Miniature Schnauzer, Border Collie

Each breed record has realistic `min`/`max` values for life, weights (kg), and heights (cm), plus a `hypoallergenic` bool and a `relationships.group.data.id` pointing to one of the 8 group UUIDs.

### Server config

- **Default port:** `8080`
- **Override:** `PORT` environment variable (`os.Getenv("PORT")`)
- **Binding:** `0.0.0.0:{PORT}` so it's reachable from `localhost`

### Implementation notes

- All logic in `main.go` — no sub-packages needed at this scale.
- `main()` reads both JSON files, stores slices in package-level vars, starts `http.ListenAndServe`.
- A single `paginate(data []json.RawMessage, page, pageSize int)` helper slices the data and computes `last`.
- Response `Content-Type: application/json; charset=utf-8`.
- No CORS headers needed (same-machine loopback calls from WinForms).

### Build & run

```powershell
cd ../go-dog-api
go run .          # dev
go build -o go-dog-api.exe .   # prod
./go-dog-api.exe
```

---

## Part 2 — App config (`Dog-Exploder/`)

### New file: `appsettings.json`

Placed at project root. Visual Studio project property: **Copy to Output Directory = Always**.

```json
{
  "DogApiBaseUrl": "http://localhost:8080/",
  "DogImageApiBaseUrl": "https://dog.ceo/"
}
```

To switch back to the real API (if it recovers), change `DogApiBaseUrl` to `https://dogapi.dog/` — no rebuild required.

### New file: `AppConfig.cs`

```
Dog-Exploder/
└── AppConfig.cs
```

- Namespace: `Dog_Exploder`
- Static class with two `static string` properties: `DogApiBaseUrl`, `DogImageApiBaseUrl`
- Reads `appsettings.json` via `System.Text.Json` on first access (lazy, thread-safe via `Lazy<T>`)
- Falls back to original hardcoded URLs if the file is missing, so the app degrades gracefully

### Changes to existing services

**`Services/DogApiService.cs`**
- Replace `BaseAddress = new Uri("https://dogapi.dog/")` → `BaseAddress = new Uri(AppConfig.DogApiBaseUrl)`

**`Services/DogImageService.cs`**
- The current `_http` client has no BaseAddress; the full URL is constructed inline.
- Extract the dog.ceo base from `AppConfig.DogImageApiBaseUrl` when building the request URL.
- Specifically: replace the hardcoded `"https://dog.ceo/api/breed/{slug}/images/random"` string with `$"{AppConfig.DogImageApiBaseUrl}api/breed/{slug}/images/random"` (trailing slash already in config value).

No other files change.

---

## Summary of deliverables

| Deliverable | Location | New/Modified |
|-------------|----------|--------------|
| `main.go` | `../go-dog-api/main.go` | New |
| `go.mod` | `../go-dog-api/go.mod` | New |
| `data/breeds.json` | `../go-dog-api/data/breeds.json` | New |
| `data/groups.json` | `../go-dog-api/data/groups.json` | New |
| `appsettings.json` | `Dog-Exploder/appsettings.json` | New |
| `AppConfig.cs` | `Dog-Exploder/AppConfig.cs` | New |
| `DogApiService.cs` | `Dog-Exploder/Services/DogApiService.cs` | Modified (1 line) |
| `DogImageService.cs` | `Dog-Exploder/Services/DogImageService.cs` | Modified (1 line) |
