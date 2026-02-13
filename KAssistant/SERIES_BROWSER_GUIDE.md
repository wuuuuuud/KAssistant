# Series Browser User Guide

## Overview

The Series Browser is a window that displays all series (comics/manga/books) from your Kavita libraries. It allows you to browse, search, and filter your entire collection in one place.

## Opening the Series Browser

From the main window:
1. Click the **"Open Series Browser"** button
2. The browser will automatically load all series from all libraries

## Features

### 1. Library Filter

**Dropdown at the top**: Select which library to view
- **"All Libraries"** (default): Shows series from all libraries
- **Individual library**: Shows only series from that library

### 2. Search

**Search box**: Type to filter series by:
- Series name
- Original name
- Summary/description

The search is **instant** and case-insensitive.

### 3. Series List

**Main area**: Displays all matching series with:
- Series name
- Library name
- Number of pages read
- Total pages

### 4. Status Information

**Bottom bar shows**:
- Total series count: `(123 total)`
- Filtered count: `Showing 45 of 123`
- Load time: `Loaded in 2.3s`
- Current status messages

## How to Use

### Browse All Series

1. Open the Series Browser
2. Wait for series to load (shows "Loading...")
3. Scroll through the complete list
4. All series from all libraries are displayed

### Filter by Library

1. Click the **Library dropdown**
2. Select a specific library
3. List updates instantly to show only that library's series

### Search for Series

1. Type in the **search box**
2. Results filter instantly as you type
3. Matches are found in:
   - Series title
   - Original title
   - Summary text

### View Series Details

1. Double-click a series (or click "View" button)
2. Opens Metadata Viewer with full details
3. Shows genres, tags, summary, etc.

### Refresh Data

1. Click **"Refresh"** button
2. Reloads all series from server
3. Useful after adding new series to Kavita

### Clear Filters

1. Click **"Clear Filter"** button
2. Resets search text
3. Resets library filter to "All Libraries"

## Understanding the Data

### Series Properties Displayed

| Property | Description |
|----------|-------------|
| Name | Main series title |
| Library | Which library contains this series |
| Pages Read | How many pages you've read |
| Last Modified | When series was last updated |

### Status Messages

| Message | Meaning |
|---------|---------|
| "Loading libraries..." | Fetching library list |
| "Loading series from X libraries..." | Fetching series data |
| "Loaded X series successfully" | Load complete ? |
| "No series found in any library" | No series exist ?? |
| "Failed to load series: ..." | Error occurred ? |

## Performance

### Loading Time

Depends on your library size:
- **Small** (< 100 series): 1-2 seconds
- **Medium** (100-500 series): 3-5 seconds
- **Large** (500+ series): 5-10 seconds
- **Multiple libraries**: Cumulative time

### After Loading

- **Filtering**: Instant ?
- **Search**: Instant ?
- **Library switching**: Instant ?
- **Scrolling**: Smooth ??

All filtering happens client-side for speed!

## Troubleshooting

### "No series found in any library"

**Possible causes**:
1. Not logged in
2. No libraries configured
3. Libraries are empty
4. Network error

**Solutions**:
1. Ensure you're logged in to Kavita
2. Check that libraries exist in Kavita
3. Verify libraries contain series
4. Check server connection
5. Click "Refresh" to retry

### "Failed to load series"

**Possible causes**:
1. Network timeout
2. Server not responding
3. Authentication expired
4. API error

**Solutions**:
1. Check server URL is correct
2. Verify Kavita server is running
3. Re-login from main window
4. Click "Refresh" to retry
5. Check error details in message

### Slow Loading

**Possible causes**:
1. Large library (many series)
2. Slow network connection
3. Server under load

**Solutions**:
1. Be patient during initial load
2. Use library filter for specific libraries
3. Filtering is instant after load
4. Consider upgrading network/server

### Missing Series

**Possible causes**:
1. Library filter active
2. Search filter active
3. Series not in Kavita yet
4. Permissions issue

**Solutions**:
1. Click "Clear Filter"
2. Check "All Libraries" selected
3. Verify series exists in Kavita
4. Check you have access to library

## Tips & Tricks

### Quick Filtering

1. **By Library**: Use dropdown for instant library switching
2. **By Name**: Type part of series name in search
3. **Combined**: Use both library filter AND search together

### Finding Specific Series

1. Type unique part of title in search
2. Or use original name if different
3. Or search by content in summary

### Viewing Details

- Double-click any series for full metadata
- Use metadata viewer to see:
  - Full summary
  - Genres and tags
  - Publication info
  - Reading progress

### Keyboard Shortcuts

- **Escape**: Close window
- **F5**: Refresh (if implemented)
- **Ctrl+F**: Focus search box (if implemented)

## Data Freshness

### When Data is Loaded

- On window open
- When clicking "Refresh"
- Not automatically updated

### To Get Latest Data

1. Click "Refresh" button
2. Or close and reopen window
3. Or scan library in Kavita first

### What Gets Loaded

- Series name and metadata
- Library information
- Reading progress
- Last modified dates

### What Doesn't Get Loaded

- Individual chapter details
- Volume breakdowns
- File paths
- User reviews/ratings

## Integration with Main Window

### Opening from Main Window

- Click "Open Series Browser" button
- Requires active API connection
- Uses current authentication

### Returning to Main Window

- Click "Close" button
- Or click X in title bar
- Main window remains open

### Opening Multiple Browsers

- You can open multiple browser windows
- Each loads data independently
- Useful for comparing libraries

## Best Practices

### For Best Performance

1. **Initial Load**: Let it complete before filtering
2. **Large Collections**: Use library filter first
3. **Searching**: Type full words for best matches
4. **Refreshing**: Only when needed (after Kavita scan)

### For Organization

1. **Name Libraries Clearly**: Makes filtering easier
2. **Regular Scans**: Keep Kavita up to date
3. **Clean Metadata**: Better search results

### For Finding Content

1. **Know Your Library**: Faster with library filter
2. **Use Search**: For specific titles
3. **Browse Mode**: Discovery of new series

## Advanced Usage

### Filtering Workflow

1. Select library (narrows results)
2. Type search term (further narrows)
3. View series details
4. Clear filters to start over

### Batch Viewing

1. Browse filtered list
2. Open metadata viewers for multiple series
3. Compare side-by-side
4. Close viewers when done

### Managing Large Collections

1. Use library filter to segment
2. Search within filtered results
3. Take advantage of instant filtering
4. Remember all data is cached after load

## API Information

### Endpoints Used

- `GET /api/Library/libraries` - Get library list
- `POST /api/Series/all-v2` - Get series (per library)

### Data Loaded

- All series from all accessible libraries
- Complete metadata for each series
- Pagination handled automatically
- Multiple API calls for large collections

### Authentication

- Uses current session token
- Inherited from main window login
- Re-login if session expires

## Conclusion

The Series Browser provides a powerful way to:
- **Browse** your entire Kavita collection
- **Search** for specific series quickly
- **Filter** by library for organization
- **View** series details easily

It's designed for speed and ease of use, with instant filtering after the initial load. Perfect for managing large comic/manga collections! ??
