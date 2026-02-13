# Fixed All Kavita API Endpoints

## Summary

Fixed all incorrect API endpoints to match the official Kavita OpenAPI specification.

## What Was Fixed

### 1. Series by Library - Changed from GET to POST
- **Before:** `GET /api/Series/series-by-library?libraryId={id}`
- **After:** `POST /api/Series/all` with request body

### 2. Recently Added - Changed from GET to POST  
- **Before:** `GET /api/Series/recently-added?pageNumber=0`
- **After:** `POST /api/Series/recently-added` with request body

### 3. On Deck - Changed from GET to POST
- **Before:** `GET /api/Series/on-deck?pageNumber=0`
- **After:** `POST /api/Series/on-deck` with request body

### 4. Search - Changed from GET to POST
- **Before:** `GET /api/Search/search?queryString={term}`
- **After:** `POST /api/Search/search` with request body

### 5. Series Metadata - Fixed path parameter
- **Before:** `GET /api/Series/metadata?seriesId={id}`
- **After:** `GET /api/Series/metadata/{id}`

### 6. Genres - Fixed endpoint path
- **Before:** `GET /api/Metadata/genres`
- **After:** `GET /api/Metadata/all-genres`

### 7. Tags - Fixed endpoint path
- **Before:** `GET /api/Metadata/tags`
- **After:** `GET /api/Metadata/all-tags`

## New Request DTOs

Added 4 new request models:
- `SeriesFilterDto` - For /api/Series/all
- `RecentlyAddedFilterDto` - For /api/Series/recently-added
- `OnDeckFilterDto` - For /api/Series/on-deck
- `SearchDto` - For /api/Search/search

## Methods Updated

### Test Methods
- TestGetSeriesForLibrary()
- TestGetRecentlyAdded()
- TestGetOnDeck()
- TestSearchSeries()
- TestGetSeriesMetadata()
- TestGetGenres()
- TestGetTags()

### Production Methods
- GetSeriesForLibraryAsync()
- GetSeriesMetadataAsync()

## Build Status

```
? Build Successful
? 0 Errors
? 0 Warnings
```

## Files Modified

1. KavitaModels.cs - Added 4 request DTOs
2. KavitaApiService.cs - Fixed 9 methods

---

**All API calls now use correct endpoints per Kavita OpenAPI spec!**
