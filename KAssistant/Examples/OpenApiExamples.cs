// OpenAPI Examples for Kavita API
// This file demonstrates how to use the KavitaApiService to communicate with Kavita

using System;
using System.Linq;
using System.Threading.Tasks;
using KAssistant.Models;
using KAssistant.Services;

namespace KAssistant.Examples
{
    /// <summary>
    /// Examples of using the Kavita API based on the OpenAPI specification.
    /// The OpenAPI document (kavita_api.json) defines all available endpoints.
    /// </summary>
    public static class OpenApiExamples
    {
        /// <summary>
        /// Basic connection and authentication example
        /// </summary>
        public static async Task AuthenticationExample()
        {
            using var api = new KavitaApiService();
            
            // Set the base URL (your Kavita server)
            await api.SetBaseUrl("http://localhost:5000");
            
            // Login - POST /api/Account/login
            var user = await api.LoginAsync("admin", "password");
            
            if (user?.Token != null)
            {
                Console.WriteLine($"Logged in as: {user.Username}");
                Console.WriteLine($"API Key: {user.ApiKey}");
            }
            
            // Get current user - GET /api/Account
            var currentUser = await api.GetCurrentUserAsync();
            Console.WriteLine($"Current user: {currentUser?.Username}");
            
            // Get available roles - GET /api/Account/roles
            var roles = await api.GetRolesAsync();
            Console.WriteLine($"Available roles: {string.Join(", ", roles ?? [])}");
        }

        /// <summary>
        /// Server information and health check example
        /// </summary>
        public static async Task ServerInfoExample()
        {
            using var api = new KavitaApiService();
            await api.SetBaseUrl("http://localhost:5000");
            
            // Health check - GET /api/Health
            var health = await api.HealthCheckAsync();
            Console.WriteLine($"Server health: {health}");
            
            // Server info - GET /api/Server/server-info-slim
            var info = await api.GetServerInfoAsync();
            Console.WriteLine($"Kavita Version: {info?.KavitaVersion}");
            Console.WriteLine($"OS: {info?.OS}");
            Console.WriteLine($".NET Version: {info?.DotNetVersion}");
            
            // Check for updates - GET /api/Server/check-update
            var update = await api.CheckForUpdatesAsync();
            if (update?.IsReleaseNewer == true)
            {
                Console.WriteLine($"New version available: {update.UpdateVersion}");
            }
            
            // Get scheduled jobs - GET /api/Server/jobs
            var jobs = await api.GetJobsAsync();
            foreach (var job in jobs ?? [])
            {
                Console.WriteLine($"Job: {job.Title} - Last run: {job.LastExecution}");
            }
        }

        /// <summary>
        /// Library management example
        /// </summary>
        public static async Task LibraryExample()
        {
            using var api = new KavitaApiService();
            await api.SetBaseUrl("http://localhost:5000");
            await api.LoginAsync("admin", "password");
            
            // Get all libraries - GET /api/Library/libraries
            var libraries = await api.GetLibrariesAsync();
            
            foreach (var library in libraries ?? [])
            {
                Console.WriteLine($"Library: {library.Name} (ID: {library.Id})");
                Console.WriteLine($"  Type: {library.Type}");
                Console.WriteLine($"  Last Scanned: {library.LastScanned}");
                Console.WriteLine($"  Folder Watching: {library.FolderWatching}");
            }
            
            // Get specific library - GET /api/Library?libraryId=1
            var lib = await api.GetLibraryAsync(1);
            Console.WriteLine($"Specific library: {lib?.Name}");
            
            // Get library type - GET /api/Library/type?libraryId=1
            var libType = await api.GetLibraryTypeAsync(1);
            Console.WriteLine($"Library type enum: {libType}");
            
            // Scan library - POST /api/Library/scan
            await api.ScanLibraryAsync(1, force: false);
            Console.WriteLine("Library scan started");
            
            // Refresh metadata - POST /api/Library/refresh-metadata
            await api.RefreshLibraryMetadataAsync(1, force: true, forceColorscape: true);
            Console.WriteLine("Metadata refresh started");
        }

        /// <summary>
        /// Series browsing and filtering example
        /// </summary>
        public static async Task SeriesExample()
        {
            using var api = new KavitaApiService();
            await api.SetBaseUrl("http://localhost:5000");
            await api.LoginAsync("admin", "password");
            
            // Get recently added series - POST /api/Series/recently-added-v2
            var recentlyAdded = await api.GetRecentlyAddedSeriesAsync(0, 10);
            Console.WriteLine($"Recently added: {recentlyAdded?.TotalCount} series");
            
            foreach (var series in recentlyAdded?.Result ?? [])
            {
                Console.WriteLine($"  - {series.Name} ({series.Pages} pages)");
            }
            
            // Get on deck series - POST /api/Series/on-deck
            var onDeck = await api.GetOnDeckSeriesAsync(0, 10);
            Console.WriteLine($"On deck: {onDeck?.TotalCount} series");
            
            // Get all series from a library - POST /api/Series/all-v2
            var allSeries = await api.GetAllSeriesAsync(1, 0, 20);
            Console.WriteLine($"Total series in library: {allSeries?.TotalCount}");
            
            // Use advanced filtering - POST /api/Series/v2
            var filter = new FilterV2Dto
            {
                Statements = new()
                {
                    new FilterStatementDto { Field = 1, Comparison = 4, Value = "manga" }
                },
                SortOptions = new SortOptionsDto { SortField = 1, IsAscending = true }
            };
            var filtered = await api.GetSeriesV2Async(filter, 0, 20);
            Console.WriteLine($"Filtered series: {filtered?.TotalCount}");
        }

        /// <summary>
        /// Series detail and metadata example
        /// </summary>
        public static async Task SeriesDetailExample()
        {
            using var api = new KavitaApiService();
            await api.SetBaseUrl("http://localhost:5000");
            await api.LoginAsync("admin", "password");
            
            int seriesId = 1;
            
            // Get series - GET /api/Series/{seriesId}
            var series = await api.GetSeriesAsync(seriesId);
            Console.WriteLine($"Series: {series?.Name}");
            Console.WriteLine($"  Summary: {series?.Summary}");
            Console.WriteLine($"  Pages: {series?.Pages}");
            Console.WriteLine($"  Pages Read: {series?.PagesRead}");
            
            // Get series detail - GET /api/Series/series-detail?seriesId=1
            var detail = await api.GetSeriesDetailAsync(seriesId);
            Console.WriteLine($"Volumes: {detail?.Volumes?.Count}");
            Console.WriteLine($"Chapters: {detail?.Chapters?.Count}");
            Console.WriteLine($"Specials: {detail?.Specials?.Count}");
            
            // Get series metadata - GET /api/Series/metadata?seriesId=1
            var metadata = await api.GetSeriesMetadataAsync(seriesId);
            Console.WriteLine($"Summary: {metadata?.Summary}");
            Console.WriteLine($"Genres: {string.Join(", ", metadata?.Genres?.Select(g => g.Title) ?? [])}");
            Console.WriteLine($"Tags: {string.Join(", ", metadata?.Tags?.Select(t => t.Title) ?? [])}");
            Console.WriteLine($"Writers: {string.Join(", ", metadata?.Writers?.Select(w => w.Name) ?? [])}");
            
            // Get related series - GET /api/Series/all-related?seriesId=1
            var related = await api.GetAllRelatedSeriesAsync(seriesId);
            Console.WriteLine($"Sequels: {related?.Sequels?.Count ?? 0}");
            Console.WriteLine($"Prequels: {related?.Prequels?.Count ?? 0}");
        }

        /// <summary>
        /// Volume and chapter example
        /// </summary>
        public static async Task VolumeChapterExample()
        {
            using var api = new KavitaApiService();
            await api.SetBaseUrl("http://localhost:5000");
            await api.LoginAsync("admin", "password");
            
            // Get volumes for a series - GET /api/Series/volumes?seriesId=1
            var volumes = await api.GetVolumesAsync(1);
            
            foreach (var volume in volumes ?? [])
            {
                Console.WriteLine($"Volume: {volume.Name}");
                Console.WriteLine($"  Pages: {volume.Pages}");
                Console.WriteLine($"  Chapters: {volume.Chapters?.Count}");
                
                foreach (var ch in volume.Chapters ?? [])
                {
                    Console.WriteLine($"    Chapter: {ch.Range} ({ch.Pages} pages)");
                }
            }
            
            // Get specific volume - GET /api/Series/volume?volumeId=1
            var vol = await api.GetVolumeAsync(1);
            Console.WriteLine($"Volume details: {vol?.Name}");
            
            // Get specific chapter - GET /api/Series/chapter?chapterId=1
            var singleChapter = await api.GetChapterAsync(1);
            Console.WriteLine($"Chapter: {singleChapter?.Title} - {singleChapter?.Pages} pages");
        }

        /// <summary>
        /// Reading progress example
        /// </summary>
        public static async Task ReadingProgressExample()
        {
            using var api = new KavitaApiService();
            await api.SetBaseUrl("http://localhost:5000");
            await api.LoginAsync("admin", "password");
            
            int seriesId = 1;
            int chapterId = 1;
            
            // Check if user has progress - GET /api/Reader/has-progress?seriesId=1
            var hasProgress = await api.HasProgressAsync(seriesId);
            Console.WriteLine($"Has progress: {hasProgress}");
            
            // Get continue point - GET /api/Reader/continue-point?seriesId=1
            var continueChapter = await api.GetContinuePointAsync(seriesId);
            Console.WriteLine($"Continue from: Chapter {continueChapter?.Range}");
            
            // Get chapter progress - GET /api/Reader/get-progress?chapterId=1
            var progress = await api.GetProgressAsync(chapterId);
            Console.WriteLine($"Current page: {progress?.PageNum}");
            
            // Save progress - POST /api/Reader/progress
            await api.SaveProgressAsync(new ProgressDto
            {
                ChapterId = chapterId,
                SeriesId = seriesId,
                VolumeId = 1,
                LibraryId = 1,
                PageNum = 50
            });
            Console.WriteLine("Progress saved");
            
            // Mark series as read - POST /api/Reader/mark-read
            await api.MarkSeriesAsReadAsync(seriesId);
            Console.WriteLine("Series marked as read");
            
            // Get time estimate - GET /api/Reader/time-left?seriesId=1
            var timeLeft = await api.GetTimeLeftAsync(seriesId);
            Console.WriteLine($"Time to finish: {timeLeft?.MinHours}-{timeLeft?.MaxHours} hours");
        }

        /// <summary>
        /// Search example
        /// </summary>
        public static async Task SearchExample()
        {
            using var api = new KavitaApiService();
            await api.SetBaseUrl("http://localhost:5000");
            await api.LoginAsync("admin", "password");
            
            // Search - GET /api/Search/search?queryString=manga
            var results = await api.SearchAsync("manga", includeChapterAndFiles: true);
            
            Console.WriteLine($"Series found: {results?.Series?.Count ?? 0}");
            foreach (var series in results?.Series ?? [])
            {
                Console.WriteLine($"  - {series.Name} (Library: {series.LibraryName})");
            }
            
            Console.WriteLine($"Collections found: {results?.Collections?.Count ?? 0}");
            Console.WriteLine($"Reading Lists found: {results?.ReadingLists?.Count ?? 0}");
            Console.WriteLine($"People found: {results?.Persons?.Count ?? 0}");
        }

        /// <summary>
        /// Collections example
        /// </summary>
        public static async Task CollectionsExample()
        {
            using var api = new KavitaApiService();
            await api.SetBaseUrl("http://localhost:5000");
            await api.LoginAsync("admin", "password");
            
            // Get all collections - GET /api/Collection
            var collections = await api.GetCollectionsAsync(ownedOnly: false);
            
            foreach (var collection in collections ?? [])
            {
                Console.WriteLine($"Collection: {collection.Title}");
                Console.WriteLine($"  Summary: {collection.Summary}");
                Console.WriteLine($"  Series Count: {collection.SeriesCount}");
                Console.WriteLine($"  Promoted: {collection.Promoted}");
            }
            
            // Get collections for a specific series - GET /api/Collection/all-series?seriesId=1
            var seriesCollections = await api.GetCollectionsForSeriesAsync(1);
            Console.WriteLine($"Series is in {seriesCollections?.Count ?? 0} collections");
        }

        /// <summary>
        /// Reading lists example
        /// </summary>
        public static async Task ReadingListsExample()
        {
            using var api = new KavitaApiService();
            await api.SetBaseUrl("http://localhost:5000");
            await api.LoginAsync("admin", "password");
            
            // Get all reading lists - POST /api/ReadingList/lists
            var lists = await api.GetReadingListsAsync(0, 20, includePromoted: true);
            
            foreach (var list in lists?.Result ?? [])
            {
                Console.WriteLine($"Reading List: {list.Title}");
                Console.WriteLine($"  Summary: {list.Summary}");
                Console.WriteLine($"  Item Count: {list.ItemCount}");
                
                // Get items in list - GET /api/ReadingList/items?readingListId=1
                var items = await api.GetReadingListItemsAsync(list.Id);
                foreach (var item in items ?? [])
                {
                    Console.WriteLine($"    - {item.SeriesName} Chapter {item.ChapterNumber}");
                }
            }
        }

        /// <summary>
        /// Stats example
        /// </summary>
        public static async Task StatsExample()
        {
            using var api = new KavitaApiService();
            await api.SetBaseUrl("http://localhost:5000");
            await api.LoginAsync("admin", "password");
            
            // Server stats - GET /api/Stats/server/stats
            var serverStats = await api.GetServerStatsAsync();
            Console.WriteLine($"Total Series: {serverStats?.TotalSeries}");
            Console.WriteLine($"Total Files: {serverStats?.TotalFiles}");
            Console.WriteLine($"Total Volumes: {serverStats?.TotalVolumes}");
            Console.WriteLine($"Total Chapters: {serverStats?.TotalChapters}");
            Console.WriteLine($"Total Size: {serverStats?.TotalSize / (1024 * 1024 * 1024.0):F2} GB");
            
            // User stats - GET /api/Stats/user-read
            var userStats = await api.GetUserStatsAsync();
            Console.WriteLine($"Pages Read: {userStats?.TotalPagesRead}");
            Console.WriteLine($"Chapters Read: {userStats?.ChaptersRead}");
            Console.WriteLine($"Words Read: {userStats?.TotalWordsRead}");
        }

        /// <summary>
        /// Metadata browse example
        /// </summary>
        public static async Task MetadataExample()
        {
            using var api = new KavitaApiService();
            await api.SetBaseUrl("http://localhost:5000");
            await api.LoginAsync("admin", "password");
            
            // Get all genres - GET /api/Metadata/genres
            var genres = await api.GetGenresAsync();
            Console.WriteLine($"Genres: {string.Join(", ", genres?.Select(g => g.Title) ?? [])}");
            
            // Get all tags - GET /api/Metadata/tags
            var tags = await api.GetTagsAsync();
            Console.WriteLine($"Tags: {string.Join(", ", tags?.Select(t => t.Title) ?? [])}");
            
            // Get age ratings - GET /api/Metadata/age-ratings
            var ageRatings = await api.GetAgeRatingsAsync();
            foreach (var rating in ageRatings ?? [])
            {
                Console.WriteLine($"Age Rating: {rating.Title} ({rating.Value})");
            }
            
            // Get publication statuses - GET /api/Metadata/publication-status
            var statuses = await api.GetPublicationStatusAsync();
            foreach (var status in statuses ?? [])
            {
                Console.WriteLine($"Status: {status.Title}");
            }
            
            // Get languages - GET /api/Metadata/languages
            var languages = await api.GetLanguagesAsync();
            Console.WriteLine($"Languages: {string.Join(", ", languages?.Select(l => l.Title) ?? [])}");
            
            // Get people - GET /api/Metadata/people
            var people = await api.GetPeopleAsync();
            Console.WriteLine($"Total people: {people?.Count}");
        }

        /// <summary>
        /// Want to read example
        /// </summary>
        public static async Task WantToReadExample()
        {
            using var api = new KavitaApiService();
            await api.SetBaseUrl("http://localhost:5000");
            await api.LoginAsync("admin", "password");
            
            int seriesId = 1;
            
            // Check if in want to read - GET /api/want-to-read?seriesId=1
            var isInList = await api.IsInWantToReadAsync(seriesId);
            Console.WriteLine($"Is in Want to Read: {isInList}");
            
            // Add to want to read - POST /api/want-to-read/add-series
            await api.AddToWantToReadAsync(seriesId);
            Console.WriteLine("Added to Want to Read");
            
            // Remove from want to read - POST /api/want-to-read/remove-series
            await api.RemoveFromWantToReadAsync(seriesId);
            Console.WriteLine("Removed from Want to Read");
        }

        /// <summary>
        /// Image URLs example
        /// </summary>
        public static void ImageUrlsExample()
        {
            using var api = new KavitaApiService();
            // Note: Must set base URL first
            api.SetBaseUrl("http://localhost:5000").Wait();
            
            // Get image URLs (these don't require authentication but can use apiKey)
            var seriesCoverUrl = api.GetSeriesCoverUrl(1, apiKey: "your-api-key");
            Console.WriteLine($"Series Cover: {seriesCoverUrl}");
            
            var volumeCoverUrl = api.GetVolumeCoverUrl(1);
            Console.WriteLine($"Volume Cover: {volumeCoverUrl}");
            
            var chapterCoverUrl = api.GetChapterCoverUrl(1);
            Console.WriteLine($"Chapter Cover: {chapterCoverUrl}");
            
            var libraryCoverUrl = api.GetLibraryCoverUrl(1);
            Console.WriteLine($"Library Cover: {libraryCoverUrl}");
        }
    }
}
