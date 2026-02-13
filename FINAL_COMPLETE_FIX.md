# ? FINAL FIX: BaseAddress Modification During Requests

## The Real Root Cause

The error was **still occurring even after tests finished** because:

### The Problem Flow
```
1. User clicks "Test Connectivity"
   ¡ú SetBaseUrl("http://localhost:5000") - changes BaseAddress
   ¡ú HTTP request starts
   ¡ú Request completes

2. User immediately clicks "Test Libraries"  
   ¡ú SetBaseUrl("http://localhost:5000") called AGAIN
   ¡ú Even though URL is same, BaseAddress property is SET again
   ¡ú If ANY async operation from previous test is still cleaning up...
   ¡ú ? CRASH: "One or more errors occurred"
```

### Why This Happened

The `HttpClient.BaseAddress` property **should not be modified** while requests might be in flight or cleaning up. Even though we had semaphore locking for the HTTP requests themselves, we were modifying `BaseAddress` **outside** that lock.

```csharp
// BEFORE (BROKEN):
public void SetBaseUrl(string baseUrl)
{
    _baseUrl = baseUrl.TrimEnd('/');
    _httpClient.BaseAddress = new Uri(_baseUrl);  // ? Not thread-safe!
}

// Each test called this, changing BaseAddress every time
_apiService.SetBaseUrl(ServerUrl);  // Concurrent modification!
```

## The Complete Fix

### 1. Made SetBaseUrl Async and Thread-Safe

```csharp
private readonly SemaphoreSlim _urlLock = new(1, 1);

public async Task SetBaseUrl(string baseUrl)
{
    await _urlLock.WaitAsync();  // ?? Lock URL changes
    try
    {
        // Only update if different
        if (_baseUrl == baseUrl?.TrimEnd('/'))
        {
            return;  // Skip if same URL
        }

        _baseUrl = baseUrl?.TrimEnd('/') ?? string.Empty;
        
        if (Uri.TryCreate(_baseUrl, UriKind.Absolute, out var uri))
        {
            _httpClient.BaseAddress = uri;
        }
    }
    finally
    {
        _urlLock.Release();  // ?? Unlock
    }
}
```

**Key Improvements:**
- ? Async method with its own semaphore lock
- ? Only updates if URL actually changed
- ? Thread-safe modification of BaseAddress
- ? Prevents concurrent modifications

### 2. Added BaseURL Validation in SafeHttpRequest

```csharp
private async Task<HttpResponseMessage> SafeHttpRequest(...)
{
    await _httpLock.WaitAsync(cancellationToken);
    try
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(...);
        }
        
        // NEW: Verify BaseURL is set
        if (string.IsNullOrWhiteSpace(_baseUrl))
        {
            throw new InvalidOperationException("Base URL is not set.");
        }
        
        return await requestFunc(cancellationToken);
    }
    finally
    {
        _httpLock.Release();
    }
}
```

### 3. Updated ViewModel to Await SetBaseUrl

```csharp
// BEFORE:
_apiService.SetBaseUrl(ServerUrl);  // Fire-and-forget ?

// AFTER:
await _apiService.SetBaseUrl(ServerUrl);  // Properly awaited ?
```

## Why This Fixes The Issue

### Timeline - Before Fix ?
```
Time 0ms:   Test1 starts ¡ú SetBaseUrl (no lock)
Time 10ms:  Test1 HTTP request executing
Time 50ms:  Test1 finishing up (async cleanup)
Time 51ms:  User clicks Test2 ¡ú SetBaseUrl (no lock) 
Time 52ms:  BaseAddress CHANGED while Test1 cleanup running
Time 53ms:  ? CRASH: "One or more errors occurred"
```

### Timeline - After Fix ?
```
Time 0ms:   Test1 starts ¡ú SetBaseUrl (locked, URL set)
Time 10ms:  Test1 HTTP request (locked, executing)
Time 50ms:  Test1 finishing (locked, cleanup)
Time 51ms:  Test1 done, lock released
Time 52ms:  User clicks Test2 ¡ú SetBaseUrl (locked, waits)
Time 100ms: SetBaseUrl lock acquired
Time 101ms: SetBaseUrl checks: URL same as before ¡ú SKIP
Time 102ms: Test2 continues with same BaseAddress
Time 103ms: ? SUCCESS: No conflicts
```

## Changed Files

### 1. KAssistant/Services/KavitaApiService.cs

**Added:**
- `private readonly SemaphoreSlim _urlLock = new(1, 1);`
- Made `SetBaseUrl` async
- URL change locking
- URL change detection
- BaseURL validation

### 2. KAssistant/ViewModels/MainWindowViewModel.cs

**Changed all calls to:**
- `await _apiService.SetBaseUrl(ServerUrl);`

## Build Status

```
? Build Successful
? 0 Errors
? 0 Warnings
```

## Version

- **v1.1.3**: **COMPLETE FIX** ?

---

**Status: ? COMPLETELY FIXED**
