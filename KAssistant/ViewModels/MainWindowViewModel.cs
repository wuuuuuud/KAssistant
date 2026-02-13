using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KAssistant.Models;
using KAssistant.Services;

namespace KAssistant.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly KavitaApiService _apiService;
        private readonly SettingsService _settingsService;
        private readonly SemaphoreSlim _executionLock = new(1, 1);

        [ObservableProperty]
        private string _serverUrl = "http://localhost:5000";

        [ObservableProperty]
        private string _username = "";

        [ObservableProperty]
        private string _password = "";

        [ObservableProperty]
        private bool _rememberCredentials = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RunAllTestsCommand))]
        [NotifyCanExecuteChangedFor(nameof(TestConnectivityCommand))]
        [NotifyCanExecuteChangedFor(nameof(TestLoginCommand))]
        [NotifyCanExecuteChangedFor(nameof(ClearResultsCommand))]
        [NotifyCanExecuteChangedFor(nameof(LoadLibrariesCommand))]
        [NotifyCanExecuteChangedFor(nameof(OpenSeriesBrowserCommand))]
        private bool _isRunning;

        [ObservableProperty]
        private string _statusMessage = "Ready to test Kavita API";

        [ObservableProperty]
        private int _selectedLibraryId;

        [ObservableProperty]
        private int _selectedSeriesId;

        [ObservableProperty]
        private string _searchTerm = "";

        public ObservableCollection<ApiTestResult> TestResults { get; } = new();
        public ObservableCollection<Library> Libraries { get; } = new();
        public ObservableCollection<Series> SeriesList { get; } = new();

        public MainWindowViewModel()
        {
            _apiService = new KavitaApiService();
            _settingsService = new SettingsService();
            
            // Load settings asynchronously
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await LoadSettingsAsync();
        }

        private async Task LoadSettingsAsync()
        {
            try
            {
                var settings = await _settingsService.LoadSettingsAsync();
                ServerUrl = settings.ServerUrl;
                RememberCredentials = settings.RememberCredentials;
                
                if (settings.RememberCredentials)
                {
                    Username = settings.Username;
                    Password = settings.Password;
                    StatusMessage = "Credentials loaded from saved settings";
                }
            }
            catch (System.Exception ex)
            {
                StatusMessage = $"Error loading settings: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task SaveSettings()
        {
            try
            {
                var settings = new AppSettings
                {
                    ServerUrl = ServerUrl,
                    Username = RememberCredentials ? Username : string.Empty,
                    Password = RememberCredentials ? Password : string.Empty,
                    RememberCredentials = RememberCredentials
                };

                await _settingsService.SaveSettingsAsync(settings);
                StatusMessage = "Settings saved successfully";
            }
            catch (System.Exception ex)
            {
                StatusMessage = $"Error saving settings: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task ClearSettings()
        {
            try
            {
                await _settingsService.ClearSettingsAsync();
                ServerUrl = "http://localhost:5000";
                Username = string.Empty;
                Password = string.Empty;
                RememberCredentials = false;
                StatusMessage = "Settings cleared";
            }
            catch (System.Exception ex)
            {
                StatusMessage = $"Error clearing settings: {ex.Message}";
            }
        }

        // Library and Series Management Commands

        [RelayCommand(CanExecute = nameof(CanExecuteTests))]
        private async Task LoadLibraries()
        {
            await ExecuteTest(async () =>
            {
                StatusMessage = "Loading libraries...";
                await EnsureAuthenticated();

                Libraries.Clear();
                SeriesList.Clear();

                // Use the new typed API method
                var libraries = await _apiService.GetLibrariesAsync();
                
                if (libraries != null && libraries.Count > 0)
                {
                    foreach (var lib in libraries.OrderBy(l => l.Name))
                    {
                        Libraries.Add(lib);
                    }

                    var result = new ApiTestResult
                    {
                        TestName = "Get Libraries",
                        Success = true,
                        Message = $"Found {libraries.Count} libraries",
                        Details = string.Join("\n", libraries.Select(l => $"- {l.Name} (ID: {l.Id}, Type: {l.Type})")),
                        Duration = System.TimeSpan.Zero
                    };
                    TestResults.Add(result);

                    StatusMessage = $"Loaded {libraries.Count} libraries successfully";
                }
                else
                {
                    var result = new ApiTestResult
                    {
                        TestName = "Get Libraries",
                        Success = false,
                        Message = "No libraries found or failed to load",
                        Details = "The server returned no libraries. Check if libraries are configured.",
                        Duration = System.TimeSpan.Zero
                    };
                    TestResults.Add(result);

                    StatusMessage = "No libraries found";
                }
            });
        }

        [RelayCommand(CanExecute = nameof(CanExecuteTests))]
        private async Task OpenSeriesBrowser()
        {
            try
            {
                IsRunning = true;
                StatusMessage = "Preparing series browser...";
                
                // Ensure we have server URL and credentials
                if (string.IsNullOrWhiteSpace(ServerUrl))
                {
                    StatusMessage = "Please enter server URL first";
                    return;
                }
                
                // Set base URL first
                await _apiService.SetBaseUrl(ServerUrl);
                
                // Ensure we're logged in before opening browser
                if (!_apiService.IsAuthenticated)
                {
                    if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                    {
                        StatusMessage = "Please enter username and password, then login first";
                        return;
                    }
                    
                    StatusMessage = "Logging in...";
                    var loginResult = await _apiService.TestLogin(Username, Password);
                    if (!loginResult.Success)
                    {
                        StatusMessage = $"Login failed: {loginResult.Message}";
                        TestResults.Add(loginResult);
                        return;
                    }
                }

                // Create and show the series browser window
                var viewModel = new SeriesBrowserViewModel(_apiService);
                var window = new Views.SeriesBrowserWindow
                {
                    DataContext = viewModel
                };

                viewModel.SetCloseAction(() => window.Close());
                window.Show();

                StatusMessage = "Opened series browser";
            }
            catch (System.Exception ex)
            {
                StatusMessage = $"Error opening series browser: {ex.Message}";
                TestResults.Add(new ApiTestResult
                {
                    TestName = "Open Browser",
                    Success = false,
                    Message = $"Error: {ex.Message}",
                    Details = ex.ToString(),
                    Duration = System.TimeSpan.Zero
                });
            }
            finally
            {
                IsRunning = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteTests))]
        private async Task LoadSeriesForLibrary()
        {
            if (SelectedLibraryId <= 0)
            {
                StatusMessage = "Please select a library first";
                return;
            }

            await ExecuteTest(async () =>
            {
                StatusMessage = $"Loading series for library {SelectedLibraryId}...";
                await EnsureAuthenticated();

                SeriesList.Clear();

                var result = await _apiService.TestGetSeriesForLibrary(SelectedLibraryId);
                TestResults.Add(result);

                StatusMessage = result.Success 
                    ? $"Loaded series: {result.Message}" 
                    : $"Failed: {result.Message}";
            });
        }

        [RelayCommand]
        private async Task GetSeriesMetadata()
        {
            if (SelectedSeriesId <= 0)
            {
                StatusMessage = "Please enter a valid Series ID";
                return;
            }

            await ExecuteTest(async () =>
            {
                StatusMessage = $"Getting metadata for series {SelectedSeriesId}...";
                await EnsureAuthenticated();

                var result = await _apiService.TestGetSeriesMetadata(SelectedSeriesId);
                TestResults.Add(result);

                StatusMessage = result.Success 
                    ? $"Metadata retrieved: {result.Message}" 
                    : $"Failed: {result.Message}";
            });
        }

        [RelayCommand]
        private async Task GetSeriesDetails()
        {
            if (SelectedSeriesId <= 0)
            {
                StatusMessage = "Please enter a valid Series ID";
                return;
            }

            await ExecuteTest(async () =>
            {
                StatusMessage = $"Getting details for series {SelectedSeriesId}...";
                await EnsureAuthenticated();

                var result = await _apiService.TestGetSeriesById(SelectedSeriesId);
                TestResults.Add(result);

                StatusMessage = result.Success 
                    ? $"Series details: {result.Message}" 
                    : $"Failed: {result.Message}";
            });
        }

        [RelayCommand]
        private async Task SearchSeries()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                StatusMessage = "Please enter a search term";
                return;
            }

            await ExecuteTest(async () =>
            {
                StatusMessage = $"Searching for '{SearchTerm}'...";
                await EnsureAuthenticated();

                var result = await _apiService.TestSearchSeries(SearchTerm);
                TestResults.Add(result);

                StatusMessage = result.Success 
                    ? $"Search completed: {result.Message}" 
                    : $"Failed: {result.Message}";
            });
        }

        [RelayCommand]
        private void ViewSeriesMetadataWindow()
        {
            if (SelectedSeriesId <= 0)
            {
                StatusMessage = "Please enter a valid Series ID";
                return;
            }

            try
            {
                // Create and show the metadata viewer window
                var viewModel = new MetadataViewerViewModel(_apiService, SelectedSeriesId);
                var window = new Views.MetadataViewerWindow
                {
                    DataContext = viewModel
                };

                viewModel.SetCloseAction(() => window.Close());
                window.Show();

                StatusMessage = $"Opened metadata viewer for series {SelectedSeriesId}";
            }
            catch (System.Exception ex)
            {
                StatusMessage = $"Error opening metadata viewer: {ex.Message}";
            }
        }

        // Original test commands...

        [RelayCommand(CanExecute = nameof(CanExecuteTests))]
        private async Task RunAllTests()
        {
            await ExecuteTest(async () =>
            {
                if (string.IsNullOrWhiteSpace(ServerUrl) || 
                    string.IsNullOrWhiteSpace(Username) || 
                    string.IsNullOrWhiteSpace(Password))
                {
                    StatusMessage = "Please fill in all fields";
                    return;
                }

                StatusMessage = "Running tests...";
                TestResults.Clear();

                await _apiService.SetBaseUrl(ServerUrl);
                var results = await _apiService.RunAllTests(Username, Password);

                foreach (var result in results)
                {
                    TestResults.Add(result);
                }

                var successCount = 0;
                foreach (var result in results)
                {
                    if (result.Success) successCount++;
                }

                StatusMessage = $"Tests completed: {successCount}/{results.Count} passed";
            });
        }

        [RelayCommand(CanExecute = nameof(CanExecuteTests))]
        private async Task TestConnectivity()
        {
            await ExecuteTest(async () =>
            {
                if (string.IsNullOrWhiteSpace(ServerUrl))
                {
                    StatusMessage = "Please enter server URL";
                    return;
                }

                StatusMessage = "Testing connectivity...";
                await _apiService.SetBaseUrl(ServerUrl);
                var result = await _apiService.TestConnectivity();
                TestResults.Add(result);
                StatusMessage = result.Message;
            });
        }

        [RelayCommand(CanExecute = nameof(CanExecuteTests))]
        private async Task TestLogin()
        {
            await ExecuteTest(async () =>
            {
                if (string.IsNullOrWhiteSpace(ServerUrl) || 
                    string.IsNullOrWhiteSpace(Username) || 
                    string.IsNullOrWhiteSpace(Password))
                {
                    StatusMessage = "Please fill in all fields";
                    return;
                }

                StatusMessage = "Testing login...";
                await _apiService.SetBaseUrl(ServerUrl);
                var result = await _apiService.TestLogin(Username, Password);
                TestResults.Add(result);
                StatusMessage = result.Message;
            });
        }

        [RelayCommand(CanExecute = nameof(CanExecuteTests))]
        private async Task TestServerInfo()
        {
            await ExecuteTest(async () =>
            {
                if (string.IsNullOrWhiteSpace(ServerUrl))
                {
                    StatusMessage = "Please enter server URL";
                    return;
                }

                StatusMessage = "Getting server info...";
                await _apiService.SetBaseUrl(ServerUrl);
                var result = await _apiService.TestServerInfo();
                TestResults.Add(result);
                StatusMessage = result.Message;
            });
        }

        [RelayCommand(CanExecute = nameof(CanExecuteTests))]
        private async Task TestLibraries()
        {
            await ExecuteAuthenticatedTest(
                "Getting libraries...",
                async () => await _apiService.TestGetLibraries()
            );
        }

        [RelayCommand(CanExecute = nameof(CanExecuteTests))]
        private async Task TestRecentlyAdded()
        {
            await ExecuteAuthenticatedTest(
                "Getting recently added series...",
                async () => await _apiService.TestGetRecentlyAdded()
            );
        }

        [RelayCommand(CanExecute = nameof(CanExecuteTests))]
        private async Task TestOnDeck()
        {
            await ExecuteAuthenticatedTest(
                "Getting on-deck series...",
                async () => await _apiService.TestGetOnDeck()
            );
        }

        [RelayCommand(CanExecute = nameof(CanExecuteTests))]
        private async Task TestUsers()
        {
            await ExecuteAuthenticatedTest(
                "Getting users...",
                async () => await _apiService.TestGetUsers()
            );
        }

        [RelayCommand(CanExecute = nameof(CanExecuteTests))]
        private async Task TestCollections()
        {
            await ExecuteAuthenticatedTest(
                "Getting collections...",
                async () => await _apiService.TestGetCollections()
            );
        }

        [RelayCommand(CanExecute = nameof(CanExecuteTests))]
        private async Task TestReadingLists()
        {
            await ExecuteAuthenticatedTest(
                "Getting reading lists...",
                async () => await _apiService.TestGetReadingLists()
            );
        }

        [RelayCommand(CanExecute = nameof(CanExecuteTests))]
        private async Task TestServerStats()
        {
            await ExecuteAuthenticatedTest(
                "Getting server stats...",
                async () => await _apiService.TestGetServerStats()
            );
        }

        [RelayCommand(CanExecute = nameof(CanExecuteTests))]
        private async Task TestUserStats()
        {
            await ExecuteAuthenticatedTest(
                "Getting user stats...",
                async () => await _apiService.TestGetUserStats()
            );
        }

        [RelayCommand(CanExecute = nameof(CanExecuteTests))]
        private void ClearResults()
        {
            TestResults.Clear();
            StatusMessage = "Results cleared";
        }

        private bool CanExecuteTests()
        {
            return !IsRunning;
        }

        // Helper method to ensure we're authenticated
        private async Task EnsureAuthenticated()
        {
            await _apiService.SetBaseUrl(ServerUrl);
            
            if (!_apiService.IsAuthenticated)
            {
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    throw new System.InvalidOperationException("Please login first");
                }

                var loginResult = await _apiService.TestLogin(Username, Password);
                if (!loginResult.Success)
                {
                    throw new System.InvalidOperationException($"Login failed: {loginResult.Message}");
                }
            }
        }

        // Helper method for executing tests with proper locking
        private async Task ExecuteTest(System.Func<Task> testAction)
        {
            if (!await _executionLock.WaitAsync(0))
            {
                StatusMessage = "A test is already running. Please wait...";
                return;
            }

            try
            {
                IsRunning = true;
                await testAction();
            }
            catch (System.Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                TestResults.Add(new ApiTestResult
                {
                    TestName = "Error",
                    Success = false,
                    Message = ex.Message,
                    Details = ex.ToString(),
                    Duration = System.TimeSpan.Zero
                });
            }
            finally
            {
                IsRunning = false;
                _executionLock.Release();
            }
        }

        // Helper method for authenticated tests
        private async Task ExecuteAuthenticatedTest(string statusMessage, System.Func<Task<ApiTestResult>> testFunc)
        {
            await ExecuteTest(async () =>
            {
                if (string.IsNullOrWhiteSpace(ServerUrl) || 
                    string.IsNullOrWhiteSpace(Username) || 
                    string.IsNullOrWhiteSpace(Password))
                {
                    StatusMessage = "Please fill in all fields and login first";
                    return;
                }

                StatusMessage = statusMessage;
                await _apiService.SetBaseUrl(ServerUrl);
                
                // Ensure we're logged in
                if (!_apiService.IsAuthenticated)
                {
                    var loginResult = await _apiService.TestLogin(Username, Password);
                    if (!loginResult.Success)
                    {
                        TestResults.Add(loginResult);
                        StatusMessage = "Login failed. Please check credentials.";
                        return;
                    }
                }

                var result = await testFunc();
                TestResults.Add(result);
                StatusMessage = result.Message;
            });
        }
    }
}
