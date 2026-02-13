# OpenAPI Quick Start Guide

## 5-Minute Getting Started

### Step 1: Installation (Already Done)
The necessary NuGet packages are already installed:
- Microsoft.OpenApi (v1.6.14)
- Microsoft.OpenApi.Readers (v1.6.14)

### Step 2: Basic Usage

```csharp
using KAssistant.Services;

// Create service
using var service = new KavitaOpenApiService();

// Set server URL
await service.SetBaseUrl("http://localhost:5000");

// Login
var loginResult = await service.TestLogin("username", "password");

if (loginResult.Success)
{
    Console.WriteLine("? Logged in successfully!");
    
    // Get libraries
    var libraries = await service.GetLibrariesAsync();
    Console.WriteLine($"Found {libraries?.Count ?? 0} libraries");
}
```

### Step 3: Run All Tests

```csharp
using var service = new KavitaOpenApiService();
await service.SetBaseUrl("http://localhost:5000");

var results = await service.RunAllTests("username", "password");

foreach (var result in results)
{
    var icon = result.Success ? "?" : "?";
    Console.WriteLine($"{icon} {result.TestName}: {result.Message}");
}
```

## Common Use Cases

### 1. Check Server Connectivity

```csharp
var result = await service.TestConnectivity();
Console.WriteLine(result.Success ? "Server is online" : "Server is offline");
```

### 2. Get Server Information

```csharp
var result = await service.TestServerInfo();
if (result.Success)
{
    Console.WriteLine(result.Details);
}
```

### 3. Browse Libraries

```csharp
await service.TestLogin("user", "pass");
var result = await service.TestGetLibraries();
Console.WriteLine(result.Details);
```

### 4. Get Series Metadata

```csharp
await service.TestLogin("user", "pass");
var metadata = await service.GetSeriesMetadataAsync(seriesId: 1);

if (metadata != null)
{
    Console.WriteLine($"Summary: {metadata.Summary}");
    Console.WriteLine($"Genres: {string.Join(", ", metadata.Genres)}");
}
```

### 5. Get Statistics

```csharp
await service.TestLogin("user", "pass");

// Server stats
var serverStats = await service.TestGetServerStats();
Console.WriteLine($"Server: {serverStats.Details}");

// User stats
var userStats = await service.TestGetUserStats();
Console.WriteLine($"User: {userStats.Details}");
```

## Advanced Usage

### Using Direct OpenAPI Service

```csharp
using var api = new OpenApiKavitaService();
await api.SetBaseUrl("http://localhost:5000");

// Login
var user = await api.LoginAsync("username", "password");

// Call any endpoint by operation ID
var libraries = await api.CallApiAsync<List<Library>>("Library_GetLibraries");

// Call with parameters
var library = await api.CallApiAsync<Library>(
    "Library_GetLibrary",
    parameters: new Dictionary<string, object> { { "libraryId", 1 } }
);

// Call with body
var series = await api.CallApiAsync<PaginatedResult<Series>>(
    "Series_GetOnDeck",
    body: new OnDeckFilterDto { PageNumber = 0, PageSize = 20 }
);
```

## Next Steps

1. **Read the full documentation**: See `OPENAPI_README.md`
2. **Review examples**: Check `Examples/OpenApiExamples.cs`
3. **Compare implementations**: See `COMPARISON.md`
4. **Explore the API**: Review `Models/kavita_api.json`

That's it! You're ready to use the Kavita OpenAPI service. ??
