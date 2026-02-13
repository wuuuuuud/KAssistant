# Trust nothing below, all AI generated rubbish
# KAssistant - Kavita API Tester

A .NET 8 Avalonia application for testing Kavita server APIs.

## Overview

KAssistant is a comprehensive testing tool for Kavita, a popular self-hosted digital library. It allows you to test various API endpoints and verify your Kavita server's functionality.

## Features

### Latest Features (v1.1.0)

? **Individual API Testing**
- Test each API endpoint separately with dedicated buttons
- Auto-login for authenticated endpoints
- Better control over which APIs to test

? **Extended API Coverage**
- **On Deck Series**: See what users are currently reading
- **Collections**: Test collection management endpoints
- **Reading Lists**: Verify reading list functionality
- **Server Statistics**: Get comprehensive server stats
- **User Statistics**: View individual user reading statistics

### Previous Features (v1.0.2.2)

? **Credential Management**
- Save server URL and credentials locally
- "Remember credentials" checkbox option
- Settings stored in AppData folder
- Quick clear settings functionality

? **Bug Fixes**
- Fixed "connection ongoing" state bug
- Fixed threading issues with UI updates
- Proper IsRunning state management
- Prevention of concurrent test execution

### Implemented API Tests

#### Basic Tests (No Authentication)
1. **Connectivity Test** - `/api/health`
2. **Server Info** - `/api/Server/server-info`

#### Authentication
3. **Login** - `/api/Account/login`

#### Library & Content (Requires Authentication)
4. **Get Libraries** - `/api/Library`
5. **Recently Added Series** - `/api/Series/recently-added`
6. **On Deck Series** - `/api/Series/on-deck` ? NEW
7. **Get Collections** - `/api/Collection` ? NEW
8. **Get Reading Lists** - `/api/ReadingList` ? NEW

#### User Management (Requires Authentication)
9. **Get Users** - `/api/Users`

#### Statistics (Requires Authentication)
10. **Server Stats** - `/api/Stats/server` ? NEW
11. **User Stats** - `/api/Stats/user` ? NEW

## How to Use

### Quick Start

1. **Configure Connection**
   - Enter your Kavita server URL (e.g., `http://localhost:5000`)
   - Enter your username and password
   - (Optional) Check "Remember credentials"

2. **Basic Testing**
   - Click "Test Connectivity" to verify server is reachable
   - Click "Test Login" to authenticate

3. **Run All Tests**
   - Click "Run All Tests" to execute all API tests at once
   - View results in the results panel

### Individual API Testing ? NEW

Use the "Individual Tests" buttons to test specific endpoints:

- **Server Info**: Get Kavita version and system information
- **Libraries**: View all configured libraries
- **Recently Added**: See newly added series
- **On Deck**: View in-progress series
- **Users**: List all server users
- **Collections**: View collections
- **Reading Lists**: See reading lists
- **Server Stats**: Comprehensive server statistics
- **User Stats**: Your reading statistics

**Note**: Authenticated endpoints will automatically attempt login if needed.

### Save Settings

- Click "?? Save Settings" to persist configuration
- Settings saved to: `%AppData%/KAssistant/settings.json`
- Click "??? Clear Settings" to delete saved settings

### Understanding Results

Each test displays:
- ? **PASS** (green) or ? **FAIL** (red) status
- Response message summary
- Execution duration
- Detailed response data (expandable)

## Settings Storage

Settings are stored in JSON format at:
```
Windows: %AppData%\KAssistant\settings.json
```

**Security Note**: Credentials are stored in plain text locally. Only use "Remember credentials" on trusted devices.

## API Documentation

For detailed information about each API endpoint, see [API_TESTING_GUIDE.md](API_TESTING_GUIDE.md).

## Kavita API Reference

### Authentication Flow
1. Login with username/password
2. Receive JWT token
3. Token included in Authorization header for subsequent requests

### Endpoint Categories

- **Public**: `/api/health`, `/api/Server/server-info`
- **Authentication**: `/api/Account/login`
- **Libraries**: `/api/Library`
- **Series**: `/api/Series/*`
- **Users**: `/api/Users`
- **Collections**: `/api/Collection`
- **Reading Lists**: `/api/ReadingList`
- **Statistics**: `/api/Stats/*`

## Models

The application includes comprehensive models for:
- Authentication (LoginRequest, AuthenticationResponse)
- Server Info (ServerInfoResponse)
- Libraries (Library)
- Series (Series, PaginatedResult)
- Users (User)
- Collections (Collection) ? NEW
- Reading Lists (ReadingList) ? NEW
- Statistics (ServerStats, UserStats) ? NEW
- Test Results (ApiTestResult)
- App Settings (AppSettings)

## Technical Details

- **Framework**: .NET 8
- **UI Framework**: Avalonia 11.3.11
- **MVVM**: CommunityToolkit.Mvvm 8.2.1
- **HTTP Client**: System.Net.Http with JSON support
- **Settings**: JSON file-based persistence

## Project Structure

```
KAssistant/
念岸岸 Models/
岫   念岸岸 KavitaModels.cs          # Data models for Kavita API
岫   弩岸岸 AppSettings.cs           # Application settings model
念岸岸 Services/
岫   念岸岸 KavitaApiService.cs      # API client service
岫   弩岸岸 SettingsService.cs       # Settings persistence service
念岸岸 ViewModels/
岫   念岸岸 MainWindowViewModel.cs   # Main window logic
岫   弩岸岸 ViewModelBase.cs         # Base view model
念岸岸 Views/
岫   弩岸岸 MainWindow.axaml         # Main window UI
念岸岸 Converters/
岫   弩岸岸 ValueConverters.cs       # XAML value converters
弩岸岸 KAssistant.csproj            # Project file
```

## Troubleshooting

### "Call from invalid thread" error
**Fixed in v1.0.2.2** - Removed `ConfigureAwait(false)` from ViewModel to ensure UI thread affinity.

### Tests show "connection ongoing" continuously
**Fixed in v1.0.2.2** - The IsRunning state is now properly managed with semaphore locking.

### Settings not loading
- Check if `settings.json` exists in `%AppData%\KAssistant\`
- Ensure the application has write permissions to AppData folder
- Try clicking "Clear Settings" and reconfiguring

### Connection fails
- Verify Kavita server is running
- Check firewall settings
- Ensure correct URL format (include http:// or https://)
- Try "Test Connectivity" first before other tests

### Authenticated tests fail
- Click "Test Login" to authenticate first
- Some endpoints require admin privileges
- Check if JWT token has expired (re-login if needed)

## Future Enhancements

Potential additions:
- Test more endpoints (Volumes, Chapters, Reading Progress)
- Export test results to file
- Automated test scheduling
- Performance benchmarking
- WebSocket connection testing
- File upload/download testing
- Metadata editing tests
- Encrypted credential storage
- Test history and comparison

## Kavita Resources

- **Official Website**: https://www.kavitareader.com/
- **GitHub**: https://github.com/Kareadita/Kavita
- **Documentation**: https://wiki.kavitareader.com/
- **API Documentation**: Check your Kavita instance at `/swagger`

## Contributing

Contributions are welcome! If you'd like to add more API tests or features:
1. Fork the repository
2. Create a feature branch
3. Submit a pull request

## License

This is a testing tool for Kavita. Please refer to Kavita's license for API usage terms.
