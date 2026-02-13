# Kavita API Testing Guide

## Overview

KAssistant now includes comprehensive API testing capabilities for Kavita servers. You can test individual endpoints or run all tests at once.

## Available API Tests

### 1. Basic Tests (No Authentication Required)

#### Connectivity Test
- **Endpoint**: `/api/health`
- **Purpose**: Verifies that the Kavita server is reachable
- **Requires Auth**: No

#### Server Info
- **Endpoint**: `/api/Server/server-info`
- **Purpose**: Retrieves server version, OS, and .NET version information
- **Requires Auth**: No

### 2. Authentication Test

#### Login Test
- **Endpoint**: `/api/Account/login`
- **Purpose**: Tests user authentication and retrieves JWT token
- **Requires Auth**: No (but provides authentication)
- **Required Fields**: Username, Password

### 3. Library & Content Tests (Requires Authentication)

#### Get Libraries
- **Endpoint**: `/api/Library`
- **Purpose**: Retrieves all libraries configured on the server
- **Requires Auth**: Yes
- **Returns**: List of libraries with ID, name, type, and last scan date

#### Recently Added Series
- **Endpoint**: `/api/Series/recently-added`
- **Purpose**: Retrieves the 10 most recently added series
- **Requires Auth**: Yes
- **Returns**: Paginated list of series

#### On Deck Series
- **Endpoint**: `/api/Series/on-deck`
- **Purpose**: Retrieves series that are currently being read
- **Requires Auth**: Yes
- **Returns**: Paginated list of in-progress series with reading progress

### 4. User Management Tests (Requires Authentication)

#### Get Users
- **Endpoint**: `/api/Users`
- **Purpose**: Retrieves all users on the server (admin only)
- **Requires Auth**: Yes
- **Returns**: List of users with their roles and status

### 5. Collections & Lists Tests (Requires Authentication)

#### Get Collections
- **Endpoint**: `/api/Collection`
- **Purpose**: Retrieves all collections
- **Requires Auth**: Yes
- **Returns**: List of collections with series count and promotion status

#### Get Reading Lists
- **Endpoint**: `/api/ReadingList`
- **Purpose**: Retrieves all reading lists
- **Requires Auth**: Yes
- **Returns**: List of reading lists with item count and details

### 6. Statistics Tests (Requires Authentication)

#### Server Stats
- **Endpoint**: `/api/Stats/server`
- **Purpose**: Retrieves server-wide statistics
- **Requires Auth**: Yes
- **Returns**: 
  - Total files
  - Total series
  - Total volumes
  - Total chapters
  - Total storage size

#### User Stats
- **Endpoint**: `/api/Stats/user`
- **Purpose**: Retrieves reading statistics for the current user
- **Requires Auth**: Yes
- **Returns**:
  - Total pages read
  - Total words read
  - Time spent reading
  - Chapters read

## How to Use

### Individual API Tests

1. **Enter Server Configuration**
   - Server URL (e.g., `http://localhost:5000`)
   - Username
   - Password

2. **Test Basic Connectivity**
   - Click "Test Connectivity" to verify server is reachable
   - Click "Server Info" to get version information

3. **Authenticate**
   - Click "Test Login" to authenticate
   - If successful, you can now test authenticated endpoints

4. **Test Individual APIs**
   - Use the individual test buttons in the "Individual Tests" section
   - Each button tests a specific API endpoint
   - Results appear in the test results panel below

### Run All Tests

Click "Run All Tests" to execute all API tests in sequence:
1. Connectivity test
2. Server info
3. Login
4. Libraries (if login successful)
5. Recently Added
6. On Deck
7. Users
8. Collections
9. Reading Lists
10. Server Stats
11. User Stats

## Understanding Test Results

Each test result shows:
- **Test Name**: The API endpoint being tested
- **Status**: ? PASS or ? FAIL
- **Message**: Summary of the result
- **Duration**: Time taken to complete the test
- **Details**: Additional information (collapsed by default)

### Success Indicators
- Green "? PASS" indicates successful API call
- Detailed information about returned data

### Failure Indicators
- Red "? FAIL" indicates failed API call
- Error message and status code
- Stack trace for debugging (if applicable)

## Troubleshooting

### "Server is not reachable"
- Verify the server URL is correct
- Ensure Kavita server is running
- Check firewall settings

### "Login failed"
- Verify username and password are correct
- Check if the user account is active (not pending)
- Ensure the user has appropriate permissions

### "Timeout" errors
- Server may be under heavy load
- Network connectivity issues
- Increase timeout settings if needed

### "401 Unauthorized" for authenticated tests
- Login may have expired
- Click "Test Login" again to refresh authentication
- Some endpoints require admin privileges

## Tips

1. **Save Settings**: Use "?? Save Settings" to remember server URL and credentials
2. **Clear Results**: Click "Clear Results" to remove previous test results
3. **Test Order**: Always test connectivity and login before authenticated endpoints
4. **Auto-Login**: Authenticated test commands will automatically attempt login if not authenticated
5. **Review Details**: Expand the details section for more information about API responses

## API Response Models

### Library Types
- `0`: Manga
- `1`: Comic
- `2`: Book

### User Roles
- `isAdmin: true`: Administrator account
- `isAdmin: false`: Regular user account

### Publication Status
- Available in series metadata
- Indicates if series is ongoing or completed

## Security Notes

- Credentials are only stored locally if "Remember credentials" is checked
- JWT tokens are only kept in memory during the session
- No credentials are transmitted to any third party
- All communication is direct between KAssistant and your Kavita server

## Version Compatibility

This tool is compatible with Kavita API version 0.7.x and above. Some endpoints may not be available in older versions.

## Contributing

If you'd like to see additional API endpoints tested, please submit a feature request or pull request on GitHub.
