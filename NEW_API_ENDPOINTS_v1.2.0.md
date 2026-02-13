# ?? New API Endpoints Added - v1.2.0

## Overview

Added 9 new Kavita API endpoint methods based on the official Kavita OpenAPI specification, expanding testing capabilities significantly.

## New Endpoints

### 1. Library Management

#### Scan Library
```csharp
TestScanLibrary(int libraryId)
```
- **Endpoint**: `POST /api/Library/scan?libraryId={id}`
- **Purpose**: Trigger a library scan
- **Use Case**: Testing library scanning functionality
- **Response**: Confirms scan has been queued
- **Auth**: Required

#### Get Library Details
```csharp
TestGetLibraryById(int libraryId)
```
- **Endpoint**: `GET /api/Library/{id}`
- **Purpose**: Get detailed information about a specific library
- **Returns**: Library name, ID, type, folder watching status, last scan date
- **Auth**: Required

### 2. Series Operations

#### Get Series for Library
```csharp
TestGetSeriesForLibrary(int libraryId)
```
- **Endpoint**: `GET /api/Series/series-by-library?libraryId={id}&pageNumber=0&pageSize=20`
- **Purpose**: List all series in a specific library
- **Returns**: Paginated list of series with names and IDs
- **Auth**: Required

#### Get Series Details
```csharp
TestGetSeriesById(int seriesId)
```
- **Endpoint**: `GET /api/Series/{id}`
- **Purpose**: Get detailed information about a specific series
- **Returns**: Series name, original name, library ID, created date, pages read, etc.
- **Auth**: Required

### 3. Search & Discovery

#### Search Series
```csharp
TestSearchSeries(string searchTerm)
```
- **Endpoint**: `GET /api/Search/search?queryString={term}`
- **Purpose**: Search for series by name
- **Returns**: Search results matching the query
- **Auth**: Required
- **Example**: Search for "naruto", "one piece", etc.

### 4. Metadata

#### Get All Genres
```csharp
TestGetGenres()
```
- **Endpoint**: `GET /api/Metadata/genres`
- **Purpose**: Retrieve all genres in the Kavita library
- **Returns**: List of genre names
- **Auth**: Required
- **Display**: Shows first 50 genres, indicates if more exist

#### Get All Tags
```csharp
TestGetTags()
```
- **Endpoint**: `GET /api/Metadata/tags`
- **Purpose**: Retrieve all tags in the Kavita library
- **Returns**: List of tag names
- **Auth**: Required
- **Display**: Shows first 50 tags, indicates if more exist

### 5. Server Monitoring

#### Get Notification Progress
```csharp
TestGetNotificationProgress()
```
- **Endpoint**: `GET /api/Server/notifications`
- **Purpose**: Check for active background tasks/notifications
- **Returns**: List of active notifications or "No active notifications"
- **Auth**: Required
- **Use Case**: Monitor scan progress, library operations, etc.

## Usage Examples

### Example 1: Test Library Workflow
```
1. Get Libraries ¡ú Get library ID
2. Get Library Details (ID from step 1) ¡ú Verify library config
3. Get Series for Library (ID from step 1) ¡ú See all series
4. Scan Library (ID from step 1) ¡ú Trigger scan
5. Get Notification Progress ¡ú Monitor scan status
```

### Example 2: Search and Explore
```
1. Search Series ("batman") ¡ú Find series
2. Get Series Details (ID from result) ¡ú Get full info
3. Get Genres ¡ú Browse by genre
4. Get Tags ¡ú Browse by tags
```

### Example 3: Library Management
```
1. Get Libraries ¡ú List all libraries
2. Get Library Details ¡ú Check each library
3. Get Series for Library ¡ú Verify content
4. Scan Library ¡ú Update library
5. Get Notification Progress ¡ú Confirm scan started
```

## Endpoint Categories

### Original Endpoints (v1.1.x)
1. Connectivity Test
2. Server Info
3. Login
4. Get Libraries
5. Recently Added Series
6. On Deck Series
7. Get Users
8. Get Collections
9. Get Reading Lists
10. Server Stats
11. User Stats

### New Endpoints (v1.2.0)
12. **Scan Library** - Library management
13. **Get Library Details** - Library management
14. **Get Series for Library** - Series operations
15. **Get Series Details** - Series operations
16. **Search Series** - Search & discovery
17. **Get Genres** - Metadata
18. **Get Tags** - Metadata
19. **Get Notification Progress** - Server monitoring

**Total:** 19 API endpoints

## Technical Details

### Authentication
All new endpoints require authentication. They check `IsAuthenticated` and return a clear error if not logged in.

### Error Handling
- **401 Unauthorized**: Clear message to login again
- **Network Errors**: Full exception details
- **Timeouts**: 30-second timeout with helpful message
- **Empty Results**: Friendly messages explaining empty states

### Response Formatting
- **Lists**: Shows count and items
- **Long Lists**: Truncates to first 50 items with "and X more" message
- **Empty Lists**: Clear explanation (e.g., "No genres found")
- **Details**: Formatted multi-line output

### Thread Safety
All new endpoints use the existing `SafeHttpRequest` wrapper:
- Semaphore lock prevents concurrent requests
- Proper disposal handling
- Exception-safe with finally blocks

## API Compatibility

Based on Kavita OpenAPI specification:
- https://raw.githubusercontent.com/Kareadita/Kavita/develop/openapi.json
- Compatible with Kavita API v1.x
- Endpoints tested against official API

## Usage in Application

These endpoints can be called:
- **Programmatically**: From ViewModels or services
- **For Testing**: To verify Kavita server functionality
- **For Debugging**: To explore server state
- **For Monitoring**: To track library operations

## Future Enhancements

Potential additional endpoints:
- Volume and Chapter operations
- Reading progress tracking
- Bookmark management
- User preferences
- Download operations
- Image/cover management
- Metadata editing

## Examples with Real Data

### Example: Get Library Details
```
Input: libraryId = 1

Output:
? Success
Message: Retrieved library 'Manga'
Details:
  Name: Manga
  ID: 1
  Type: 1
  Folder Watching: true
  Last Scanned: 2024-01-15 14:30:00
```

### Example: Search Series
```
Input: searchTerm = "one piece"

Output:
? Success
Message: Search completed for 'one piece'
Details:
  Search results:
  [
    {
      "id": 123,
      "name": "One Piece",
      "libraryId": 1,
      ...
    },
    {
      "id": 456,
      "name": "One Piece: Strong World",
      ...
    }
  ]
```

### Example: Get Genres
```
Output:
? Success
Message: Found 45 genres
Details:
  Found 45 genres:
  - Action
  - Adventure
  - Comedy
  - Drama
  - Fantasy
  - Horror
  - Mystery
  - Romance
  - Sci-Fi
  - Slice of Life
  ... (and 35 more)
```

## Integration Points

These endpoints integrate with:
- **ViewModels**: For UI interactions
- **Services**: For business logic
- **Testing Suite**: For automated testing
- **Monitoring**: For server health checks

## Error Scenarios

### Scenario: Search with No Results
```
? Success
Message: Search completed for 'nonexistent'
Details:
  Search results:
  []
```

### Scenario: Scan Already Running
```
? Failed
Message: Failed: BadRequest
Details:
  A scan is already in progress for this library
```

### Scenario: Library Not Found
```
? Failed
Message: Failed: NotFound
Details:
  Library with ID 999 does not exist
```

## Benefits

1. **More Comprehensive Testing**: Test library management, search, and metadata
2. **Better Debugging**: Inspect individual series and libraries
3. **Progress Monitoring**: Track background operations
4. **Content Discovery**: Search and browse by genres/tags
5. **Library Management**: Trigger scans, check status
6. **Metadata Exploration**: See all available genres and tags

## Version History

- **v1.0.x**: Basic connectivity and login
- **v1.1.x**: 11 core endpoints
- **v1.2.0**: +9 new endpoints (19 total) ?

## Summary

This update significantly expands the API testing capabilities, adding:
- ? Library management (scan, details)
- ? Series operations (list, details)
- ? Search functionality
- ? Metadata access (genres, tags)
- ? Server monitoring (notifications)

All endpoints follow the same patterns:
- Authentication checks
- Proper error handling
- Thread-safe execution
- Consistent formatting
- Clear error messages

**The application now supports 19 Kavita API endpoints for comprehensive testing and monitoring!** ??
