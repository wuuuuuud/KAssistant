# COMPLETE TROUBLESHOOTING GUIDE

Since you're still experiencing issues, let's systematically identify and fix the problem.

## Step 1: Identify the Exact Problem

### What to Check:

1. **Does the app start?**
   - ? YES ¡ú Go to Step 2
   - ? NO ¡ú Check build errors, missing dependencies

2. **Can you login?**
   - ? YES ¡ú Go to Step 3
   - ? NO ¡ú Check server URL, credentials, network

3. **Does Series Browser open?**
   - ? YES ¡ú Go to Step 4
   - ? NO ¡ú Check authentication, button binding

4. **Do series load?**
   - ? YES ¡ú Problem solved!
   - ? NO ¡ú Continue to Step 5

5. **What happens when you try to load series?**
   - Nothing (blank screen)
   - Error message
   - Crash
   - Spinning/loading forever

## Step 2: Run Diagnostics

I've created a diagnostic tool. Add this to your MainWindowViewModel:

```csharp
[RelayCommand]
private async Task RunDiagnostics()
{
    try
    {
        StatusMessage = "Running diagnostics...";
        await KAssistant.Diagnostics.KavitaDiagnostics.RunDiagnostics(
            ServerUrl,
            Username,
            Password
        );
        StatusMessage = "Diagnostics complete - check Output window (Ctrl+Alt+O)";
    }
    catch (Exception ex)
    {
        StatusMessage = $"Diagnostics error: {ex.Message}";
    }
}
```

And add this button to MainWindow.axaml:

```xml
<Button Content="?? Run Diagnostics" 
        Command="{Binding RunDiagnosticsCommand}"
        Margin="5"/>
```

Then:
1. Run the app
2. Click "Run Diagnostics"
3. Open Output window (View ¡ú Output or Ctrl+Alt+O)
4. Copy ALL the output
5. Read the results below

## Step 3: Interpret Diagnostic Results

### Result A: All Tests Pass (?????)
```
? Connected
? Login successful
? Libraries endpoint works
? Recently-added endpoint works
? All-v2 endpoint works
```

**This means**: API works, problem is in the app logic

**Solutions**:
1. Check if `_apiService.IsAuthenticated` is true before loading
2. Verify `GetAllSeriesAsync(0, 1000)` is being called
3. Check if results are being added to the UI collection
4. Look for exceptions in Output window

### Result B: Connection Failed (?)
```
? Connection failed: NotFound
```

**This means**: Can't reach the server

**Solutions**:
1. Check server URL format (should be `http://localhost:5000` not `http://localhost:5000/`)
2. Ensure Kavita is running
3. Try accessing `http://localhost:5000` in your browser
4. Check firewall/antivirus

### Result C: Login Failed (?)
```
? Connected
? Login failed: 401 Unauthorized
```

**This means**: Wrong credentials or auth issue

**Solutions**:
1. Verify username/password in Kavita web UI
2. Check if account is locked
3. Try creating a new API key in Kavita
4. Check Kavita authentication settings

### Result D: Recently-Added Failed (?)
```
? Connected
? Login successful
? Libraries endpoint works
? Recently-added failed: 404
```

**This means**: Endpoint doesn't exist (wrong Kavita version)

**Solutions**:
1. Check your Kavita version
2. Try `/api/Series/recently-added` (without -v2)
3. Update Kavita to latest version
4. Use different endpoint

### Result E: JSON Parse Error (?)
```
? Recently-added endpoint works
? Could not parse response: JsonException
```

**This means**: Response format mismatch

**Solutions**:
1. Look at the "First series object structure" output
2. Compare with `Series` class in `KavitaModels.cs`
3. Add missing fields
4. Fix property name mismatches

## Step 4: Common Fixes

### Fix 1: Server URL Format

**Problem**: URL has trailing slash or wrong format

**Wrong**:
```csharp
ServerUrl = "http://localhost:5000/";  // ? Trailing slash
ServerUrl = "localhost:5000";          // ? Missing protocol
ServerUrl = "http://localhost:5000/api"; // ? Has /api
```

**Correct**:
```csharp
ServerUrl = "http://localhost:5000";   // ? Correct
```

### Fix 2: Authentication Not Set

**Problem**: Token not being passed to API calls

**Check** in `KavitaApiService.cs`:
```csharp
public async Task<UserDto?> LoginAsync(string username, string password)
{
    var result = await _apiService.LoginAsync(username, password);
    if (result?.Token != null)
    {
        _isAuthenticated = true; // ¡û Make sure this is set
    }
    return result;
}
```

### Fix 3: Series Browser Not Getting Auth

**Problem**: Series Browser opens before login complete

**Solution**: Ensure login happens first:

```csharp
[RelayCommand(CanExecute = nameof(CanExecuteTests))]
private async Task OpenSeriesBrowser()
{
    try
    {
        // ADD THIS: Ensure we're logged in
        if (!_apiService.IsAuthenticated)
        {
            var loginResult = await _apiService.TestLogin(Username, Password);
            if (!loginResult.Success)
            {
                StatusMessage = "Please login first";
                return;
            }
        }
        
        var viewModel = new SeriesBrowserViewModel(_apiService);
        // ... rest of code
    }
    catch (Exception ex)
    {
        StatusMessage = $"Error: {ex.Message}";
    }
}
```

### Fix 4: Wrong Endpoint Being Used

**Problem**: Using `/api/Series/all-v2` which doesn't work

**Solution**: Already fixed - using `recently-added-v2`

Check `OpenApiKavitaService.cs` line ~250:
```csharp
// Should be using GetRecentlyAddedSeriesAsync()
var libraryResult = await GetRecentlyAddedSeriesAsync(
    currentPage, 
    100, 
    library.Id
);
```

### Fix 5: Missing Fields in Series Class

**Problem**: JSON has fields our class doesn't

**Solution**: Already added in `KavitaModels.cs`

Verify `Series` class has these fields:
- `pages`
- `wordCount`
- `format`
- `minHoursToRead`, `maxHoursToRead`, `avgHoursToRead`
- `folderPath`
- `lastChapterAdded`
- `latestReadDate`
- `userRating`, `userReview`
- `coverImage`
- `lowestFolderPath`

### Fix 6: UI Not Updating

**Problem**: Series load but UI doesn't show them

**Solution**: Check `SeriesBrowserViewModel.cs`:

```csharp
private async Task LoadDataAsync()
{
    // ...
    var allSeriesResult = await _apiService.GetAllSeriesAsync(0, 1000);
    
    if (allSeriesResult?.Result != null)
    {
        _allSeries = allSeriesResult.Result;  // ¡û Check this line
        ApplyFilter();  // ¡û Check this is called
    }
}

private void ApplyFilter()
{
    FilteredSeries.Clear();  // ¡û UI-bound collection
    
    var filtered = _allSeries.AsEnumerable();
    // ... filtering logic ...
    
    foreach (var series in filtered.OrderBy(s => s.Name))
    {
        FilteredSeries.Add(series);  // ¡û Check this adds to UI
    }
}
```

## Step 5: Enable Detailed Logging

Add this to the top of `OpenApiKavitaService.cs`:

```csharp
private bool _enableDebugLogging = true;  // Set to true for debugging

private void Log(string message)
{
    if (_enableDebugLogging)
    {
        Console.WriteLine($"[OpenApiKavitaService] {message}");
    }
}
```

Then add `Log()` calls throughout:

```csharp
public async Task<T?> PostAsync<T>(string path, object? body = null)
{
    Log($"POST {path}");
    // ... existing code ...
    Log($"Response: {response.StatusCode}");
    return result;
}
```

## Step 6: Check Output Window

1. Run app in Debug mode (F5)
2. Open Output window (View ¡ú Output or Ctrl+Alt+O)
3. Select "Debug" from dropdown
4. Try to load series
5. Look for:
   - Console.WriteLine messages
   - Exception messages
   - HTTP status codes
   - JSON errors

## Step 7: Try Alternative Approach

If nothing works, try loading series differently:

### Option A: Use OnDeck Instead
```csharp
// In GetAllSeriesAsync()
var result = await GetOnDeckSeriesAsync(currentPage, 100, library.Id);
```

### Option B: Load Per Library Only
```csharp
// Don't try to load all libraries at once
// Just load from selected library
public async Task LoadSeriesFromLibrary(int libraryId)
{
    var result = await _apiService.GetRecentlyAddedSeriesAsync(0, 1000, libraryId);
    // Process result
}
```

### Option C: Use HTTP Directly
```csharp
// Bypass our service and use HttpClient directly
using var client = new HttpClient();
client.BaseAddress = new Uri(ServerUrl);
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

var response = await client.PostAsync("/api/Series/recently-added-v2", content);
var json = await response.Content.ReadAsStringAsync();
Console.WriteLine(json);  // See actual response
```

## Step 8: Specific Error Messages

### "No series found in any library"

**Cause**: API returns empty result

**Check**:
1. Do series actually exist in Kavita?
2. Is library scanned?
3. Do you have permission to view them?

### "JsonException"

**Cause**: Response format mismatch

**Fix**: Add missing fields to Series class (already done)

### "HttpRequestException"

**Cause**: Network/connection issue

**Fix**: Check server URL, firewall, Kavita running

### "InvalidOperationException: Base URL not set"

**Cause**: `SetBaseUrl()` not called

**Fix**: Call `await _apiService.SetBaseUrl(ServerUrl)` before any API calls

### "401 Unauthorized"

**Cause**: Not logged in or token expired

**Fix**: Login again before opening Series Browser

## Step 9: Fresh Start

If all else fails:

1. **Close Visual Studio**
2. **Delete** `bin` and `obj` folders
3. **Rebuild** solution (Ctrl+Shift+B)
4. **Clean** solution first (Build ¡ú Clean Solution)
5. **Rebuild** again
6. **Run** in Debug mode

## Step 10: Report Issue

If still not working, please provide:

1. **Diagnostic output** (from Run Diagnostics button)
2. **Console output** (from Output window)
3. **Error message** (exact text)
4. **When it fails** (startup, login, loading series, etc.)
5. **Kavita version**
6. **What you see** (blank screen, error dialog, crash, etc.)

Then I can provide a targeted fix!

## Quick Checklist

- [ ] Built successfully
- [ ] Can start app
- [ ] Can enter server URL and credentials
- [ ] Can click Login
- [ ] Login succeeds (check status message)
- [ ] Can click "Open Series Browser"
- [ ] Series Browser window opens
- [ ] See "Loading libraries..." message
- [ ] See "Found X libraries" message
- [ ] See "Loading from library: ..." messages
- [ ] See "? Loaded X series" messages
- [ ] See series in the list
- [ ] Can search/filter series

**Which step fails?** That's where the problem is!
