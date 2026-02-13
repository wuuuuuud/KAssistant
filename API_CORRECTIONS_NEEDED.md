# Kavita API Endpoint Corrections

Based on the official Kavita OpenAPI spec, here are the corrections needed:

## Current Issues

### 1. Library Endpoint - ? CORRECT
```
Current: GET /api/Library
Status: CORRECT
```

### 2. Series by Library - ? WRONG
```
Current: GET /api/Series/series-by-library?libraryId={id}&pageNumber={page}&pageSize={size}
Correct: POST /api/Series/all
Body: {
  "libraryIds": [1],
  "pageNumber": 0,
  "pageSize": 100
}
```

### 3. Recently Added - ? WRONG
```
Current: GET /api/Series/recently-added?pageNumber=0&pageSize=10
Correct: POST /api/Series/recently-added
Body: {
  "pageNumber": 0,
  "pageSize": 10
}
```

### 4. On Deck - ? WRONG
```
Current: GET /api/Series/on-deck?pageNumber=0&pageSize=10
Correct: POST /api/Series/on-deck
Body: {
  "pageNumber": 0,
  "pageSize": 10,
  "libraryId": 0
}
```

### 5. Search - ? WRONG
```
Current: GET /api/Search/search?queryString={term}
Correct: POST /api/Search/search
Body: {
  "queryString": "search term"
}
```

### 6. Series Metadata - ? WRONG
```
Current: GET /api/Series/metadata?seriesId={id}
Correct: GET /api/Series/metadata/{id}
```

### 7. Update Metadata - ? WRONG
```
Current: POST /api/Series/metadata?seriesId={id}
Correct: POST /api/Series/metadata
Body: SeriesMetadata object with seriesId
```

### 8. Users - ? WRONG (requires pagination)
```
Current: GET /api/Users
Correct: GET /api/User
```

### 9. Server Info - ? CORRECT
```
Current: GET /api/Server/server-info
Status: CORRECT
```

### 10. Genres/Tags - ? WRONG
```
Current: GET /api/Metadata/genres
Correct: GET /api/Metadata/all-genres
Current: GET /api/Metadata/tags
Correct: GET /api/Metadata/all-tags
```

## Summary of Required Changes

1. Change Series endpoints from GET to POST with request bodies
2. Fix metadata endpoint to use path parameter
3. Fix User endpoint path
4. Fix Genres/Tags endpoint paths
5. Add proper request DTOs for POST endpoints
