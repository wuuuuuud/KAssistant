# ?? Updated UI with Library and Series Browser

## Overview

The UI has been significantly enhanced with a new "Browse and Metadata" panel that allows you to:
- Search for series by name
- Browse series in a specific library
- Get detailed metadata for any book/series
- Quick access to common operations

## New UI Sections

### ?? Browse and Metadata Panel

Located in the left column between "Server Configuration" and "API Tests".

#### Search Feature
```
©°©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©Ð©¤©¤©¤©¤©´
©¦ Search series...                    ©¦ ?? ©¦
©¸©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©Ø©¤©¤©¤©¤©¼
```
- **Input**: Enter series name to search
- **Button**: Click ?? to search
- **Result**: Displays search results in the Test Results panel

#### Library Browser
```
Library ID: [___1___] [Load Series]
```
- **Input**: Enter library ID (e.g., 1, 2, 3)
- **Button**: Click "Load Series" to list all series in that library
- **Result**: Displays series list with IDs in Test Results

#### Series Details
```
Series ID: [__123__] [Details] [Metadata]
```
- **Input**: Enter series ID
- **Buttons**:
  - `Details`: Get basic series information
  - `Metadata`: Get full metadata (summary, genres, tags, etc.)
- **Result**: Shows comprehensive information in Test Results

#### Quick Actions
```
[?? Libraries] [?? Recent] [?? On Deck]
```
- **Libraries**: List all libraries on server
- **Recent**: Show recently added series
- **On Deck**: Show series user is currently reading

## Complete Workflow Examples

### Workflow 1: Find and View Book Metadata

**Goal**: Search for a book and view its metadata

1. **Login**
   ```
   - Enter server URL, username, password
   - Click "Test Login"
   - Wait for success message
   ```

2. **Search for Series**
   ```
   - In "Search series..." box, enter: "One Piece"
   - Click ?? button
   - Review search results in right panel
   - Note the series ID from results
   ```

3. **Get Series Metadata**
   ```
   - In "Series ID" box, enter the ID (e.g., 123)
   - Click "Metadata" button
   - Review full metadata:
     ? Summary
     ? Age Rating
     ? Publication Status
     ? Language
     ? Genres (up to 10 shown)
     ? Tags (up to 10 shown)
   ```

### Workflow 2: Browse Library Contents

**Goal**: See all books in a specific library

1. **Login First**
   ```
   - Complete authentication as shown above
   ```

2. **Get Library ID**
   ```
   - Click "?? Libraries" quick action
   - Review library list in results
   - Note the library ID you want to browse
   ```

3. **Load Series List**
   ```
   - In "Library ID" box, enter: 1
   - Click "Load Series" button
   - Review series list with IDs
   ```

4. **View Series Details**
   ```
   - Pick a series ID from the list
   - Enter it in "Series ID" box
   - Click "Details" for basic info
   - OR click "Metadata" for full metadata
   ```

### Workflow 3: Check Recently Added Books

**Goal**: See what's new on the server

1. **Quick Check**
   ```
   - Login (if not already)
   - Click "?? Recent" quick action
   - Review recently added series
   ```

2. **Examine a New Book**
   ```
   - Pick a series ID from recent list
   - Enter in "Series ID" box
   - Click "Metadata" for full information
   ```

## UI Layout

```
©°©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©´
©¦                    KAssistant - Kavita API Tester               ©¦
©À©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©Ð©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©È
©¦  LEFT PANEL (400px)  ©¦         RIGHT PANEL (Main Area)          ©¦
©¦                      ©¦                                          ©¦
©¦  ©°©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©´©¦  Test Results                           ©¦
©¦  ©¦ Server Config    ©¦©¦  ©°©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©´©¦
©¦  ©¦ - URL            ©¦©¦  ©¦ ? Recently Added Series           ©¦©¦
©¦  ©¦ - Username       ©¦©¦  ©¦ Retrieved 10 of 25 series          ©¦©¦
©¦  ©¦ - Password       ©¦©¦  ©¦                                    ©¦©¦
©¦  ©¦ [Test Login]     ©¦©¦  ©¦ - One Piece                        ©¦©¦
©¦  ©¸©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¼©¦  ©¦ - Naruto                           ©¦©¦
©¦                      ©¦  ©¦ - Bleach                           ©¦©¦
©¦  ©°©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©´©¦  ©¦ ...                                ©¦©¦
©¦  ©¦ Browse & Metadata©¦©¦  ©¸©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¼©¦
©¦  ©¦ [Search...]   ?? ©¦©¦                                          ©¦
©¦  ©¦ Library ID: [1]  ©¦©¦  ©°©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©´©¦
©¦  ©¦ Series ID: [123] ©¦©¦  ©¦ ? Get Series Metadata             ©¦©¦
©¦  ©¦ [Details][Meta]  ©¦©¦  ©¦ Metadata retrieved for series 123  ©¦©¦
©¦  ©¦                  ©¦©¦  ©¦                                    ©¦©¦
©¦  ©¦ Quick Actions:   ©¦©¦  ©¦ Summary: Epic adventure...         ©¦©¦
©¦  ©¦ [??][??][??]      ©¦©¦  ©¦ Age Rating: Teen                   ©¦©¦
©¦  ©¸©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¼©¦  ©¦ Genres: Action, Adventure, Fantasy ©¦©¦
©¦                      ©¦  ©¦ Tags: Swords, Magic, Dragons...    ©¦©¦
©¦  ©°©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©´©¦  ©¸©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¼©¦
©¦  ©¦ API Tests        ©¦©¦                                          ©¦
©¦  ©¦ [Run All Tests]  ©¦©¦  Status: Metadata retrieved: Metadata  ©¦
©¦  ©¦ [Clear Results]  ©¦©¦         retrieved for series 123      ©¦
©¦  ©¦                  ©¦©¦                                          ©¦
©¦  ©¦ Individual Tests:©¦©¦                                          ©¦
©¦  ©¦ [Connectivity]   ©¦©¦                                          ©¦
©¦  ©¦ [Server Info]    ©¦©¦                                          ©¦
©¦  ©¦ [Libraries]      ©¦©¦                                          ©¦
©¦  ©¦ ...              ©¦©¦                                          ©¦
©¦  ©¸©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¼©¦                                          ©¦
©¸©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©Ø©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¼
```

## New Commands in ViewModel

### Search and Browse Commands

| Command | Property | Description |
|---------|----------|-------------|
| `SearchSeriesCommand` | `SearchTerm` | Search for series by name |
| `LoadLibrariesCommand` | - | List all libraries |
| `LoadSeriesForLibraryCommand` | `SelectedLibraryId` | List series in library |
| `GetSeriesDetailsCommand` | `SelectedSeriesId` | Get basic series info |
| `GetSeriesMetadataCommand` | `SelectedSeriesId` | Get full metadata |

### Observable Properties

```csharp
[ObservableProperty]
private int _selectedLibraryId;  // Library ID input

[ObservableProperty]
private int _selectedSeriesId;   // Series ID input

[ObservableProperty]
private string _searchTerm = ""; // Search input
```

### Collections

```csharp
public ObservableCollection<Library> Libraries { get; }    // Library list
public ObservableCollection<Series> SeriesList { get; }   // Series list
```

## Auto-Authentication

All new browse/metadata commands automatically handle authentication:

```csharp
private async Task EnsureAuthenticated()
{
    // Automatically logs in if not authenticated
    // Uses saved username/password
    // Throws error if credentials missing
}
```

**Benefits**:
- No need to manually login before browsing
- Automatic token refresh if expired
- Clear error messages if authentication fails

## Error Handling

### Common Scenarios

1. **Not Logged In**
   ```
   ? Error
   Message: Please login first
   Details: Authentication required for this operation
   ```
   **Solution**: Enter credentials and click "Test Login"

2. **Invalid Library ID**
   ```
   ? Failed: NotFound
   Message: Library with ID 999 does not exist
   ```
   **Solution**: Use "?? Libraries" to see valid IDs

3. **Invalid Series ID**
   ```
   ? Failed: NotFound
   Message: Series with ID 99999 not found
   ```
   **Solution**: Use "Load Series" to see valid IDs

4. **Empty Search**
   ```
   Status: Please enter a search term
   ```
   **Solution**: Type something in the search box first

## Tips and Best Practices

### 1. Get Library IDs First
```
Step 1: Click "?? Libraries"
Step 2: Note the library IDs from results
Step 3: Use those IDs to browse series
```

### 2. Use Search to Find Series
```
Step 1: Enter partial name (e.g., "one")
Step 2: Click ??
Step 3: Get series IDs from results
Step 4: Use IDs for metadata lookup
```

### 3. Bookmark Common IDs
```
Keep a note of frequently used:
- Library IDs (e.g., Manga=1, Comics=2)
- Favorite series IDs
- Makes browsing faster
```

### 4. Use Quick Actions
```
Instead of "Run All Tests":
- Use "?? Libraries" for quick library check
- Use "?? Recent" to see new additions
- Use "?? On Deck" for current reads
```

### 5. Review Details Before Metadata
```
Series "Details" = Quick info
Series "Metadata" = Full metadata (larger response)

Use Details first, then Metadata if needed
```

## Keyboard Workflow

1. **Tab** through fields efficiently:
   ```
   Tab ¡ú Search box
   Tab ¡ú Library ID
   Tab ¡ú Series ID
   Enter ¡ú Triggers focused button's command
   ```

2. **Copy/Paste Series IDs**:
   ```
   - From results panel
   - Into Series ID box
   - Quick metadata lookup
   ```

## Result Display

### Series Details Format
```
? Get Series Details
Retrieved 'One Piece'

Name: One Piece
Original Name: ¥ï¥ó¥Ô©`¥¹
ID: 123
Library ID: 1
Created: 2024-01-01 00:00:00
Last Modified: 2024-01-15 12:00:00
Pages Read: 150
```

### Metadata Format
```
? Get Series Metadata
Metadata retrieved for series 123

Summary: Monkey D. Luffy and his crew of...
Age Rating: Teen
Publication Status: Ongoing
Language: English

Genres (8):
  - Action
  - Adventure
  - Comedy
  - Drama
  - Fantasy
  - Shounen
  - Supernatural
  - Martial Arts

Tags (15):
  - Pirates
  - Treasure
  - Devil Fruit
  - Grand Line
  - Marines
  - Straw Hat Pirates
  - One Piece Treasure
  - World Government
  - Haki
  - Bounty Hunters
  ... and 5 more
```

## Performance Considerations

1. **Page Size**: Series lists limited to 20 items
2. **Metadata Loading**: Only loads when requested
3. **Auto-Login**: Cached token reused
4. **Result Streaming**: Results appear as they complete

## Integration with Existing Features

### Settings Persistence
- Library ID and Series ID remembered during session
- Search term persists until cleared
- Credentials saved if "Remember" checked

### Test Results Panel
- All browse operations show in same panel
- Same formatting as API tests
- Duration tracking for performance analysis

### Status Bar
- Real-time operation status
- Clear success/failure messages
- Running indicator (¡ñ) when busy

## Accessibility

- **Tab Order**: Logical flow through inputs
- **Labels**: Clear field descriptions
- **Watermarks**: Example values shown
- **Buttons**: Descriptive text
- **Status**: Always visible at bottom

## Future Enhancements

Potential additions:
- ? Series list with clickable items
- ? Library dropdown selector
- ? Metadata editing interface
- ? Cover image preview
- ? Favorite series bookmarks
- ? Export metadata to file

## Summary

### What's New
- ? Search series by name
- ? Browse series by library
- ? View series details
- ? View full metadata
- ? Quick action buttons
- ? Auto-authentication
- ? Clean, organized layout

### Total Buttons
**Browse Section**: 7 buttons
- Search (??)
- Load Series
- Details
- Metadata
- ?? Libraries
- ?? Recent
- ?? On Deck

**Total UI Buttons**: 18+ (including all API test buttons)

### Total Input Fields
- Server URL
- Username
- Password
- Search Term (new)
- Library ID (new)
- Series ID (new)

**Total**: 6 input fields

---

**The UI now provides a complete browsing and metadata viewing experience for your Kavita library!** ???

You can search, browse, and examine any book's metadata without leaving the application.
