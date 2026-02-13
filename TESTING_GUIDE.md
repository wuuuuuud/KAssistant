# Testing Guide - Multiple Instance Fix

## How to Verify the Fix Works

### Test 1: Rapid Button Clicking ?

**Purpose**: Verify only one test session can run at a time

**Steps**:
1. Launch KAssistant
2. Enter any server URL (doesn't need to be valid for this test)
3. Enter any username and password
4. **Click "Run All Tests" button 10-20 times as fast as you can**

**Expected Results**:
- ? Only ONE test session should start
- ? Buttons should become disabled (grayed out) immediately
- ? You should see results from only ONE test run
- ? Status should show "Running tests..." or "Tests are already running..."
- ? NO "multiple instance" error should appear

**What Was Fixed**:
- Before: Multiple test sessions could start
- After: SemaphoreSlim prevents concurrent execution

---

### Test 2: Command While Running ?

**Purpose**: Verify commands are disabled during execution

**Steps**:
1. Enter valid server details
2. Click "Run All Tests"
3. **While tests are running, try clicking**:
   - "Test Connectivity" button
   - "Test Login" button  
   - "Run All Tests" button again
   - "Clear Results" button

**Expected Results**:
- ? All buttons should be disabled (unclickable)
- ? If you somehow click, status shows "A test is already running..."
- ? Original test session continues uninterrupted
- ? Only one set of results appears

**What Was Fixed**:
- Before: Commands could be invoked while running
- After: CanExecute pattern automatically disables commands

---

### Test 3: Invalid Server Timeout ?

**Purpose**: Verify state resets after errors/timeouts

**Steps**:
1. Enter an invalid server URL: `http://192.168.999.999:5000`
2. Enter any username/password
3. Click "Run All Tests"
4. Wait for timeout (10-30 seconds)

**Expected Results**:
- ? Tests should timeout with error messages
- ? After timeout, `IsRunning` should return to false
- ? Buttons should become enabled again
- ? Orange dot (¡ñ) should disappear
- ? You should be able to run tests again

**What Was Fixed**:
- Before: Timeout might leave IsRunning stuck
- After: Finally block + SemaphoreSlim.Release() guarantees cleanup

---

### Test 4: Visual Indicators ?

**Purpose**: Verify UI reflects state correctly

**Steps**:
1. Start any test
2. Observe the UI

**What to Check**:
- ? **Buttons**: Grayed out and unclickable while running
- ? **Orange Dot (¡ñ)**: Appears in status bar while running
- ? **Status Message**: Shows current operation
- ? **After completion**: All buttons re-enable, orange dot disappears

---

### Test 5: State After Exception ?

**Purpose**: Verify cleanup even when errors occur

**Steps**:
1. Enter valid URL but wrong credentials
2. Run tests
3. Authentication should fail

**Expected Results**:
- ? Test results show login failure
- ? State still resets properly
- ? Buttons become enabled
- ? You can try again with different credentials

---

## Common Scenarios

### Scenario: "I clicked too fast and got stuck"
**Should NOT happen anymore**
- SemaphoreSlim prevents concurrent execution
- If stuck, wait 30 seconds for timeout
- Check for orange dot (¡ñ) - if visible, tests are running

### Scenario: "Buttons won't enable"
**Should NOT happen anymore**  
- Finally blocks guarantee state reset
- If it happens:
  1. Wait 30 seconds (max timeout)
  2. Check status message
  3. If still stuck, restart app (this should be extremely rare)

### Scenario: "Multiple test results appear"
**Should NOT happen anymore**
- Only one execution allowed at a time
- If this happens, the fix didn't work (please report)

---

## Technical Verification

### Check the Code
Look for these patterns in `MainWindowViewModel.cs`:

```csharp
// ? SemaphoreSlim lock
private readonly SemaphoreSlim _executionLock = new(1, 1);

// ? NotifyCanExecuteChangedFor attribute
[NotifyCanExecuteChangedFor(nameof(RunAllTestsCommand))]
private bool _isRunning;

// ? CanExecute parameter
[RelayCommand(CanExecute = nameof(CanExecuteTests))]

// ? Semaphore check
if (!await _executionLock.WaitAsync(0).ConfigureAwait(false))
{
    StatusMessage = "Tests are already running...";
    return;
}

// ? Finally with Release
finally
{
    IsRunning = false;
    _executionLock.Release();
}
```

### Debug Mode
If you're debugging:
1. Set breakpoint at `if (!await _executionLock.WaitAsync(0))`
2. Click button multiple times rapidly
3. First click should enter the try block
4. Subsequent clicks should hit the return statement

---

## Success Criteria

The fix is working if:
- ? Rapid clicking only starts one test session
- ? Buttons disable immediately when tests start
- ? Buttons re-enable after tests complete/fail/timeout
- ? Status messages are clear
- ? No "multiple instance" errors occur
- ? Orange dot appears/disappears correctly

---

## If Issues Persist

If you still get "multiple instance" errors:

1. **Verify Version**: Check it's v1.0.2.3+
2. **Clean Build**: `dotnet clean` then `dotnet build`
3. **Check Code**: Ensure the SemaphoreSlim code is present
4. **Restart IDE**: Sometimes IntelliSense/caching issues
5. **Report**: If still failing, there's a deeper issue

---

## Version Info

This fix is in **version 1.0.2.3** and includes:
- ? SemaphoreSlim lock mechanism
- ? CanExecute pattern with NotifyCanExecuteChangedFor
- ? Three-layer protection
- ? Guaranteed cleanup in finally blocks

The "multiple instance" issue should be **completely resolved**.
