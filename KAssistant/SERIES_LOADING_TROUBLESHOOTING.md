# Series Loading Troubleshooting Guide

## Debug Logging Enabled

I've added comprehensive debug logging to help identify the exact issue. When you run the application and try to load series, you'll now see detailed console output.

## How to View Debug Output

### Option 1: Visual Studio
1. Run the application in Debug mode (F5)
2. Open the **Output** window (View ¡ú Output or Ctrl+Alt+O)
3. Select "Debug" from the "Show output from:" dropdown
4. Look for console output messages

### Option 2: Console Application
If running from command line:
```bash
dotnet run --project KAssistant/KAssistant.csproj
```

## What to Look For

### 1. Library Loading
```
GetAllSeriesAsync called with libraryId=0, pageNumber=0, pageSize=1000
Loading series from all libraries...
Found 3 libraries to query
```

**If you see "No libraries found!":**
- Problem: No libraries returned from API
- Solution: Check authentication, verify libraries exist in Kavita

### 2. Per-Library Loading
```
Loading series from library: Manga (ID: 1)
  Fetching page 0 for library 1
POST /api/Series/all-v2
Body: {"LibraryIds":[1],"PageNumber":0,"PageSize":100}
Response Status: OK
Response Body: {"currentPage":0,"pageSize":100,"totalCount":45,"totalPages":1,"result":[...]}
  Got 45 series (Total: 45, Page 1/1)
Loaded 45 series from library Manga
```

**Look for:**
- **Response Status**: Should be "OK" (200)
- **totalCount**: Should show number of series
- **result**: Should contain series data

### 3. Common Error Patterns

#### Pattern A: 404 Not Found
```
POST /api/Series/all-v2
Response Status: NotFound
ERROR loading series from library 1 (Manga): Response status code does not indicate success: 404 (Not Found)
```

**This means:** The endpoint `/api/Series/all-v2` doesn't exist!

**Solution:** The API endpoint might be different. Common alternatives:
- `/api/Series/all` (without -v2)
- `/api/series/all` (lowercase)
- Different endpoint entirely

#### Pattern B: 401 Unauthorized
```
Response Status: Unauthorized
ERROR loading series from library 1 (Manga): Response status code does not indicate success: 401 (Unauthorized)
```

**This means:** Not authenticated or token expired

**Solution:**
- Ensure you're logged in before opening Series Browser
- Check that token is being passed correctly
- Try logging in again

#### Pattern C: 400 Bad Request
```
Response Status: BadRequest
Response Body: {"errors":{"LibraryIds":["The LibraryIds field is required"]}}
```

**This means:** Request format is wrong

**Solution:** The DTO structure might not match what the API expects

#### Pattern D: Empty Result
```
Response Status: OK
Response Body: {"currentPage":0,"pageSize":100,"totalCount":0,"totalPages":0,"result":[]}
  No more series on page 0
Loaded 0 series from library Manga
```

**This means:** API returned successfully but no series found

**Possible causes:**
- Library is actually empty
- Filter is too restrictive
- Series exist but API isn't returning them

## Diagnostic Steps

### Step 1: Verify Authentication
Check console for:
```
Logged in as username
Token: eyJ... (truncated)
```

### Step 2: Verify Libraries Load
Check console for:
```
Found X libraries to query
Loading series from library: LibraryName (ID: N)
```

### Step 3: Check API Response
Look at the actual response body to see what the API is returning:
```
Response Body: {...}
```

### Step 4: Check for Exceptions
Look for:
```
ERROR loading series from library X
Stack trace: ...
```

## Quick Fixes

### Fix 1: Wrong Endpoint
If you see 404 errors, try changing the endpoint:

1. Open `KAssistant/Services/OpenApiKavitaService.cs`
2. Find line with `/api/Series/all-v2`
3. Try changing to:
   - `/api/Series/all`
   - `/api/series/all`
   - `/api/Series`

### Fix 2: Wrong HTTP Method
The endpoint might need GET instead of POST:

Change from:
```csharp
await PostAsync<PaginatedResult<Series>>("/api/Series/all-v2", filter);
```

To:
```csharp
await GetAsync<PaginatedResult<Series>>($"/api/Series/all?libraryId={library.Id}&pageNumber={currentPage}&pageSize=100");
```

### Fix 3: Missing Authentication
Ensure authentication happens before series loading:

1. Login first
2. Wait for success
3. Then open Series Browser

## Testing Individual Libraries

To test if the issue is with "all libraries" vs specific library:

1. In Series Browser, select a specific library from dropdown
2. Check if series load for that library
3. Compare console output

## API Endpoint Verification

To verify which endpoint to use:

1. Check Kavita documentation
2. Look in `kavita_api.json` for series endpoints
3. Try the Kavita web interface and check browser dev tools (F12) ¡ú Network tab
4. See what endpoint the web UI uses for series listing

## Common Solutions

### Solution 1: Update Endpoint
Based on actual Kavita API, the endpoint might be:
- `/api/Series/all-v2` (current attempt)
- `/api/Series/v2/all` (alternative format)
- `/api/Series` with query parameters

### Solution 2: Change HTTP Method
Some APIs require GET with query params instead of POST with body:

```csharp
// Instead of POST with filter object
GET /api/Series/all-v2?libraryId=1&pageNumber=0&pageSize=100
```

### Solution 3: Fix DTO Structure
The `SeriesFilterDto` might need different properties:

```csharp
// Current
public class SeriesFilterDto
{
    [JsonPropertyName("libraryIds")]
    public List<int> LibraryIds { get; set; }
    // ...
}

// Might need
public class SeriesFilterDto
{
    [JsonPropertyName("LibraryIds")] // Capital L
    public List<int> LibraryIds { get; set; }
    // ...
}
```

## Next Steps

1. **Run the application** with debug logging
2. **Try to load series** in Series Browser
3. **Copy ALL console output** from the Output window
4. **Share the output** so I can see exactly what's happening
5. **Check Kavita API version** - endpoint might be different in your version

## Getting More Help

When reporting the issue, please provide:

1. **Full console output** from attempting to load series
2. **Kavita version** you're connecting to
3. **Does the Kavita web UI** show series correctly?
4. **Can you see series** in Kavita's own interface?
5. **HTTP status code** from the API response
6. **Response body** from the API

## Alternative Approach

If the endpoint truly doesn't exist, we can use an alternative:

1. Use `/api/Series/recently-added-v2` to get series
2. Use `/api/Series/on-deck` to get series
3. Iterate through each library's detail page

Let me know what the console output shows and I can provide a targeted fix!
