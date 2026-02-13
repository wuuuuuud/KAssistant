using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using KAssistant.Models;

namespace KAssistant.Services
{
    /// <summary>
    /// Backward-compatible wrapper around OpenApiKavitaService
    /// This maintains the existing KavitaApiService interface and adds test methods
    /// </summary>
    public class KavitaApiService : IDisposable
    {
        private readonly OpenApiKavitaService _apiService;
        private bool _isAuthenticated;

        public KavitaApiService()
        {
            _apiService = new OpenApiKavitaService();
        }

        public bool IsAuthenticated => _isAuthenticated;

        public Task SetBaseUrl(string baseUrl)
        {
            return _apiService.SetBaseUrl(baseUrl);
        }

        public void SetAuthToken(string token)
        {
            _apiService.SetAuthToken(token);
            _isAuthenticated = !string.IsNullOrEmpty(token);
        }

        public void ClearAuthToken()
        {
            _apiService.ClearAuthToken();
            _isAuthenticated = false;
        }

        #region Test Methods

        private async Task<ApiTestResult> ExecuteTest(string testName, Func<Task<string>> action)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var message = await action();
                sw.Stop();
                return new ApiTestResult
                {
                    TestName = testName,
                    Success = true,
                    Message = message,
                    Duration = sw.Elapsed
                };
            }
            catch (Exception ex)
            {
                sw.Stop();
                return new ApiTestResult
                {
                    TestName = testName,
                    Success = false,
                    Message = ex.Message,
                    Details = ex.ToString(),
                    Duration = sw.Elapsed
                };
            }
        }

        public Task<ApiTestResult> TestConnectivity()
        {
            return ExecuteTest("Connectivity Test", async () =>
            {
                var serverInfo = await _apiService.GetServerInfoAsync();
                return serverInfo != null
                    ? $"Connected. Kavita version: {serverInfo.KavitaVersion}"
                    : "Connected but no server info returned";
            });
        }

        public Task<ApiTestResult> TestLogin(string username, string password)
        {
            return ExecuteTest("Login Test", async () =>
            {
                var user = await _apiService.LoginAsync(username, password);
                if (user?.Token != null)
                {
                    _isAuthenticated = true;
                    return $"Logged in as {user.Username}";
                }
                throw new Exception("Login failed - no token returned");
            });
        }

        public Task<ApiTestResult> TestServerInfo()
        {
            return ExecuteTest("Server Info Test", async () =>
            {
                var serverInfo = await _apiService.GetServerInfoAsync();
                return serverInfo != null
                    ? $"Kavita {serverInfo.KavitaVersion} on {serverInfo.OS}"
                    : "No server info returned";
            });
        }

        public Task<ApiTestResult> TestGetLibraries()
        {
            return ExecuteTest("Get Libraries Test", async () =>
            {
                var libraries = await _apiService.GetLibrariesAsync();
                return libraries != null
                    ? $"Found {libraries.Count} libraries"
                    : "No libraries returned";
            });
        }

        public Task<ApiTestResult> TestGetRecentlyAdded()
        {
            return ExecuteTest("Get Recently Added Test", async () =>
            {
                var result = await _apiService.GetRecentlyAddedSeriesAsync(0, 20);
                return result != null
                    ? $"Found {result.TotalCount} recently added series"
                    : "No recently added series returned";
            });
        }

        public Task<ApiTestResult> TestGetOnDeck()
        {
            return ExecuteTest("Get On Deck Test", async () =>
            {
                var result = await _apiService.GetOnDeckSeriesAsync(0, 20);
                return result != null
                    ? $"Found {result.TotalCount} on deck items"
                    : "No on deck items returned";
            });
        }

        public Task<ApiTestResult> TestGetUsers()
        {
            return ExecuteTest("Get Users Test", async () =>
            {
                var users = await _apiService.GetUsersAsync();
                return users != null
                    ? $"Found {users.Count} users"
                    : "No users returned";
            });
        }

        public Task<ApiTestResult> TestGetCollections()
        {
            return ExecuteTest("Get Collections Test", async () =>
            {
                var collections = await _apiService.GetCollectionsAsync();
                return collections != null
                    ? $"Found {collections.Count} collections"
                    : "No collections returned";
            });
        }

        public Task<ApiTestResult> TestGetReadingLists()
        {
            return ExecuteTest("Get Reading Lists Test", async () =>
            {
                var readingLists = await _apiService.GetReadingListsAsync();
                return readingLists != null
                    ? $"Found {readingLists.TotalCount} reading lists"
                    : "No reading lists returned";
            });
        }

        public Task<ApiTestResult> TestGetServerStats()
        {
            return ExecuteTest("Get Server Stats Test", async () =>
            {
                var stats = await _apiService.GetServerStatsAsync();
                return stats != null
                    ? $"Total Series: {stats.TotalSeries}, Total Files: {stats.TotalFiles}"
                    : "No server stats returned";
            });
        }

        public Task<ApiTestResult> TestGetUserStats()
        {
            return ExecuteTest("Get User Stats Test", async () =>
            {
                var stats = await _apiService.GetUserStatsAsync();
                return stats != null
                    ? $"Pages Read: {stats.TotalPagesRead}, Chapters Read: {stats.ChaptersRead}"
                    : "No user stats returned";
            });
        }

        public Task<ApiTestResult> TestGetSeriesById(int seriesId)
        {
            return ExecuteTest($"Get Series {seriesId} Test", async () =>
            {
                var series = await _apiService.GetSeriesAsync(seriesId);
                return series != null
                    ? $"Series: {series.Name}"
                    : "No series returned";
            });
        }

        public Task<ApiTestResult> TestGetSeriesMetadata(int seriesId)
        {
            return ExecuteTest($"Get Series Metadata {seriesId} Test", async () =>
            {
                var metadata = await _apiService.GetSeriesMetadataAsync(seriesId);
                return metadata != null
                    ? $"Summary length: {metadata.Summary?.Length ?? 0}, Genres: {metadata.Genres?.Count ?? 0}"
                    : "No metadata returned";
            });
        }

        public Task<ApiTestResult> TestGetSeriesForLibrary(int libraryId)
        {
            return ExecuteTest($"Get Series for Library {libraryId} Test", async () =>
            {
                var result = await _apiService.GetAllSeriesAsync(libraryId, 0, 20);
                return result != null
                    ? $"Found {result.TotalCount} series in library"
                    : "No series returned";
            });
        }

        public Task<ApiTestResult> TestSearchSeries(string query)
        {
            return ExecuteTest($"Search Series '{query}' Test", async () =>
            {
                var result = await _apiService.SearchAsync(query);
                var seriesCount = result?.Series?.Count ?? 0;
                return $"Found {seriesCount} series matching '{query}'";
            });
        }

        public async Task<List<ApiTestResult>> RunAllTests(string username, string password)
        {
            var results = new List<ApiTestResult>();

            results.Add(await TestConnectivity());

            var loginResult = await TestLogin(username, password);
            results.Add(loginResult);

            if (!loginResult.Success)
            {
                return results;
            }

            results.Add(await TestServerInfo());
            results.Add(await TestGetLibraries());
            results.Add(await TestGetRecentlyAdded());
            results.Add(await TestGetOnDeck());
            results.Add(await TestGetCollections());
            results.Add(await TestGetReadingLists());
            results.Add(await TestGetServerStats());
            results.Add(await TestGetUserStats());

            try
            {
                results.Add(await TestGetUsers());
            }
            catch
            {
                results.Add(new ApiTestResult
                {
                    TestName = "Get Users Test",
                    Success = false,
                    Message = "User may not have admin rights"
                });
            }

            return results;
        }

        #endregion

        #region Authentication

        public async Task<UserDto?> LoginAsync(string username, string password)
        {
            var result = await _apiService.LoginAsync(username, password);
            if (result?.Token != null)
            {
                _isAuthenticated = true;
            }
            return result;
        }

        public Task<UserDto?> GetCurrentUserAsync()
        {
            return _apiService.GetCurrentUserAsync();
        }

        public Task<List<string>?> GetRolesAsync()
        {
            return _apiService.GetRolesAsync();
        }

        #endregion

        #region Server

        public Task<ServerInfoResponse?> GetServerInfoAsync()
        {
            return _apiService.GetServerInfoAsync();
        }

        public Task<List<JobDto>?> GetJobsAsync()
        {
            return _apiService.GetJobsAsync();
        }

        public Task<UpdateNotificationDto?> CheckForUpdatesAsync()
        {
            return _apiService.CheckForUpdatesAsync();
        }

        public Task ClearCacheAsync()
        {
            return _apiService.ClearCacheAsync();
        }

        public Task BackupDatabaseAsync()
        {
            return _apiService.BackupDatabaseAsync();
        }

        public Task<string?> HealthCheckAsync()
        {
            return _apiService.HealthCheckAsync();
        }

        #endregion

        #region Libraries

        public Task<List<Library>?> GetLibrariesAsync()
        {
            return _apiService.GetLibrariesAsync();
        }

        public Task<Library?> GetLibraryAsync(int libraryId)
        {
            return _apiService.GetLibraryAsync(libraryId);
        }

        public Task<int> GetLibraryTypeAsync(int libraryId)
        {
            return _apiService.GetLibraryTypeAsync(libraryId);
        }

        public Task ScanLibraryAsync(int libraryId, bool force = false)
        {
            return _apiService.ScanLibraryAsync(libraryId, force);
        }

        public Task ScanAllLibrariesAsync(bool force = false)
        {
            return _apiService.ScanAllLibrariesAsync(force);
        }

        public Task RefreshLibraryMetadataAsync(int libraryId, bool force = true, bool forceColorscape = true)
        {
            return _apiService.RefreshLibraryMetadataAsync(libraryId, force, forceColorscape);
        }

        public Task<bool> LibraryNameExistsAsync(string name)
        {
            return _apiService.LibraryNameExistsAsync(name);
        }

        #endregion

        #region Series

        public Task<Series?> GetSeriesAsync(int seriesId)
        {
            return _apiService.GetSeriesAsync(seriesId);
        }

        public Task<SeriesDetailDto?> GetSeriesDetailAsync(int seriesId)
        {
            return _apiService.GetSeriesDetailAsync(seriesId);
        }

        public Task<SeriesMetadata?> GetSeriesMetadataAsync(int seriesId)
        {
            return _apiService.GetSeriesMetadataAsync(seriesId);
        }

        public Task UpdateSeriesMetadataAsync(UpdateSeriesMetadataDto metadata)
        {
            return _apiService.UpdateSeriesMetadataAsync(metadata);
        }

        public Task<List<VolumeDto>?> GetVolumesAsync(int seriesId)
        {
            return _apiService.GetVolumesAsync(seriesId);
        }

        public Task<VolumeDto?> GetVolumeAsync(int volumeId)
        {
            return _apiService.GetVolumeAsync(volumeId);
        }

        public Task<ChapterDto?> GetChapterAsync(int chapterId)
        {
            return _apiService.GetChapterAsync(chapterId);
        }

        public Task<PaginatedResult<Series>?> GetSeriesV2Async(FilterV2Dto filter, int pageNumber = 0, int pageSize = 20)
        {
            return _apiService.GetSeriesV2Async(filter, pageNumber, pageSize);
        }

        public Task<PaginatedResult<Series>?> GetRecentlyAddedSeriesAsync(
            int pageNumber = 0, 
            int pageSize = 20, 
            int? libraryId = null)
        {
            return _apiService.GetRecentlyAddedSeriesAsync(pageNumber, pageSize, libraryId);
        }

        public Task<PaginatedResult<Series>?> GetOnDeckSeriesAsync(
            int pageNumber = 0, 
            int pageSize = 20, 
            int? libraryId = null)
        {
            return _apiService.GetOnDeckSeriesAsync(pageNumber, pageSize, libraryId);
        }

        public Task<PaginatedResult<Series>?> GetAllSeriesAsync(
            int libraryId, 
            int pageNumber = 0, 
            int pageSize = 20)
        {
            return _apiService.GetAllSeriesAsync(libraryId, pageNumber, pageSize);
        }

        public Task RefreshSeriesMetadataAsync(int libraryId, int seriesId, bool forceUpdate = true, bool forceColorscape = false)
        {
            return _apiService.RefreshSeriesMetadataAsync(libraryId, seriesId, forceUpdate, forceColorscape);
        }

        public Task ScanSeriesAsync(int libraryId, int seriesId)
        {
            return _apiService.ScanSeriesAsync(libraryId, seriesId);
        }

        public Task<bool> DeleteSeriesAsync(int seriesId)
        {
            return _apiService.DeleteSeriesAsync(seriesId);
        }

        public Task<List<Series>?> GetRelatedSeriesAsync(int seriesId, int relation)
        {
            return _apiService.GetRelatedSeriesAsync(seriesId, relation);
        }

        public Task<RelatedSeriesDto?> GetAllRelatedSeriesAsync(int seriesId)
        {
            return _apiService.GetAllRelatedSeriesAsync(seriesId);
        }

        #endregion

        #region Search

        public Task<SearchResultGroupDto?> SearchAsync(string queryString, bool includeChapterAndFiles = true)
        {
            return _apiService.SearchAsync(queryString, includeChapterAndFiles);
        }

        public Task<Series?> GetSeriesForChapterAsync(int chapterId)
        {
            return _apiService.GetSeriesForChapterAsync(chapterId);
        }

        #endregion

        #region Reader

        public Task<ChapterInfoDto?> GetChapterInfoAsync(int chapterId, bool extractPdf = false, bool includeDimensions = false)
        {
            return _apiService.GetChapterInfoAsync(chapterId, extractPdf, includeDimensions);
        }

        public Task<ProgressDto?> GetProgressAsync(int chapterId)
        {
            return _apiService.GetProgressAsync(chapterId);
        }

        public Task SaveProgressAsync(ProgressDto progress)
        {
            return _apiService.SaveProgressAsync(progress);
        }

        public Task<ChapterDto?> GetContinuePointAsync(int seriesId)
        {
            return _apiService.GetContinuePointAsync(seriesId);
        }

        public Task<bool> HasProgressAsync(int seriesId)
        {
            return _apiService.HasProgressAsync(seriesId);
        }

        public Task MarkSeriesAsReadAsync(int seriesId)
        {
            return _apiService.MarkSeriesAsReadAsync(seriesId);
        }

        public Task MarkSeriesAsUnreadAsync(int seriesId)
        {
            return _apiService.MarkSeriesAsUnreadAsync(seriesId);
        }

        public Task<int> GetNextChapterAsync(int seriesId, int volumeId, int currentChapterId)
        {
            return _apiService.GetNextChapterAsync(seriesId, volumeId, currentChapterId);
        }

        public Task<int> GetPrevChapterAsync(int seriesId, int volumeId, int currentChapterId)
        {
            return _apiService.GetPrevChapterAsync(seriesId, volumeId, currentChapterId);
        }

        public Task<HourEstimateRangeDto?> GetTimeLeftAsync(int seriesId)
        {
            return _apiService.GetTimeLeftAsync(seriesId);
        }

        #endregion

        #region Stats

        public Task<ServerStats?> GetServerStatsAsync()
        {
            return _apiService.GetServerStatsAsync();
        }

        public Task<UserStats?> GetUserStatsAsync(int? userId = null)
        {
            return _apiService.GetUserStatsAsync(userId);
        }

        #endregion

        #region Collections

        public Task<List<Collection>?> GetCollectionsAsync(bool ownedOnly = false)
        {
            return _apiService.GetCollectionsAsync(ownedOnly);
        }

        public Task<List<Collection>?> GetCollectionAsync(int collectionId)
        {
            return _apiService.GetCollectionAsync(collectionId);
        }

        public Task<List<Collection>?> GetCollectionsForSeriesAsync(int seriesId, bool ownedOnly = false)
        {
            return _apiService.GetCollectionsForSeriesAsync(seriesId, ownedOnly);
        }

        public Task DeleteCollectionAsync(int tagId)
        {
            return _apiService.DeleteCollectionAsync(tagId);
        }

        #endregion

        #region Reading Lists

        public Task<ReadingListDto?> GetReadingListAsync(int readingListId)
        {
            return _apiService.GetReadingListAsync(readingListId);
        }

        public Task<PaginatedResult<ReadingListDto>?> GetReadingListsAsync(int pageNumber = 0, int pageSize = 20, bool includePromoted = true)
        {
            return _apiService.GetReadingListsAsync(pageNumber, pageSize, includePromoted);
        }

        public Task<List<ReadingListItemDto>?> GetReadingListItemsAsync(int readingListId)
        {
            return _apiService.GetReadingListItemsAsync(readingListId);
        }

        public Task DeleteReadingListAsync(int readingListId)
        {
            return _apiService.DeleteReadingListAsync(readingListId);
        }

        #endregion

        #region Users

        public Task<List<User>?> GetUsersAsync(bool includePending = false)
        {
            return _apiService.GetUsersAsync(includePending);
        }

        public Task<List<string>?> GetUserNamesAsync()
        {
            return _apiService.GetUserNamesAsync();
        }

        public Task<UserPreferencesDto?> GetUserPreferencesAsync()
        {
            return _apiService.GetUserPreferencesAsync();
        }

        #endregion

        #region Metadata

        public Task<List<GenreTagDto>?> GetGenresAsync(string? libraryIds = null)
        {
            return _apiService.GetGenresAsync(libraryIds);
        }

        public Task<List<TagDto>?> GetTagsAsync(string? libraryIds = null)
        {
            return _apiService.GetTagsAsync(libraryIds);
        }

        public Task<List<AgeRatingDto>?> GetAgeRatingsAsync(string? libraryIds = null)
        {
            return _apiService.GetAgeRatingsAsync(libraryIds);
        }

        public Task<List<AgeRatingDto>?> GetPublicationStatusAsync(string? libraryIds = null)
        {
            return _apiService.GetPublicationStatusAsync(libraryIds);
        }

        public Task<List<LanguageDto>?> GetLanguagesAsync(string? libraryIds = null)
        {
            return _apiService.GetLanguagesAsync(libraryIds);
        }

        public Task<List<PersonDto>?> GetPeopleAsync(string? libraryIds = null)
        {
            return _apiService.GetPeopleAsync(libraryIds);
        }

        #endregion

        #region Want To Read

        public Task<bool> IsInWantToReadAsync(int seriesId)
        {
            return _apiService.IsInWantToReadAsync(seriesId);
        }

        public Task AddToWantToReadAsync(params int[] seriesIds)
        {
            return _apiService.AddToWantToReadAsync(seriesIds);
        }

        public Task RemoveFromWantToReadAsync(params int[] seriesIds)
        {
            return _apiService.RemoveFromWantToReadAsync(seriesIds);
        }

        #endregion

        #region Image URLs

        public string GetSeriesCoverUrl(int seriesId, string? apiKey = null)
        {
            return _apiService.GetSeriesCoverUrl(seriesId, apiKey);
        }

        public string GetVolumeCoverUrl(int volumeId, string? apiKey = null)
        {
            return _apiService.GetVolumeCoverUrl(volumeId, apiKey);
        }

        public string GetChapterCoverUrl(int chapterId, string? apiKey = null)
        {
            return _apiService.GetChapterCoverUrl(chapterId, apiKey);
        }

        public string GetLibraryCoverUrl(int libraryId, string? apiKey = null)
        {
            return _apiService.GetLibraryCoverUrl(libraryId, apiKey);
        }

        #endregion

        public void Dispose()
        {
            _apiService.Dispose();
        }
    }
}
