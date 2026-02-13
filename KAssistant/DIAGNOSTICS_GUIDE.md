# Kavita API Diagnostics Runner

## Purpose
This tool helps diagnose what's actually wrong with the Kavita API connection and series loading.

## How to Use

### Option 1: Add to MainWindow
Add this button to your MainWindow.axaml:

```xml
<Button Content="Run Diagnostics" Command="{Binding RunDiagnosticsCommand}" />
```

Then add this to MainWindowViewModel.cs:

```csharp
[RelayCommand]
private async Task RunDiagnostics()
{
    StatusMessage = "Running diagnostics...";
    await KAssistant.Diagnostics.KavitaDiagnostics.RunDiagnostics(
        ServerUrl,
        Username,
        Password
    );
    StatusMessage = "Diagnostics complete - check Output window";
}
```

### Option 2: Run from Program.cs

Replace the content of Program.cs temporarily:

```csharp
using System;
using System.Threading.Tasks;
using KAssistant.Diagnostics;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Enter Kavita Server URL (e.g., http://localhost:5000):");
        var url = Console.ReadLine() ?? "http://localhost:5000";
        
        Console.WriteLine("Enter username:");
        var username = Console.ReadLine() ?? "";
        
        Console.WriteLine("Enter password:");
        var password = Console.ReadLine() ?? "";
        
        await KavitaDiagnostics.RunDiagnostics(url, username, password);
        
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
```

### Option 3: PowerShell Script

Create a file `RunDiagnostics.ps1`:

```powershell
$serverUrl = Read-Host "Enter Kavita Server URL (e.g., http://localhost:5000)"
$username = Read-Host "Enter username"
$password = Read-Host "Enter password" -AsSecureString
$passwordPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($password))

cd KAssistant
dotnet run --no-build --property:StartupObject=KAssistant.Diagnostics.DiagnosticsRunner -- $serverUrl $username $passwordPlain
```

## What the Diagnostics Test

1. **Connectivity** - Can we reach the Kavita server?
2. **Authentication** - Can we login with your credentials?
3. **Libraries** - Can we fetch libraries?
4. **Recently-Added** - Does this endpoint work?
5. **All-V2** - Does this endpoint work?

## What to Look For

### If you see ? for all tests:
- The API connection works
- The problem is likely in the DTO mapping
- Check the "First series object structure" output to see what fields are actually returned

### If you see ? Connection failed:
- Check the server URL is correct
- Make sure Kavita is running
- Check firewall settings

### If you see ? Login failed:
- Check username/password are correct
- Check authentication method in Kavita settings

### If you see ? for recently-added or all-v2:
- The endpoint doesn't exist or has different name
- Check your Kavita version
- Try alternative endpoints

## Reading the Output

The diagnostics will show you:

1. **HTTP Status Codes** - 200 OK, 404 Not Found, 401 Unauthorized, etc.
2. **Actual JSON Response** - What the API actually returns
3. **Field Structure** - What properties are in the Series objects
4. **Error Messages** - Any exceptions or errors

## Common Issues and Solutions

### Issue: "Connection failed: 404"
**Solution**: Server URL is wrong or Kavita not running

### Issue: "Login failed: 401"
**Solution**: Wrong username/password

### Issue: "Recently-added failed: 404"
**Solution**: Endpoint doesn't exist - Kavita version mismatch

### Issue: "Could not parse response"
**Solution**: Response format doesn't match our DTOs

### Issue: JSON shows extra fields
**Solution**: Need to add those fields to Series class

### Issue: JSON shows different field names
**Solution**: Need to update JsonPropertyName attributes

## Next Steps After Running Diagnostics

1. **Copy the entire output** from the console
2. **Look for the "First series object structure"** section
3. **Compare with our Series class** in KavitaModels.cs
4. **Add any missing fields** to the Series class
5. **Fix any mismatched property names**

## Example Good Output

```
=== KAVITA API DIAGNOSTICS ===

1. Testing connectivity to Kavita server...
   ? Connected! Response: {"kavitaVersion":"0.8.3.1"...

2. Testing login...
   ? Login successful!
   Token: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

3. Testing libraries endpoint...
   ? Libraries endpoint works!
   Response: [{"id":1,"name":"Manga","type":1...

4. Testing recently-added endpoint...
   ? Recently-added endpoint works!
   Response: {"currentPage":0,"pageSize":5,"totalCount":45...
   Series count in response: 5
   
   First series object structure:
      - id: Number
      - name: String
      - pages: Number
      - wordCount: Number
      (etc.)

5. Testing all-v2 endpoint...
   ? All-v2 endpoint works!

=== DIAGNOSTICS COMPLETE ===
```

## Example Bad Output

```
=== KAVITA API DIAGNOSTICS ===

1. Testing connectivity to Kavita server...
   ? Connection failed: NotFound
   
=== DIAGNOSTICS COMPLETE ===
```

This means your server URL is wrong or Kavita isn't running.

## Getting Help

When asking for help, provide:
1. Complete diagnostic output
2. Your Kavita version
3. Your server URL format
4. Any error messages from Visual Studio Output window
