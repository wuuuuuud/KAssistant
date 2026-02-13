using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KAssistant.Models;
using KAssistant.Services;

namespace KAssistant.ViewModels
{
    public partial class SeriesBrowserViewModel : ViewModelBase
    {
        private readonly KavitaApiService _apiService;
        private Action? _closeAction;
        private List<Series> _allSeries = new();
        private Stopwatch _loadStopwatch = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShowEmptyState))]
        private bool _isLoading = true;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShowEmptyState))]
        private bool _hasError;

        [ObservableProperty]
        private string _errorMessage = "";

        [ObservableProperty]
        private string _statusMessage = "Initializing...";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FilteredCountText))]
        private string _searchText = "";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FilteredCountText))]
        private Library? _selectedLibrary;

        [ObservableProperty]
        private string _loadTimeText = "";

        public ObservableCollection<Library> Libraries { get; } = new();
        
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FilteredCountText))]
        [NotifyPropertyChangedFor(nameof(ShowEmptyState))]
        private ObservableCollection<Series> _filteredSeries = new();

        public string TotalSeriesText => $"({_allSeries.Count} total)";
        
        public string FilteredCountText => FilteredSeries.Count != _allSeries.Count 
            ? $"Showing {FilteredSeries.Count} of {_allSeries.Count}" 
            : "";

        public bool ShowEmptyState => !IsLoading && !HasError && FilteredSeries.Count == 0;

        public SeriesBrowserViewModel()
        {
            // Design-time constructor
            _apiService = new KavitaApiService();
        }

        public SeriesBrowserViewModel(KavitaApiService apiService)
        {
            _apiService = apiService;
            
            // Load data on initialization
            _ = LoadDataAsync();
        }

        public void SetCloseAction(Action closeAction)
        {
            _closeAction = closeAction;
        }

        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            
            if (e.PropertyName == nameof(SearchText) || e.PropertyName == nameof(SelectedLibrary))
            {
                ApplyFilter();
            }
        }

        private async Task LoadDataAsync()
        {
            _loadStopwatch = Stopwatch.StartNew();
            
            try
            {
                IsLoading = true;
                HasError = false;
                ErrorMessage = "";
                StatusMessage = "Loading libraries...";

                // Load libraries first
                var libraries = await _apiService.GetLibrariesAsync().ConfigureAwait(false);
                
                if (libraries == null || libraries.Count == 0)
                {
                    HasError = true;
                    ErrorMessage = "No libraries found or failed to load libraries.\n\nPlease ensure you are logged in and have at least one library configured.";
                    StatusMessage = "No libraries found";
                    _loadStopwatch.Stop();
                    return;
                }

                // Add "All Libraries" option
                Libraries.Clear();
                Libraries.Add(new Library { Id = 0, Name = "All Libraries", Type = 0 });
                foreach (var lib in libraries.OrderBy(l => l.Name))
                {
                    Libraries.Add(lib);
                }

                // Select "All Libraries" by default
                SelectedLibrary = Libraries.First();

                StatusMessage = $"Loading series from {libraries.Count} libraries...";

                // Load all series
                var allSeriesResult = await _apiService.GetAllSeriesAsync(0, 1000).ConfigureAwait(false);
                
                if (allSeriesResult == null || allSeriesResult.Result.Count == 0)
                {
                    _allSeries.Clear();
                    ApplyFilter();
                    StatusMessage = "No series found in any library";
                    _loadStopwatch.Stop();
                    LoadTimeText = $"Loaded in {_loadStopwatch.Elapsed.TotalSeconds:F1}s";
                    return;
                }

                _allSeries = allSeriesResult.Result;
                ApplyFilter();

                _loadStopwatch.Stop();
                LoadTimeText = $"Loaded in {_loadStopwatch.Elapsed.TotalSeconds:F1}s";
                StatusMessage = $"Loaded {_allSeries.Count} series successfully";

                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(TotalSeriesText)));
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Failed to load series: {ex.Message}\n\n{ex}";
                StatusMessage = "Error occurred";
                _loadStopwatch.Stop();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApplyFilter()
        {
            FilteredSeries.Clear();

            var filtered = _allSeries.AsEnumerable();

            // Filter by library
            if (SelectedLibrary != null && SelectedLibrary.Id > 0)
            {
                filtered = filtered.Where(s => s.LibraryId == SelectedLibrary.Id);
            }

            // Filter by search text
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchLower = SearchText.ToLower();
                filtered = filtered.Where(s => 
                    (s.Name?.ToLower().Contains(searchLower) ?? false) ||
                    (s.OriginalName?.ToLower().Contains(searchLower) ?? false) ||
                    (s.Summary?.ToLower().Contains(searchLower) ?? false));
            }

            foreach (var series in filtered.OrderBy(s => s.Name))
            {
                FilteredSeries.Add(series);
            }
            
            StatusMessage = FilteredSeries.Count == _allSeries.Count 
                ? $"Showing all {FilteredSeries.Count} series" 
                : $"Filtered to {FilteredSeries.Count} of {_allSeries.Count} series";
        }

        [RelayCommand]
        private async Task Refresh()
        {
            await LoadDataAsync();
        }

        [RelayCommand]
        private void ClearFilter()
        {
            SearchText = "";
            SelectedLibrary = Libraries.FirstOrDefault();
        }

        [RelayCommand]
        private void ViewSeries(Series series)
        {
            if (series == null) return;

            try
            {
                var viewModel = new MetadataViewerViewModel(_apiService, series.Id);
                var window = new Views.MetadataViewerWindow
                {
                    DataContext = viewModel
                };

                viewModel.SetCloseAction(() => window.Close());
                window.Show();

                StatusMessage = $"Opened metadata viewer for '{series.Name}'";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error opening viewer: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task CopySeriesId(int seriesId)
        {
            try
            {
                // In a real app, we'd use Avalonia clipboard
                // For now, just show the ID
                StatusMessage = $"Series ID: {seriesId} (ready to use)";
                
                // Give feedback
                await Task.Delay(2000);
                StatusMessage = $"Showing {FilteredSeries.Count} series";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        [RelayCommand]
        private void CloseWindow()
        {
            _closeAction?.Invoke();
        }
    }
}
