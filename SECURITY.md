# Security Notice

## Credential Storage

KAssistant provides the option to save your Kavita server credentials locally for convenience. Please be aware of the following:

### How Credentials Are Stored

- **Location**: `%AppData%\KAssistant\settings.json` (Windows)
- **Format**: Plain text JSON file
- **Contents**: Server URL, username, and password (if "Remember credentials" is checked)

### Security Considerations

?? **Important**: Credentials are stored in **plain text** without encryption.

#### Recommendations:

1. **Only use on trusted devices**
   - Do not enable "Remember credentials" on shared or public computers
   - Ensure your computer has proper user account protection

2. **File system security**
   - The settings file is stored in your user's AppData folder
   - Other users on the same computer cannot access it (by default)
   - Malware or applications running under your user account could access it

3. **Alternative approaches**
   - Manually enter credentials each time for maximum security
   - Use a password manager to store credentials
   - Consider using a dedicated Kavita test account with limited permissions

4. **Clear settings when done**
   - Use the "Clear Settings" button to remove saved credentials
   - Delete the settings file manually if needed

### Best Practices

- ? Use "Remember credentials" only on personal, secure devices
- ? Keep your operating system and antivirus up to date
- ? Use strong, unique passwords for your Kavita account
- ? Clear saved credentials before lending or selling your device
- ? Consider using test accounts rather than admin accounts

- ? Don't save admin credentials on shared computers
- ? Don't use the same password across multiple services
- ? Don't forget to clear settings on public/shared machines

### Future Improvements

Potential security enhancements being considered:
- Encrypted credential storage using Windows Data Protection API (DPAPI)
- Option to store only server URL, not credentials
- Integration with system credential managers
- Session-only credential storage

### Manual Settings Removal

If you want to manually remove saved settings:

1. Press `Win + R`
2. Type: `%AppData%\KAssistant`
3. Delete the `settings.json` file

Or use the "Clear Settings" button in the application.

## Reporting Security Issues

If you discover a security vulnerability, please email the maintainer or open a GitHub issue.

---

**Remember**: The security of your credentials is ultimately your responsibility. Use the "Remember credentials" feature at your own discretion based on your security requirements and environment.
