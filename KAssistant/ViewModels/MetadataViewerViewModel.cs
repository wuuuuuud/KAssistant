using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KAssistant.Models;
using KAssistant.Services;

namespace KAssistant.ViewModels
{
    public partial class MetadataViewerViewModel : ViewModelBase
    {
        private readonly KavitaApiService _apiService;
        private readonly int _targetSeriesId;
        private Action? _closeAction;

        [ObservableProperty]
        private string _seriesName = "Loading...";

        [ObservableProperty]
        private string _originalName = "";

        [ObservableProperty]
        private int _seriesId;

        [ObservableProperty]
        private int _libraryId;

        [ObservableProperty]
        private int _pagesRead;

        [ObservableProperty]
        private string _summary = "";

        [ObservableProperty]
        private string _ageRating = "";

        [ObservableProperty]
        private string _publicationStatus = "";

        [ObservableProperty]
        private string _language = "";

        [ObservableProperty]
        private string _created = "";

        [ObservableProperty]
        private string _lastModified = "";

        [ObservableProperty]
        private string _coverImageLocked = "";

        [ObservableProperty]
        private string _statusMessage = "Loading metadata...";

        [ObservableProperty]
        private string _errorMessage = "";

        [ObservableProperty]
        private bool _isLoading = true;

        [ObservableProperty]
        private bool _hasError;

        public ObservableCollection<string> Genres { get; } = new();
        public ObservableCollection<string> Tags { get; } = new();

        public bool HasOriginalName => !string.IsNullOrWhiteSpace(OriginalName) && OriginalName != SeriesName;
        public bool HasSummary => !string.IsNullOrWhiteSpace(Summary);
        public bool HasAgeRating => !string.IsNullOrWhiteSpace(AgeRating);
        public bool HasPublicationStatus => !string.IsNullOrWhiteSpace(PublicationStatus);
        public bool HasLanguage => !string.IsNullOrWhiteSpace(Language);
        public bool HasGenres => Genres.Count > 0;
        public bool HasTags => Tags.Count > 0;

        public MetadataViewerViewModel()
        {
            // Design-time constructor
            _apiService = new KavitaApiService();
            _targetSeriesId = 0;
        }

        public MetadataViewerViewModel(KavitaApiService apiService, int seriesId)
        {
            _apiService = apiService;
            _targetSeriesId = seriesId;
            SeriesId = seriesId;

            // Load data on initialization
            _ = LoadDataAsync();
        }

        public void SetCloseAction(Action closeAction)
        {
            _closeAction = closeAction;
        }

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                HasError = false;
                ErrorMessage = "";
                StatusMessage = "Loading metadata...";

                // Get series details first
                var detailsResult = await _apiService.TestGetSeriesById(_targetSeriesId);
                if (detailsResult.Success)
                {
                    // Parse series details from the result
                    StatusMessage = "Loading series details...";
                    ParseSeriesDetailsFromTestResult(detailsResult);
                }

                // Get metadata
                var metadataResult = await _apiService.TestGetSeriesMetadata(_targetSeriesId);
                if (metadataResult.Success)
                {
                    StatusMessage = "Metadata loaded successfully";
                    // Parse metadata from result
                    ParseMetadataFromTestResult(metadataResult);
                }
                else
                {
                    HasError = true;
                    ErrorMessage = $"{metadataResult.Message}\n\n{metadataResult.Details}";
                    StatusMessage = "Failed to load metadata";
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error loading metadata: {ex.Message}";
                StatusMessage = "Error occurred";
            }
            finally
            {
                IsLoading = false;
                OnPropertyChanged(nameof(HasOriginalName));
                OnPropertyChanged(nameof(HasSummary));
                OnPropertyChanged(nameof(HasAgeRating));
                OnPropertyChanged(nameof(HasPublicationStatus));
                OnPropertyChanged(nameof(HasLanguage));
                OnPropertyChanged(nameof(HasGenres));
                OnPropertyChanged(nameof(HasTags));
            }
        }

        private void ParseSeriesDetailsFromTestResult(ApiTestResult result)
        {
            // Parse the Details string to extract series information
            var details = result.Details ?? "";
            var lines = details.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                
                if (trimmed.StartsWith("Name:"))
                {
                    SeriesName = trimmed.Substring("Name:".Length).Trim();
                }
                else if (trimmed.StartsWith("Original Name:"))
                {
                    OriginalName = trimmed.Substring("Original Name:".Length).Trim();
                }
                else if (trimmed.StartsWith("Library ID:"))
                {
                    if (int.TryParse(trimmed.Substring("Library ID:".Length).Trim(), out var libId))
                    {
                        LibraryId = libId;
                    }
                }
                else if (trimmed.StartsWith("Pages Read:"))
                {
                    if (int.TryParse(trimmed.Substring("Pages Read:".Length).Trim(), out var pages))
                    {
                        PagesRead = pages;
                    }
                }
                else if (trimmed.StartsWith("Created:"))
                {
                    Created = trimmed.Substring("Created:".Length).Trim();
                }
                else if (trimmed.StartsWith("Last Modified:"))
                {
                    LastModified = trimmed.Substring("Last Modified:".Length).Trim();
                }
            }
        }

        private void ParseMetadataFromTestResult(ApiTestResult result)
        {
            // Parse the Details string to extract metadata
            var details = result.Details ?? "";
            var lines = details.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            bool inGenresSection = false;
            bool inTagsSection = false;

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                
                if (trimmed.StartsWith("Summary:"))
                {
                    Summary = trimmed.Substring("Summary:".Length).Trim();
                    inGenresSection = false;
                    inTagsSection = false;
                }
                else if (trimmed.StartsWith("Age Rating:"))
                {
                    AgeRating = trimmed.Substring("Age Rating:".Length).Trim();
                    inGenresSection = false;
                    inTagsSection = false;
                }
                else if (trimmed.StartsWith("Publication Status:"))
                {
                    PublicationStatus = trimmed.Substring("Publication Status:".Length).Trim();
                    inGenresSection = false;
                    inTagsSection = false;
                }
                else if (trimmed.StartsWith("Language:"))
                {
                    Language = trimmed.Substring("Language:".Length).Trim();
                    inGenresSection = false;
                    inTagsSection = false;
                }
                else if (trimmed.StartsWith("Genres ("))
                {
                    inGenresSection = true;
                    inTagsSection = false;
                }
                else if (trimmed.StartsWith("Tags ("))
                {
                    inGenresSection = false;
                    inTagsSection = true;
                }
                else if (trimmed.StartsWith("- "))
                {
                    var item = trimmed.Substring(2).Trim();
                    if (inGenresSection)
                    {
                        Genres.Add(item);
                    }
                    else if (inTagsSection)
                    {
                        Tags.Add(item);
                    }
                }
            }

            // Set cover lock status
            CoverImageLocked = "No"; // Default value
        }

        [RelayCommand]
        private async Task RefreshMetadata()
        {
            Genres.Clear();
            Tags.Clear();
            await LoadDataAsync();
        }

        [RelayCommand]
        private void EditMetadata()
        {
            // TODO: Implement metadata editing
            StatusMessage = "Metadata editing coming soon...";
        }

        [RelayCommand]
        private void CloseWindow()
        {
            _closeAction?.Invoke();
        }
    }
}
