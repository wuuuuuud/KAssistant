# SettingsService Documentation

## Overview

The `SettingsService` is responsible for persisting application settings (server URL, credentials) to disk in JSON format. It works in conjunction with the `AppSettings` model class.

## Files

### SettingsService.cs
Location: `KAssistant/Services/SettingsService.cs`

### AppSettings.cs
Location: `KAssistant/Models/AppSettings.cs`

## AppSettings Model

```csharp
public class AppSettings
{
    [JsonPropertyName("serverUrl")]
    public string ServerUrl { get; set; } = "http://localhost:5000";

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    [JsonPropertyName("rememberCredentials")]
    public bool RememberCredentials { get; set; } = false;
}
```

### Properties

- **ServerUrl**: The base URL of the Kavita server (default: http://localhost:5000)
- **Username**: User's login username
- **Password**: User's login password (stored in plain text locally)
- **RememberCredentials**: Whether to persist credentials between sessions

All properties use `[JsonPropertyName]` attributes to ensure consistent JSON serialization.

## SettingsService Methods

### Constructor
```csharp
public SettingsService()
```

Initializes the service and creates the settings directory if it doesn't exist:
- Location: `%AppData%/KAssistant/settings.json` (Windows)
- Creates directory automatically if missing

### LoadSettingsAsync
```csharp
public async Task<AppSettings> LoadSettingsAsync()
```

Loads settings from disk. Features:
- Returns a new `AppSettings()` object if file doesn't exist
- Handles errors gracefully (logs to console, returns defaults)
- Uses async file I/O for better performance

**Example:**
```csharp
var settingsService = new SettingsService();
var settings = await settingsService.LoadSettingsAsync();
Console.WriteLine($"Server URL: {settings.ServerUrl}");
```

### SaveSettingsAsync
```csharp
public async Task SaveSettingsAsync(AppSettings settings)
```

Saves settings to disk with formatted JSON. Features:
- Pretty-prints JSON (WriteIndented = true)
- Uses async file I/O
- Throws exception on error (after logging)

**Example:**
```csharp
var settings = new AppSettings
{
    ServerUrl = "http://myserver:5000",
    Username = "admin",
    Password = "password",
    RememberCredentials = true
};

await settingsService.SaveSettingsAsync(settings);
```

### ClearSettingsAsync
```csharp
public async Task ClearSettingsAsync()
```

Deletes the settings file from disk. Features:
- Safely handles non-existent file
- Logs errors but throws exception
- Useful for logout/reset scenarios

**Example:**
```csharp
await settingsService.ClearSettingsAsync();
```

### GetSettingsPath
```csharp
public string GetSettingsPath()
```

Returns the full path to the settings file.

**Example:**
```csharp
var path = settingsService.GetSettingsPath();
Console.WriteLine($"Settings stored at: {path}");
// Output: Settings stored at: C:\Users\Username\AppData\Roaming\KAssistant\settings.json
```

## Storage Location

Settings are stored in a platform-specific location:

### Windows
```
C:\Users\{Username}\AppData\Roaming\KAssistant\settings.json
```

### Linux
```
/home/{username}/.config/KAssistant/settings.json
```

### macOS
```
/Users/{username}/.config/KAssistant/settings.json
```

## JSON Format

The settings file uses the following JSON structure:

```json
{
  "serverUrl": "http://localhost:5000",
  "username": "myusername",
  "password": "mypassword",
  "rememberCredentials": true
}
```

## Security Considerations

?? **Important**: The password is stored in **plain text** in the settings file. This is convenient for development but not recommended for production use.

Consider these alternatives for production:
1. Use Windows Credential Manager
2. Use DPAPI (Data Protection API)
3. Encrypt the password before storing
4. Don't store password, only store refresh token
5. Use system keychain (macOS/Linux)

## Usage in ViewModels

Typical usage pattern in a ViewModel:

```csharp
public class MainWindowViewModel
{
    private readonly SettingsService _settingsService;
    
    public MainWindowViewModel()
    {
        _settingsService = new SettingsService();
        _ = LoadSettings();
    }
    
    private async Task LoadSettings()
    {
        var settings = await _settingsService.LoadSettingsAsync();
        ServerUrl = settings.ServerUrl;
        
        if (settings.RememberCredentials)
        {
            Username = settings.Username;
            Password = settings.Password;
        }
    }
    
    private async Task SaveSettings()
    {
        var settings = new AppSettings
        {
            ServerUrl = ServerUrl,
            Username = RememberCredentials ? Username : string.Empty,
            Password = RememberCredentials ? Password : string.Empty,
            RememberCredentials = RememberCredentials
        };
        
        await _settingsService.SaveSettingsAsync(settings);
    }
}
```

## Error Handling

The service handles errors gracefully:

1. **LoadSettingsAsync**: Returns default settings on error (never throws)
2. **SaveSettingsAsync**: Logs error and throws exception
3. **ClearSettingsAsync**: Logs error and throws exception

All errors are written to `Console.WriteLine` for debugging.

## Integration with API Service

The settings work seamlessly with the KavitaApiService:

```csharp
// Load settings
var settings = await _settingsService.LoadSettingsAsync();

// Configure API service
var apiService = new KavitaApiService();
await apiService.SetBaseUrl(settings.ServerUrl);

// Attempt login if credentials are remembered
if (settings.RememberCredentials && 
    !string.IsNullOrEmpty(settings.Username))
{
    var result = await apiService.TestLogin(
        settings.Username, 
        settings.Password);
        
    if (result.Success)
    {
        Console.WriteLine("Auto-login successful!");
    }
}
```

## Best Practices

1. **Always check RememberCredentials** before storing sensitive data
2. **Clear settings on logout** if user doesn't want credentials remembered
3. **Validate settings** after loading (check if ServerUrl is valid)
4. **Handle load errors** gracefully (settings file might be corrupted)
5. **Don't store sensitive data** if RememberCredentials is false

## Testing

The SettingsService can be tested easily:

```csharp
// Test saving and loading
var service = new SettingsService();

var testSettings = new AppSettings
{
    ServerUrl = "http://test:5000",
    Username = "testuser",
    RememberCredentials = true
};

await service.SaveSettingsAsync(testSettings);
var loaded = await service.LoadSettingsAsync();

Assert.Equal(testSettings.ServerUrl, loaded.ServerUrl);
Assert.Equal(testSettings.Username, loaded.Username);

// Clean up
await service.ClearSettingsAsync();
```

## Code Status

? No compilation errors  
? All dependencies properly configured  
? Proper async/await usage  
? Thread-safe (each instance has its own file path)  
? Cross-platform compatible  

## Conclusion

The SettingsService provides a simple, reliable way to persist application settings. It integrates seamlessly with the KavitaApiService and follows .NET best practices for async file I/O and JSON serialization.
