# ?? Metadata Modification API Endpoints

## Overview

Added 5 new API endpoints for modifying book/series metadata in Kavita libraries. These endpoints allow you to update metadata, lock covers, rate series, and delete series.

## New Metadata Endpoints

### 1. Get Series Metadata
```csharp
TestGetSeriesMetadata(int seriesId)
```

**Purpose**: Retrieve current metadata for a series  
**Endpoint**: `GET /api/Series/metadata?seriesId={id}`  
**Auth**: Required  

**Returns**:
- Summary/description
- Age rating
- Publication status
- Language
- Genres (up to 10 shown, indicates if more)
- Tags (up to 10 shown, indicates if more)

**Example Output**:
```
? Success
Message: Metadata retrieved for series 123
Details:
  Summary: A thrilling adventure series...
  Age Rating: Teen
  Publication Status: Ongoing
  Language: English
  
  Genres (5):
    - Action
    - Adventure
    - Fantasy
    - Shounen
    - Supernatural
  
  Tags (12):
    - Magic
    - Swords
    - Demons
    - Angels
    - Tournament Arc
    - Power Levels
    - Friendship
    - Betrayal
    - Epic Battles
    - World Building
    ... and 2 more
```

### 2. Update Series Metadata
```csharp
TestUpdateSeriesMetadata(int seriesId, SeriesMetadata metadata)
```

**Purpose**: Update metadata fields for a series  
**Endpoint**: `POST /api/Series/metadata?seriesId={id}`  
**Auth**: Required (Edit permissions)  

**Parameters**:
- `seriesId`: Series to update
- `metadata`: SeriesMetadata object with fields to update

**Updatable Fields**:
- `Summary` - Series description
- `AgeRating` - Content rating
- `PublicationStatus` - Ongoing, Completed, etc.
- `Language` - Language of content
- `Genres` - List of genre strings
- `Tags` - List of tag strings

**Example**:
```csharp
var metadata = new SeriesMetadata
{
    Summary = "Updated description of the series",
    AgeRating = "Mature",
    PublicationStatus = "Completed",
    Language = "English",
    Genres = new List<string> { "Action", "Drama", "Thriller" },
    Tags = new List<string> { "Dark", "Psychological", "Revenge" }
};

var result = await apiService.TestUpdateSeriesMetadata(123, metadata);
```

**Success Output**:
```
? Success
Message: Metadata updated successfully
Details:
  Metadata updated for series ID: 123
  
  Updated fields:
  - Summary: Updated description of the series...
  - Age Rating: Mature
  - Publication Status: Completed
  - Language: English
  - Genres: 3 genres
  - Tags: 3 tags
```

**Error Responses**:
- `401 Unauthorized`: Not logged in
- `403 Forbidden`: No edit permissions

### 3. Lock/Unlock Series Cover
```csharp
TestLockSeriesCover(int seriesId, bool locked)
```

**Purpose**: Lock or unlock a series cover image  
**Endpoint**: `POST /api/Series/update-cover-lock?seriesId={id}&locked={true/false}`  
**Auth**: Required  

**Behavior**:
- **Locked**: Cover won't be updated during library scans
- **Unlocked**: Cover can be auto-updated from metadata/files

**Use Cases**:
- Preserve custom cover images
- Prevent overwriting manually selected covers
- Lock covers for series with poor auto-detection

**Example - Lock Cover**:
```csharp
var result = await apiService.TestLockSeriesCover(123, true);
```

**Output**:
```
? Success
Message: Cover locked for series 123
Details: Cover image is now locked and won't be updated during scans
```

**Example - Unlock Cover**:
```csharp
var result = await apiService.TestLockSeriesCover(123, false);
```

**Output**:
```
? Success
Message: Cover unlocked for series 123
Details: Cover image is now unlocked and can be updated during scans
```

### 4. Update Series Rating
```csharp
TestUpdateSeriesRating(int seriesId, float rating)
```

**Purpose**: Set user rating for a series  
**Endpoint**: `POST /api/Series/update-rating?seriesId={id}&userRating={rating}`  
**Auth**: Required  

**Rating Scale**: 0.0 to 5.0 stars  
**Validation**: Rejects ratings outside range  

**Example**:
```csharp
var result = await apiService.TestUpdateSeriesRating(123, 4.5f);
```

**Output**:
```
? Success
Message: Rating updated to 4.5 stars
Details: Series 123 rated 4.5/5.0
```

**Invalid Rating**:
```csharp
var result = await apiService.TestUpdateSeriesRating(123, 6.0f);
```

**Output**:
```
? Failed
Message: Invalid rating
Details: Rating must be between 0 and 5
```

### 5. Delete Series
```csharp
TestDeleteSeries(int seriesId)
```

**Purpose**: Remove series from Kavita database  
**Endpoint**: `DELETE /api/Series/{id}`  
**Auth**: Required (Admin)  

**?? WARNING**: 
- Removes series from Kavita's database
- Files on disk are **NOT** deleted
- Series can be re-added by scanning the library
- This is for database cleanup, not file deletion

**Use Cases**:
- Remove incorrectly parsed series
- Clean up duplicate entries
- Remove series from specific library

**Example**:
```csharp
var result = await apiService.TestDeleteSeries(123);
```

**Output**:
```
? Success
Message: Series 123 deleted successfully
Details: ?? WARNING: This operation removes the series from Kavita's database. 
         Files on disk are not deleted.
```

**Forbidden**:
```
? Failed
Message: Forbidden - Admin access required
Details: Only administrators can delete series.
```

## Complete Workflow Examples

### Workflow 1: Update Book Metadata
```
1. Get Libraries ¡ú Find library ID
2. Get Series for Library ¡ú Find series ID
3. Get Series Metadata ¡ú Review current metadata
4. Update Series Metadata ¡ú Make changes
5. Get Series Metadata ¡ú Verify updates
```

### Workflow 2: Rate and Lock Cover
```
1. Search Series ¡ú Find series
2. Get Series Details ¡ú Get series ID
3. Update Series Rating ¡ú Rate 4.5 stars
4. Lock Series Cover ¡ú Protect custom cover
5. Get Series Details ¡ú Confirm changes
```

### Workflow 3: Clean Up Duplicates
```
1. Get Series for Library ¡ú Find duplicate
2. Get Series Details ¡ú Verify it's a duplicate
3. Delete Series ¡ú Remove from database
4. Scan Library ¡ú Re-scan if needed
```

## Metadata Object Structure

```csharp
public class SeriesMetadata
{
    public string Summary { get; set; } = string.Empty;
    public List<string> Genres { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public string AgeRating { get; set; } = string.Empty;
    public string PublicationStatus { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
}
```

### Common Values

**Age Ratings**:
- `Everyone`
- `Teen`
- `Mature`
- `Adults Only`
- `Unknown`

**Publication Status**:
- `Ongoing`
- `Completed`
- `Cancelled`
- `Hiatus`
- `Unknown`

**Languages**:
- `English`
- `Japanese`
- `Korean`
- `Chinese`
- `French`
- `German`
- etc.

## Permission Requirements

| Endpoint | User | Admin | Notes |
|----------|------|-------|-------|
| Get Metadata | ? | ? | Anyone authenticated |
| Update Metadata | ?* | ? | *Requires edit permissions |
| Lock Cover | ? | ? | Anyone authenticated |
| Update Rating | ? | ? | Personal rating |
| Delete Series | ? | ? | Admin only |

## Error Handling

All endpoints handle:
- **Authentication**: Returns clear "Not authenticated" message
- **Authorization**: Specific messages for 401/403 errors
- **Validation**: Input validation before API call
- **Network Errors**: Full exception details
- **Timeouts**: 30-second timeout with message

## Thread Safety

All metadata endpoints use `SafeHttpRequest`:
- Semaphore lock prevents concurrent requests
- Proper disposal handling
- Exception-safe operations

## Best Practices

### 1. Always Get Before Update
```csharp
// Get current metadata
var currentMeta = await api.TestGetSeriesMetadata(seriesId);

// Modify as needed
var updatedMeta = new SeriesMetadata
{
    Summary = "New description",
    Genres = currentMeta.Genres, // Keep existing genres
    Tags = currentMeta.Tags,      // Keep existing tags
    AgeRating = "Mature",         // Update this field
    // ...
};

// Update
await api.TestUpdateSeriesMetadata(seriesId, updatedMeta);
```

### 2. Lock Covers After Manual Changes
```csharp
// After uploading custom cover via web UI
await api.TestLockSeriesCover(seriesId, true);
```

### 3. Verify Before Delete
```csharp
// Get details first
var details = await api.TestGetSeriesById(seriesId);

// Confirm it's the right series
if (details.Success && /* user confirms */)
{
    await api.TestDeleteSeries(seriesId);
}
```

### 4. Batch Operations
```csharp
// Rate multiple series
var seriesIds = new[] { 1, 2, 3, 4, 5 };
foreach (var id in seriesIds)
{
    await api.TestUpdateSeriesRating(id, 5.0f);
    await Task.Delay(100); // Small delay between requests
}
```

## API Endpoint Summary

### Original Endpoints (v1.2.0): 19 total
1. Connectivity Test
2. Server Info
3. Login
4. Get Libraries
5. Scan Library
6. Get Library Details
7. Get Series for Library
8. Get Series Details
9. Recently Added
10. On Deck
11. Search Series
12. Get Users
13. Get Collections
14. Get Reading Lists
15. Get Genres
16. Get Tags
17. Server Stats
18. User Stats
19. Notifications

### New Metadata Endpoints (v1.3.0): +5
20. **Get Series Metadata**
21. **Update Series Metadata**
22. **Lock/Unlock Cover**
23. **Update Rating**
24. **Delete Series**

**Total: 24 API Endpoints**

## Example Integration

```csharp
// Complete metadata update workflow
public async Task UpdateBookMetadata(int seriesId)
{
    var api = new KavitaApiService();
    await api.SetBaseUrl("http://localhost:5000");
    await api.TestLogin("username", "password");
    
    // Get current metadata
    var getMeta = await api.TestGetSeriesMetadata(seriesId);
    if (!getMeta.Success)
    {
        Console.WriteLine($"Failed to get metadata: {getMeta.Message}");
        return;
    }
    
    // Update metadata
    var newMeta = new SeriesMetadata
    {
        Summary = "An epic fantasy adventure...",
        AgeRating = "Teen",
        PublicationStatus = "Ongoing",
        Language = "English",
        Genres = new List<string> { "Fantasy", "Adventure", "Magic" },
        Tags = new List<string> { "Dragons", "Wizards", "Prophecy" }
    };
    
    var updateResult = await api.TestUpdateSeriesMetadata(seriesId, newMeta);
    if (updateResult.Success)
    {
        Console.WriteLine("Metadata updated successfully!");
        
        // Lock the cover to preserve it
        await api.TestLockSeriesCover(seriesId, true);
        
        // Rate it
        await api.TestUpdateSeriesRating(seriesId, 4.5f);
    }
    else
    {
        Console.WriteLine($"Update failed: {updateResult.Message}");
    }
}
```

## Testing Recommendations

### Test Cases

1. **Read Operations**
   - Get metadata for existing series
   - Get metadata for non-existent series
   - Get metadata without authentication

2. **Update Operations**
   - Update single field
   - Update multiple fields
   - Update with empty values
   - Update without permissions

3. **Cover Lock**
   - Lock then unlock
   - Lock multiple series
   - Lock non-existent series

4. **Rating**
   - Valid ratings (0-5)
   - Invalid ratings (<0, >5)
   - Decimal ratings (4.5, 3.7)

5. **Delete**
   - Delete as admin
   - Delete as regular user (should fail)
   - Delete non-existent series

## Version History

- **v1.0.x**: Basic endpoints
- **v1.1.x**: 11 core endpoints  
- **v1.2.0**: +9 endpoints (19 total)
- **v1.3.0**: +5 metadata endpoints (24 total) ?

## Summary

The metadata modification endpoints provide:
- ? Full metadata read/write capability
- ? Cover image lock management
- ? User rating system
- ? Series deletion (admin only)
- ? Comprehensive error handling
- ? Permission-aware operations
- ? Thread-safe execution

**You can now fully manage book/series metadata programmatically!** ???
