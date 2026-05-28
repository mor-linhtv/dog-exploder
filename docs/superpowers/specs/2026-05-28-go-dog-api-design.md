---
name: go-dog-api-design
description: App-side config changes to support a configurable base URL for the dog breeds API, replacing hardcoded dogapi.dog
metadata:
  type: project
---

# App Config — Configurable API Base URL

**Date:** 2026-05-28  
**Status:** Approved

## Problem

`DogApiService` and `DogImageService` hardcode their base URLs. Switching to a local replacement API requires a code change and rebuild.

## Solution

Introduce `appsettings.json` + `AppConfig.cs` so base URLs are runtime-configurable without a rebuild.

---

## API Contract (what the app expects)

The app calls two external APIs. Both base URLs must be configurable.

**Breeds/Groups API** (`DogApiBaseUrl`):
- `GET {base}/api/v2/breeds?page[number]={n}` — JSON:API paginated breeds
- `GET {base}/api/v2/groups?page[number]={n}` — JSON:API paginated groups
- Response shape must match the DTOs in `DogApiService.cs`

**Image API** (`DogImageApiBaseUrl`):
- `GET {base}/api/breed/{slug}/images/random`
- Returns `{ "message": "<image-url>", "status": "success" }`

---

## Deliverables

### New file: `appsettings.json`

Placed at project root. Project property: **Copy to Output Directory = Always**.

```json
{
  "DogApiBaseUrl": "http://localhost:8080/",
  "DogImageApiBaseUrl": "https://dog.ceo/"
}
```

### New file: `AppConfig.cs`

- Namespace: `Dog_Exploder`
- Static class, two `static string` properties: `DogApiBaseUrl`, `DogImageApiBaseUrl`
- Reads `appsettings.json` via `System.Text.Json` once on first access (lazy, thread-safe via `Lazy<T>`)
- Falls back to original hardcoded URLs if the file is missing

### Modified: `Services/DogApiService.cs`

Replace `BaseAddress = new Uri("https://dogapi.dog/")` → `new Uri(AppConfig.DogApiBaseUrl)`

### Modified: `Services/DogImageService.cs`

Replace hardcoded `"https://dog.ceo/api/breed/{slug}/images/random"` → `$"{AppConfig.DogImageApiBaseUrl}api/breed/{slug}/images/random"`
