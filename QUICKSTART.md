# Quick Start Guide

## First Time Setup

1. **Launch KAssistant**
   - The application will start with default settings

2. **Configure Your Kavita Server**
   ```
   Server URL: http://localhost:5000
   Username: your_username
   Password: your_password
   ```

3. **Test Connection**
   - Click "Test Connectivity" to verify the server is reachable
   - Look for green "? PASS" in the results

4. **Authenticate**
   - Click "Test Login" to verify your credentials
   - A successful login will retrieve an authentication token

5. **Run Full Tests**
   - Click "Run All Tests" to execute all available API tests
   - Watch the status bar for progress
   - View detailed results for each test

## Saving Your Settings

### To Save Credentials:

1. Check "Remember credentials (saved locally)"
2. Click "?? Save Settings"
3. Next time you open the app, your settings will load automatically

### To Clear Saved Settings:

1. Click "??? Clear Settings"
2. Or manually delete: `%AppData%\KAssistant\settings.json`

## Understanding Test Results

Each test result shows:

```
[Test Name]                          ? PASS / ? FAIL
[Message about the test outcome]
Duration: 0.25s
[Detailed response information]
```

### Color Coding:
- **Green (? PASS)**: Test succeeded
- **Red (? FAIL)**: Test failed

### Status Bar:
- Shows current operation
- Orange dot (¡ñ) indicates tests are running
- Controls are disabled during test execution

## Common Workflows

### Quick Health Check
```
1. Enter server URL
2. Click "Test Connectivity"
3. Review result
```

### Verify Authentication
```
1. Enter all credentials
2. Click "Test Login"
3. Check for successful token retrieval
```

### Full API Validation
```
1. Enter all credentials
2. Check "Remember credentials" (optional)
3. Click "?? Save Settings" (optional)
4. Click "Run All Tests"
5. Wait for all tests to complete
6. Review all results
```

## Tips

- **Server URL Format**: Always include `http://` or `https://`
- **Port Number**: Default Kavita port is 5000
- **Admin Account**: Some tests require admin privileges
- **Clear Results**: Use "Clear Results" to clean up before new tests
- **Running Indicator**: Watch the orange dot (¡ñ) to see when tests are active

## Troubleshooting

### "Cannot reach server"
- ? Check if Kavita is running
- ? Verify the URL is correct
- ? Check firewall settings
- ? Try `http://127.0.0.1:5000` instead of localhost

### "Login failed"
- ? Verify username and password
- ? Check caps lock is off
- ? Ensure the account is active in Kavita

### "Tests are already running"
- Wait for current tests to complete
- Look for the orange dot (¡ñ) in status bar
- If stuck, restart the application

### Settings not saving
- ? Check write permissions to AppData folder
- ? Ensure "Remember credentials" is checked
- ? Click "?? Save Settings" button

## Keyboard Shortcuts

Currently, all interactions require clicking buttons. Keyboard shortcuts may be added in future versions.

## Getting Help

If you encounter issues:
1. Check the README.md for detailed information
2. Review SECURITY.md for credential safety
3. Check CHANGELOG.md for recent changes
4. Review test results for error messages

## Example Configurations

### Local Development
```
Server URL: http://localhost:5000
Username: admin
Password: [your password]
```

### Network Server
```
Server URL: http://192.168.1.100:5000
Username: testuser
Password: [your password]
```

### Custom Port
```
Server URL: http://localhost:8080
Username: admin
Password: [your password]
```

### HTTPS Server
```
Server URL: https://kavita.yourdomain.com
Username: admin
Password: [your password]
```
