using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using KAssistant.Models;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace KAssistant.Services
{
    /// <summary>
    /// Core OpenAPI-based Kavita API service
    /// Provides simple HTTP methods for API communication
    /// </summary>
    public class OpenApiKavitaService : IDisposable
    {
        private HttpClient? _httpClient;
        private string? _baseUrl;
        private string? _authToken;
        private readonly object _lockObject = new();
        private OpenApiDocument? _openApiDocument;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
            ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            PropertyNamingPolicy = null,
            UnknownTypeHandling = System.Text.Json.Serialization.JsonUnknownTypeHandling.JsonElement,
            UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Skip
        };

        /// <summary>
        /// Enables or disables detailed logging
        /// </summary>
        public bool EnableDetailedLogging { get; set; } = true;

        /// <summary>
        /// Log file path for detailed error logging
        /// </summary>
        public string? LogFilePath { get; set; }

        public OpenApiKavitaService()
        {
            LoadOpenApiDocument();
            InitializeLogging();
        }

        private void InitializeLogging()
        {
            try
            {
                var logDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }
                LogFilePath = Path.Combine(logDirectory, $"kavita_api_{DateTime.Now:yyyy-MM-dd}.log");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not initialize log file: {ex.Message}");
            }
        }

        private void LogError(string method, string path, Exception ex, string? requestBody = null, string? responseBody = null, int? statusCode = null, long? elapsedMs = null)
        {
            if (!EnableDetailedLogging) return;

            var logEntry = new StringBuilder();
            logEntry.AppendLine("========================================");
            logEntry.AppendLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
            logEntry.AppendLine($"Method: {method}");
            logEntry.AppendLine($"Path: {path}");
            logEntry.AppendLine($"Base URL: {_baseUrl}");
            logEntry.AppendLine($"Full URL: {_baseUrl}{path}");
            
            if (statusCode.HasValue)
            {
                logEntry.AppendLine($"Status Code: {statusCode}");
            }
            
            if (elapsedMs.HasValue)
            {
                logEntry.AppendLine($"Elapsed Time: {elapsedMs}ms");
            }

            logEntry.AppendLine($"Exception Type: {ex.GetType().FullName}");
            logEntry.AppendLine($"Exception Message: {ex.Message}");

            if (ex.InnerException != null)
            {
                logEntry.AppendLine($"Inner Exception Type: {ex.InnerException.GetType().FullName}");
                logEntry.AppendLine($"Inner Exception Message: {ex.InnerException.Message}");
            }

            if (!string.IsNullOrEmpty(requestBody))
            {
                logEntry.AppendLine("Request Body:");
                logEntry.AppendLine(TruncateForLog(requestBody, 2000));
            }

            if (!string.IsNullOrEmpty(responseBody))
            {
                logEntry.AppendLine("Response Body:");
                logEntry.AppendLine(TruncateForLog(responseBody, 2000));
            }

            logEntry.AppendLine("Stack Trace:");
            logEntry.AppendLine(ex.StackTrace);
            logEntry.AppendLine("========================================");

            var logMessage = logEntry.ToString();

            // Write to console
            Console.WriteLine(logMessage);

            // Write to file
            WriteToLogFile(logMessage);
        }

        private void LogRequest(string method, string path, string? requestBody = null)
        {
            if (!EnableDetailedLogging) return;

            var logEntry = new StringBuilder();
            logEntry.AppendLine($"[REQUEST] {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
            logEntry.AppendLine($"Method: {method} | Path: {path}");
            
            if (!string.IsNullOrEmpty(requestBody))
            {
                logEntry.AppendLine($"Body: {TruncateForLog(requestBody, 500)}");
            }

            Console.WriteLine(logEntry.ToString());
        }

        private void LogResponse(string method, string path, int statusCode, string? responseBody, long elapsedMs)
        {
            if (!EnableDetailedLogging) return;

            var logEntry = new StringBuilder();
            logEntry.AppendLine($"[RESPONSE] {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
            logEntry.AppendLine($"Method: {method} | Path: {path} | Status: {statusCode} | Time: {elapsedMs}ms");
            
            if (!string.IsNullOrEmpty(responseBody))
            {
                logEntry.AppendLine($"Body Preview: {TruncateForLog(responseBody, 300)}");
            }

            Console.WriteLine(logEntry.ToString());
        }

        private void WriteToLogFile(string message)
        {
            if (string.IsNullOrEmpty(LogFilePath)) return;

            try
            {
                lock (_lockObject)
                {
                    File.AppendAllText(LogFilePath, message + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not write to log file: {ex.Message}");
            }
        }

        private static string TruncateForLog(string content, int maxLength)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;
            
            if (content.Length <= maxLength) return content;
            
            return content.Substring(0, maxLength) + $"... [truncated, total length: {content.Length}]";
        }

        private void LoadOpenApiDocument()
        {
            try
            {
                var apiJsonPath = Path.Combine(AppContext.BaseDirectory, "Models", "kavita_api.json");
                if (!File.Exists(apiJsonPath))
                {
                    Console.WriteLine($"Warning: kavita_api.json not found at: {apiJsonPath}");
                    return;
                }

                using var stream = File.OpenRead(apiJsonPath);
                var reader = new OpenApiStreamReader();
                _openApiDocument = reader.Read(stream, out var diagnostic);

                if (diagnostic.Errors.Count > 0)
                {
                    Console.WriteLine("OpenAPI parsing errors:");
                    foreach (var error in diagnostic.Errors)
                    {
                        Console.WriteLine($"  - {error.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading OpenAPI document: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the loaded OpenAPI document for inspection
        /// </summary>
        public OpenApiDocument? GetOpenApiDocument() => _openApiDocument;

        /// <summary>
        /// Gets all available API paths from the OpenAPI document
        /// </summary>
        public IEnumerable<string> GetAvailablePaths()
        {
            return _openApiDocument?.Paths.Keys ?? Enumerable.Empty<string>();
        }

        /// <summary>
        /// Gets information about a specific API path
        /// </summary>
        public OpenApiPathItem? GetPathInfo(string path)
        {
            if (_openApiDocument?.Paths.TryGetValue(path, out var pathItem) == true)
            {
                return pathItem;
            }
            return null;
        }

        public Task SetBaseUrl(string baseUrl)
        {
            lock (_lockObject)
            {
                _baseUrl = baseUrl.TrimEnd('/');
                _httpClient?.Dispose();
                _httpClient = null;
            }
            return Task.CompletedTask;
        }

        public void SetAuthToken(string token)
        {
            lock (_lockObject)
            {
                _authToken = token;
                if (_httpClient != null)
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", token);
                }
            }
        }

        public void ClearAuthToken()
        {
            lock (_lockObject)
            {
                _authToken = null;
                if (_httpClient != null)
                {
                    _httpClient.DefaultRequestHeaders.Authorization = null;
                }
            }
        }

        private HttpClient GetHttpClient()
        {
            lock (_lockObject)
            {
                if (_httpClient == null)
                {
                    if (string.IsNullOrEmpty(_baseUrl))
                    {
                        throw new InvalidOperationException("Base URL not set. Call SetBaseUrl first.");
                    }

                    _httpClient = new HttpClient
                    {
                        BaseAddress = new Uri(_baseUrl),
                        Timeout = TimeSpan.FromSeconds(30)
                    };

                    _httpClient.DefaultRequestHeaders.Accept.Clear();
                    _httpClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    if (!string.IsNullOrEmpty(_authToken))
                    {
                        _httpClient.DefaultRequestHeaders.Authorization = 
                            new AuthenticationHeaderValue("Bearer", _authToken);
                    }
                }

                return _httpClient;
            }
        }

        public async Task<T?> GetAsync<T>(string path)
        {
            var stopwatch = Stopwatch.StartNew();
            string? responseContent = null;
            int? statusCode = null;

            try
            {
                LogRequest("GET", path);
                
                var client = GetHttpClient();
                var response = await client.GetAsync(path);
                
                statusCode = (int)response.StatusCode;
                responseContent = await response.Content.ReadAsStringAsync();
                
                stopwatch.Stop();
                LogResponse("GET", path, statusCode.Value, responseContent, stopwatch.ElapsedMilliseconds);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = $"API Error: {path} returned {response.StatusCode}";
                    Console.WriteLine(errorMessage);
                    Console.WriteLine($"Response: {responseContent}");
                    
                    LogError("GET", path, new HttpRequestException(errorMessage), 
                        responseBody: responseContent, 
                        statusCode: statusCode, 
                        elapsedMs: stopwatch.ElapsedMilliseconds);
                    
                    response.EnsureSuccessStatusCode();
                }
                
                if (string.IsNullOrWhiteSpace(responseContent))
                {
                    Console.WriteLine($"Warning: Empty response from {path}");
                    return default;
                }
                
                try
                {
                    return JsonSerializer.Deserialize<T>(responseContent, JsonOptions);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"JSON Deserialization Error for {path}:");
                    Console.WriteLine($"  Exception: {ex.Message}");
                    Console.WriteLine($"  Response: {responseContent.Substring(0, Math.Min(500, responseContent.Length))}...");
                    
                    LogError("GET", path, ex, 
                        responseBody: responseContent, 
                        statusCode: statusCode, 
                        elapsedMs: stopwatch.ElapsedMilliseconds);
                    throw;
                }
            }
            catch (HttpRequestException ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"HTTP Request Error for {path}: {ex.Message}");
                
                LogError("GET", path, ex, 
                    responseBody: responseContent, 
                    statusCode: statusCode, 
                    elapsedMs: stopwatch.ElapsedMilliseconds);
                throw;
            }
            catch (TaskCanceledException ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Request Timeout for {path}: {ex.Message}");
                
                LogError("GET", path, ex, 
                    responseBody: responseContent, 
                    statusCode: statusCode, 
                    elapsedMs: stopwatch.ElapsedMilliseconds);
                throw;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                LogError("GET", path, ex, 
                    responseBody: responseContent, 
                    statusCode: statusCode, 
                    elapsedMs: stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public async Task<T?> PostAsync<T>(string path, object? body = null)
        {
            var stopwatch = Stopwatch.StartNew();
            string? requestJson = null;
            string? responseBody = null;
            int? statusCode = null;

            try
            {
                var client = GetHttpClient();
                
                HttpContent? content = null;
                if (body != null)
                {
                    requestJson = JsonSerializer.Serialize(body, JsonOptions);
                    content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                }

                LogRequest("POST", path, requestJson);

                var response = await client.PostAsync(path, content);
                
                statusCode = (int)response.StatusCode;
                responseBody = await response.Content.ReadAsStringAsync();
                
                stopwatch.Stop();
                LogResponse("POST", path, statusCode.Value, responseBody, stopwatch.ElapsedMilliseconds);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = $"API Error: {path} returned {response.StatusCode}";
                    Console.WriteLine(errorMessage);
                    Console.WriteLine($"Response: {responseBody}");
                    
                    LogError("POST", path, new HttpRequestException(errorMessage), 
                        requestBody: requestJson, 
                        responseBody: responseBody, 
                        statusCode: statusCode, 
                        elapsedMs: stopwatch.ElapsedMilliseconds);
                    
                    response.EnsureSuccessStatusCode();
                }
                
                if (string.IsNullOrWhiteSpace(responseBody))
                {
                    Console.WriteLine($"Warning: Empty response from {path}");
                    return default;
                }
                
                try
                {
                    return JsonSerializer.Deserialize<T>(responseBody, JsonOptions);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"JSON Deserialization Error for {path}:");
                    Console.WriteLine($"  Exception: {ex.Message}");
                    Console.WriteLine($"  Response: {responseBody.Substring(0, Math.Min(500, responseBody.Length))}...");
                    
                    LogError("POST", path, ex, 
                        requestBody: requestJson, 
                        responseBody: responseBody, 
                        statusCode: statusCode, 
                        elapsedMs: stopwatch.ElapsedMilliseconds);
                    throw;
                }
            }
            catch (HttpRequestException ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"HTTP Request Error for {path}: {ex.Message}");
                
                LogError("POST", path, ex, 
                    requestBody: requestJson, 
                    responseBody: responseBody, 
                    statusCode: statusCode, 
                    elapsedMs: stopwatch.ElapsedMilliseconds);
                throw;
            }
            catch (TaskCanceledException ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Request Timeout for {path}: {ex.Message}");
                
                LogError("POST", path, ex, 
                    requestBody: requestJson, 
                    responseBody: responseBody, 
                    statusCode: statusCode, 
                    elapsedMs: stopwatch.ElapsedMilliseconds);
                throw;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                LogError("POST", path, ex, 
                    requestBody: requestJson, 
                    responseBody: responseBody, 
                    statusCode: statusCode, 
                    elapsedMs: stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public async Task PostAsync(string path, object? body = null)
        {
            var stopwatch = Stopwatch.StartNew();
            string? requestJson = null;
            string? responseBody = null;
            int? statusCode = null;

            try
            {
                var client = GetHttpClient();
                
                HttpContent? content = null;
                if (body != null)
                {
                    requestJson = JsonSerializer.Serialize(body, JsonOptions);
                    content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                }

                LogRequest("POST", path, requestJson);

                var response = await client.PostAsync(path, content);
                
                statusCode = (int)response.StatusCode;
                responseBody = await response.Content.ReadAsStringAsync();
                
                stopwatch.Stop();
                LogResponse("POST", path, statusCode.Value, responseBody, stopwatch.ElapsedMilliseconds);
                
                if (!response.IsSuccessStatusCode)
                {
                    LogError("POST", path, new HttpRequestException($"Status: {response.StatusCode}"), 
                        requestBody: requestJson, 
                        responseBody: responseBody, 
                        statusCode: statusCode, 
                        elapsedMs: stopwatch.ElapsedMilliseconds);
                }
                
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                LogError("POST", path, ex, 
                    requestBody: requestJson, 
                    responseBody: responseBody, 
                    statusCode: statusCode, 
                    elapsedMs: stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public async Task<T?> DeleteAsync<T>(string path)
        {
            var stopwatch = Stopwatch.StartNew();
            string? responseBody = null;
            int? statusCode = null;

            try
            {
                LogRequest("DELETE", path);
                
                var client = GetHttpClient();
                var response = await client.DeleteAsync(path);
                
                statusCode = (int)response.StatusCode;
                responseBody = await response.Content.ReadAsStringAsync();
                
                stopwatch.Stop();
                LogResponse("DELETE", path, statusCode.Value, responseBody, stopwatch.ElapsedMilliseconds);
                
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"API Error: {path} returned {response.StatusCode}");
                    
                    LogError("DELETE", path, new HttpRequestException($"Status: {response.StatusCode}"), 
                        responseBody: responseBody, 
                        statusCode: statusCode, 
                        elapsedMs: stopwatch.ElapsedMilliseconds);
                    
                    response.EnsureSuccessStatusCode();
                }
                
                if (string.IsNullOrWhiteSpace(responseBody))
                {
                    return default;
                }
                
                return JsonSerializer.Deserialize<T>(responseBody, JsonOptions);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                LogError("DELETE", path, ex, 
                    responseBody: responseBody, 
                    statusCode: statusCode, 
                    elapsedMs: stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public async Task DeleteAsync(string path)
        {
            var stopwatch = Stopwatch.StartNew();
            string? responseBody = null;
            int? statusCode = null;

            try
            {
                LogRequest("DELETE", path);
                
                var client = GetHttpClient();
                var response = await client.DeleteAsync(path);
                
                statusCode = (int)response.StatusCode;
                responseBody = await response.Content.ReadAsStringAsync();
                
                stopwatch.Stop();
                LogResponse("DELETE", path, statusCode.Value, responseBody, stopwatch.ElapsedMilliseconds);
                
                if (!response.IsSuccessStatusCode)
                {
                    LogError("DELETE", path, new HttpRequestException($"Status: {response.StatusCode}"), 
                        responseBody: responseBody, 
                        statusCode: statusCode, 
                        elapsedMs: stopwatch.ElapsedMilliseconds);
                }
                
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                LogError("DELETE", path, ex, 
                    responseBody: responseBody, 
                    statusCode: statusCode, 
                    elapsedMs: stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        #region Authentication - /api/Account

        /// <summary>
        /// POST /api/Account/login - Perform a login
        /// </summary>
        public async Task<UserDto?> LoginAsync(string username, string password)
        {
            var loginRequest = new LoginRequest { Username = username, Password = password };
            var result = await PostAsync<UserDto>("/api/Account/login", loginRequest);
            
            if (result?.Token != null)
            {
                SetAuthToken(result.Token);
            }
            
            return result;
        }

        /// <summary>
        /// GET /api/Account - Returns the current user
        /// </summary>
        public Task<UserDto?> GetCurrentUserAsync()
        {
            return GetAsync<UserDto>("/api/Account");
        }

        /// <summary>
        /// GET /api/Account/roles - Get all roles
        /// </summary>
        public Task<List<string>?> GetRolesAsync()
        {
            return GetAsync<List<string>>("/api/Account/roles");
        }

        #endregion

        #region Admin - /api/Admin

        /// <summary>
        /// GET /api/Admin/exists - Checks if an admin exists
        /// </summary>
        public Task<bool> AdminExistsAsync()
        {
            return GetAsync<bool>("/api/Admin/exists");
        }

        #endregion

        #region Server - /api/Server

        /// <summary>
        /// GET /api/Server/server-info-slim - Returns non-sensitive server info
        /// </summary>
        public Task<ServerInfoResponse?> GetServerInfoAsync()
        {
            return GetAsync<ServerInfoResponse>("/api/Server/server-info-slim");
        }

        /// <summary>
        /// GET /api/Server/jobs - Returns list of reoccurring jobs
        /// </summary>
        public Task<List<JobDto>?> GetJobsAsync()
        {
            return GetAsync<List<JobDto>>("/api/Server/jobs");
        }

        /// <summary>
        /// GET /api/Server/check-update - Checks for updates
        /// </summary>
        public Task<UpdateNotificationDto?> CheckForUpdatesAsync()
        {
            return GetAsync<UpdateNotificationDto>("/api/Server/check-update");
        }

        /// <summary>
        /// POST /api/Server/clear-cache - Performs cleanup of cache
        /// </summary>
        public Task ClearCacheAsync()
        {
            return PostAsync("/api/Server/clear-cache");
        }

        /// <summary>
        /// POST /api/Server/backup-db - Performs backup of database
        /// </summary>
        public Task BackupDatabaseAsync()
        {
            return PostAsync("/api/Server/backup-db");
        }

        #endregion

        #region Health - /api/Health

        /// <summary>
        /// GET /api/Health - Health check endpoint
        /// </summary>
        public Task<string?> HealthCheckAsync()
        {
            return GetAsync<string>("/api/Health");
        }

        #endregion

        #region Libraries - /api/Library

        /// <summary>
        /// GET /api/Library/libraries - Return all libraries
        /// </summary>
        public Task<List<Library>?> GetLibrariesAsync()
        {
            return GetAsync<List<Library>>("/api/Library/libraries");
        }

        /// <summary>
        /// GET /api/Library - Return a specific library
        /// </summary>
        public Task<Library?> GetLibraryAsync(int libraryId)
        {
            return GetAsync<Library>($"/api/Library?libraryId={libraryId}");
        }

        /// <summary>
        /// GET /api/Library/type - Returns the type of the library
        /// </summary>
        public Task<int> GetLibraryTypeAsync(int libraryId)
        {
            return GetAsync<int>($"/api/Library/type?libraryId={libraryId}");
        }

        /// <summary>
        /// POST /api/Library/scan - Scans a library for file changes
        /// </summary>
        public Task ScanLibraryAsync(int libraryId, bool force = false)
        {
            return PostAsync($"/api/Library/scan?libraryId={libraryId}&force={force}");
        }

        /// <summary>
        /// POST /api/Library/scan-all - Scans all libraries
        /// </summary>
        public Task ScanAllLibrariesAsync(bool force = false)
        {
            return PostAsync($"/api/Library/scan-all?force={force}");
        }

        /// <summary>
        /// POST /api/Library/refresh-metadata - Runs Cover Image Generation task
        /// </summary>
        public Task RefreshLibraryMetadataAsync(int libraryId, bool force = true, bool forceColorscape = true)
        {
            return PostAsync($"/api/Library/refresh-metadata?libraryId={libraryId}&force={force}&forceColorscape={forceColorscape}");
        }

        /// <summary>
        /// GET /api/Library/name-exists - Checks if library name exists
        /// </summary>
        public Task<bool> LibraryNameExistsAsync(string name)
        {
            return GetAsync<bool>($"/api/Library/name-exists?name={Uri.EscapeDataString(name)}");
        }

        #endregion

        #region Series - /api/Series

        /// <summary>
        /// GET /api/Series/{seriesId} - Fetches a Series by Id
        /// </summary>
        public Task<Series?> GetSeriesAsync(int seriesId)
        {
            return GetAsync<Series>($"/api/Series/{seriesId}");
        }

        /// <summary>
        /// GET /api/Series/series-detail - Get series detail page data
        /// </summary>
        public Task<SeriesDetailDto?> GetSeriesDetailAsync(int seriesId)
        {
            return GetAsync<SeriesDetailDto>($"/api/Series/series-detail?seriesId={seriesId}");
        }

        /// <summary>
        /// GET /api/Series/metadata - Returns metadata for a series
        /// </summary>
        public Task<SeriesMetadata?> GetSeriesMetadataAsync(int seriesId)
        {
            return GetAsync<SeriesMetadata>($"/api/Series/metadata?seriesId={seriesId}");
        }

        /// <summary>
        /// POST /api/Series/metadata - Update series metadata
        /// </summary>
        public Task UpdateSeriesMetadataAsync(UpdateSeriesMetadataDto metadata)
        {
            return PostAsync("/api/Series/metadata", metadata);
        }

        /// <summary>
        /// GET /api/Series/volumes - Returns all volumes for a series
        /// </summary>
        public Task<List<VolumeDto>?> GetVolumesAsync(int seriesId)
        {
            return GetAsync<List<VolumeDto>>($"/api/Series/volumes?seriesId={seriesId}");
        }

        /// <summary>
        /// GET /api/Series/volume - Returns a specific volume
        /// </summary>
        public Task<VolumeDto?> GetVolumeAsync(int volumeId)
        {
            return GetAsync<VolumeDto>($"/api/Series/volume?volumeId={volumeId}");
        }

        /// <summary>
        /// GET /api/Series/chapter - Returns a specific chapter
        /// </summary>
        public Task<ChapterDto?> GetChapterAsync(int chapterId)
        {
            return GetAsync<ChapterDto>($"/api/Series/chapter?chapterId={chapterId}");
        }

        /// <summary>
        /// POST /api/Series/v2 - Gets series with filter
        /// </summary>
        public Task<PaginatedResult<Series>?> GetSeriesV2Async(FilterV2Dto filter, int pageNumber = 0, int pageSize = 20)
        {
            return PostAsync<PaginatedResult<Series>>($"/api/Series/v2?PageNumber={pageNumber}&PageSize={pageSize}", filter);
        }

        /// <summary>
        /// POST /api/Series/recently-added-v2 - Gets recently added series
        /// </summary>
        public async Task<PaginatedResult<Series>?> GetRecentlyAddedSeriesAsync(
            int pageNumber = 0, 
            int pageSize = 20, 
            int? libraryId = null)
        {
            var filter = new FilterV2Dto();
            
            var path = libraryId.HasValue 
                ? $"/api/Series/recently-added-v2?PageNumber={pageNumber}&PageSize={pageSize}&libraryId={libraryId}" 
                : $"/api/Series/recently-added-v2?PageNumber={pageNumber}&PageSize={pageSize}";
            
            return await PostAsync<PaginatedResult<Series>>(path, filter);
        }

        /// <summary>
        /// POST /api/Series/on-deck - Fetches series on deck
        /// </summary>
        public async Task<PaginatedResult<Series>?> GetOnDeckSeriesAsync(
            int pageNumber = 0, 
            int pageSize = 20, 
            int? libraryId = null)
        {
            var path = $"/api/Series/on-deck?PageNumber={pageNumber}&PageSize={pageSize}&libraryId={libraryId ?? 0}";
            return await PostAsync<PaginatedResult<Series>>(path);
        }

        /// <summary>
        /// POST /api/Series/all-v2 - Returns all series for the library
        /// </summary>
        public async Task<PaginatedResult<Series>?> GetAllSeriesAsync(
            int libraryId, 
            int pageNumber = 0, 
            int pageSize = 20)
        {
            // If libraryId is 0 or negative, fetch from all libraries
            if (libraryId <= 0)
            {
                Console.WriteLine($"Loading series from all libraries...");
                
                var libraries = await GetLibrariesAsync();
                if (libraries == null || libraries.Count == 0)
                {
                    Console.WriteLine("No libraries found");
                    return new PaginatedResult<Series>
                    {
                        CurrentPage = pageNumber,
                        PageSize = pageSize,
                        TotalCount = 0,
                        TotalPages = 0,
                        Result = new List<Series>()
                    };
                }

                Console.WriteLine($"Found {libraries.Count} libraries");
                
                var allSeries = new List<Series>();
                
                foreach (var library in libraries)
                {
                    Console.WriteLine($"Loading from library: {library.Name} (ID: {library.Id})");
                    
                    try
                    {
                        int currentPage = 0;
                        bool hasMore = true;
                        int librarySeriesCount = 0;
                        
                        while (hasMore)
                        {
                            var libraryResult = await GetRecentlyAddedSeriesAsync(currentPage, 100, library.Id);
                            
                            if (libraryResult?.Result != null && libraryResult.Result.Count > 0)
                            {
                                foreach (var series in libraryResult.Result)
                                {
                                    if (!allSeries.Any(s => s.Id == series.Id))
                                    {
                                        allSeries.Add(series);
                                        librarySeriesCount++;
                                    }
                                }
                                
                                currentPage++;
                                hasMore = currentPage < libraryResult.TotalPages;
                            }
                            else
                            {
                                hasMore = false;
                            }
                        }
                        
                        if (librarySeriesCount > 0)
                        {
                            Console.WriteLine($"  ? Loaded {librarySeriesCount} series from {library.Name}");
                        }
                        else
                        {
                            Console.WriteLine($"  - No series in {library.Name}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  ? Error in {library.Name}: {ex.Message}");
                        continue;
                    }
                }

                if (allSeries.Count > 0)
                {
                    Console.WriteLine($"? Successfully loaded {allSeries.Count} total series");
                }
                else
                {
                    Console.WriteLine($"? No series could be loaded from any library");
                }
                
                allSeries = allSeries.OrderBy(s => s.Name).ToList();
                
                return new PaginatedResult<Series>
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalCount = allSeries.Count,
                    TotalPages = (int)Math.Ceiling(allSeries.Count / (double)pageSize),
                    Result = allSeries
                };
            }
            
            var result = await GetRecentlyAddedSeriesAsync(pageNumber, pageSize, libraryId);
            if (result != null)
            {
                result.Result = result.Result.OrderBy(s => s.Name).ToList();
            }
            return result;
        }

        /// <summary>
        /// POST /api/Series/refresh-metadata - Runs Cover Image Generation task for series
        /// </summary>
        public Task RefreshSeriesMetadataAsync(int libraryId, int seriesId, bool forceUpdate = true, bool forceColorscape = false)
        {
            var request = new RefreshSeriesDto
            {
                LibraryId = libraryId,
                SeriesId = seriesId,
                ForceUpdate = forceUpdate,
                ForceColorscape = forceColorscape
            };
            return PostAsync("/api/Series/refresh-metadata", request);
        }

        /// <summary>
        /// POST /api/Series/scan - Scan a series
        /// </summary>
        public Task ScanSeriesAsync(int libraryId, int seriesId)
        {
            var request = new RefreshSeriesDto
            {
                LibraryId = libraryId,
                SeriesId = seriesId
            };
            return PostAsync("/api/Series/scan", request);
        }

        /// <summary>
        /// DELETE /api/Series/{seriesId} - Deletes a series
        /// </summary>
        public Task<bool> DeleteSeriesAsync(int seriesId)
        {
            return DeleteAsync<bool>($"/api/Series/{seriesId}");
        }

        /// <summary>
        /// GET /api/Series/related - Fetches related series
        /// </summary>
        public Task<List<Series>?> GetRelatedSeriesAsync(int seriesId, int relation)
        {
            return GetAsync<List<Series>>($"/api/Series/related?seriesId={seriesId}&relation={relation}");
        }

        /// <summary>
        /// GET /api/Series/all-related - Returns all related series
        /// </summary>
        public Task<RelatedSeriesDto?> GetAllRelatedSeriesAsync(int seriesId)
        {
            return GetAsync<RelatedSeriesDto>($"/api/Series/all-related?seriesId={seriesId}");
        }

        #endregion

        #region Search - /api/Search

        /// <summary>
        /// GET /api/Search/search - Searches against different entities
        /// </summary>
        public Task<SearchResultGroupDto?> SearchAsync(string queryString, bool includeChapterAndFiles = true)
        {
            return GetAsync<SearchResultGroupDto>($"/api/Search/search?queryString={Uri.EscapeDataString(queryString)}&includeChapterAndFiles={includeChapterAndFiles}");
        }

        /// <summary>
        /// GET /api/Search/series-for-chapter - Returns series for chapter id
        /// </summary>
        public Task<Series?> GetSeriesForChapterAsync(int chapterId)
        {
            return GetAsync<Series>($"/api/Search/series-for-chapter?chapterId={chapterId}");
        }

        #endregion

        #region Reader - /api/Reader

        /// <summary>
        /// GET /api/Reader/chapter-info - Returns chapter info for reading
        /// </summary>
        public Task<ChapterInfoDto?> GetChapterInfoAsync(int chapterId, bool extractPdf = false, bool includeDimensions = false)
        {
            return GetAsync<ChapterInfoDto>($"/api/Reader/chapter-info?chapterId={chapterId}&extractPdf={extractPdf}&includeDimensions={includeDimensions}");
        }

        /// <summary>
        /// GET /api/Reader/get-progress - Returns progress for a chapter
        /// </summary>
        public Task<ProgressDto?> GetProgressAsync(int chapterId)
        {
            return GetAsync<ProgressDto>($"/api/Reader/get-progress?chapterId={chapterId}");
        }

        /// <summary>
        /// POST /api/Reader/progress - Save progress for a chapter
        /// </summary>
        public Task SaveProgressAsync(ProgressDto progress)
        {
            return PostAsync("/api/Reader/progress", progress);
        }

        /// <summary>
        /// GET /api/Reader/continue-point - Get the chapter to continue reading from
        /// </summary>
        public Task<ChapterDto?> GetContinuePointAsync(int seriesId)
        {
            return GetAsync<ChapterDto>($"/api/Reader/continue-point?seriesId={seriesId}");
        }

        /// <summary>
        /// GET /api/Reader/has-progress - Returns if user has progress on series
        /// </summary>
        public Task<bool> HasProgressAsync(int seriesId)
        {
            return GetAsync<bool>($"/api/Reader/has-progress?seriesId={seriesId}");
        }

        /// <summary>
        /// POST /api/Reader/mark-read - Marks a series as read
        /// </summary>
        public Task MarkSeriesAsReadAsync(int seriesId)
        {
            var request = new MarkReadDto { SeriesId = seriesId };
            return PostAsync("/api/Reader/mark-read", request);
        }

        /// <summary>
        /// POST /api/Reader/mark-unread - Marks a series as unread
        /// </summary>
        public Task MarkSeriesAsUnreadAsync(int seriesId)
        {
            var request = new MarkReadDto { SeriesId = seriesId };
            return PostAsync("/api/Reader/mark-unread", request);
        }

        /// <summary>
        /// GET /api/Reader/next-chapter - Returns the next chapter
        /// </summary>
        public Task<int> GetNextChapterAsync(int seriesId, int volumeId, int currentChapterId)
        {
            return GetAsync<int>($"/api/Reader/next-chapter?seriesId={seriesId}&volumeId={volumeId}&currentChapterId={currentChapterId}");
        }

        /// <summary>
        /// GET /api/Reader/prev-chapter - Returns the previous chapter
        /// </summary>
        public Task<int> GetPrevChapterAsync(int seriesId, int volumeId, int currentChapterId)
        {
            return GetAsync<int>($"/api/Reader/prev-chapter?seriesId={seriesId}&volumeId={volumeId}&currentChapterId={currentChapterId}");
        }

        /// <summary>
        /// GET /api/Reader/time-left - Returns estimate time to finish series
        /// </summary>
        public Task<HourEstimateRangeDto?> GetTimeLeftAsync(int seriesId)
        {
            return GetAsync<HourEstimateRangeDto>($"/api/Reader/time-left?seriesId={seriesId}");
        }

        #endregion

        #region Stats - /api/Stats

        /// <summary>
        /// GET /api/Stats/server/stats - Returns server statistics
        /// </summary>
        public Task<ServerStats?> GetServerStatsAsync()
        {
            return GetAsync<ServerStats>("/api/Stats/server/stats");
        }

        /// <summary>
        /// GET /api/Stats/user-read - Returns user reading statistics
        /// </summary>
        public Task<UserStats?> GetUserStatsAsync(int? userId = null)
        {
            var path = userId.HasValue 
                ? $"/api/Stats/user-read?userId={userId}" 
                : "/api/Stats/user-read";
            return GetAsync<UserStats>(path);
        }

        #endregion

        #region Collections - /api/Collection

        /// <summary>
        /// GET /api/Collection - Returns all collection tags for a user
        /// </summary>
        public Task<List<Collection>?> GetCollectionsAsync(bool ownedOnly = false)
        {
            return GetAsync<List<Collection>>($"/api/Collection?ownedOnly={ownedOnly}");
        }

        /// <summary>
        /// GET /api/Collection/single - Returns a single collection by id
        /// </summary>
        public Task<List<Collection>?> GetCollectionAsync(int collectionId)
        {
            return GetAsync<List<Collection>>($"/api/Collection/single?collectionId={collectionId}");
        }

        /// <summary>
        /// GET /api/Collection/all-series - Returns collections containing the series
        /// </summary>
        public Task<List<Collection>?> GetCollectionsForSeriesAsync(int seriesId, bool ownedOnly = false)
        {
            return GetAsync<List<Collection>>($"/api/Collection/all-series?seriesId={seriesId}&ownedOnly={ownedOnly}");
        }

        /// <summary>
        /// DELETE /api/Collection - Removes a collection
        /// </summary>
        public Task DeleteCollectionAsync(int tagId)
        {
            return DeleteAsync($"/api/Collection?tagId={tagId}");
        }

        #endregion

        #region Reading Lists - /api/ReadingList

        /// <summary>
        /// GET /api/ReadingList - Fetches a single reading list
        /// </summary>
        public Task<ReadingListDto?> GetReadingListAsync(int readingListId)
        {
            return GetAsync<ReadingListDto>($"/api/ReadingList?readingListId={readingListId}");
        }

        /// <summary>
        /// POST /api/ReadingList/lists - Returns reading lists
        /// </summary>
        public Task<PaginatedResult<ReadingListDto>?> GetReadingListsAsync(int pageNumber = 0, int pageSize = 20, bool includePromoted = true)
        {
            return PostAsync<PaginatedResult<ReadingListDto>>($"/api/ReadingList/lists?PageNumber={pageNumber}&PageSize={pageSize}&includePromoted={includePromoted}");
        }

        /// <summary>
        /// GET /api/ReadingList/items - Fetches all reading list items
        /// </summary>
        public Task<List<ReadingListItemDto>?> GetReadingListItemsAsync(int readingListId)
        {
            return GetAsync<List<ReadingListItemDto>>($"/api/ReadingList/items?readingListId={readingListId}");
        }

        /// <summary>
        /// DELETE /api/ReadingList - Deletes a reading list
        /// </summary>
        public Task DeleteReadingListAsync(int readingListId)
        {
            return DeleteAsync($"/api/ReadingList?readingListId={readingListId}");
        }

        #endregion

        #region Users - /api/Users

        /// <summary>
        /// GET /api/Users - Returns all users
        /// </summary>
        public Task<List<User>?> GetUsersAsync(bool includePending = false)
        {
            return GetAsync<List<User>>($"/api/Users?includePending={includePending}");
        }

        /// <summary>
        /// GET /api/Users/names - Returns a list of usernames
        /// </summary>
        public Task<List<string>?> GetUserNamesAsync()
        {
            return GetAsync<List<string>>("/api/Users/names");
        }

        /// <summary>
        /// GET /api/Users/get-preferences - Returns user preferences
        /// </summary>
        public Task<UserPreferencesDto?> GetUserPreferencesAsync()
        {
            return GetAsync<UserPreferencesDto>("/api/Users/get-preferences");
        }

        #endregion

        #region Metadata - /api/Metadata

        /// <summary>
        /// GET /api/Metadata/genres - Fetches genres from the instance
        /// </summary>
        public Task<List<GenreTagDto>?> GetGenresAsync(string? libraryIds = null)
        {
            var path = string.IsNullOrEmpty(libraryIds) 
                ? "/api/Metadata/genres" 
                : $"/api/Metadata/genres?libraryIds={libraryIds}";
            return GetAsync<List<GenreTagDto>>(path);
        }

        /// <summary>
        /// GET /api/Metadata/tags - Fetches all tags from the instance
        /// </summary>
        public Task<List<TagDto>?> GetTagsAsync(string? libraryIds = null)
        {
            var path = string.IsNullOrEmpty(libraryIds) 
                ? "/api/Metadata/tags" 
                : $"/api/Metadata/tags?libraryIds={libraryIds}";
            return GetAsync<List<TagDto>>(path);
        }

        /// <summary>
        /// GET /api/Metadata/age-ratings - Fetches all age ratings
        /// </summary>
        public Task<List<AgeRatingDto>?> GetAgeRatingsAsync(string? libraryIds = null)
        {
            var path = string.IsNullOrEmpty(libraryIds) 
                ? "/api/Metadata/age-ratings" 
                : $"/api/Metadata/age-ratings?libraryIds={libraryIds}";
            return GetAsync<List<AgeRatingDto>>(path);
        }

        /// <summary>
        /// GET /api/Metadata/publication-status - Fetches all publication status
        /// </summary>
        public Task<List<AgeRatingDto>?> GetPublicationStatusAsync(string? libraryIds = null)
        {
            var path = string.IsNullOrEmpty(libraryIds) 
                ? "/api/Metadata/publication-status" 
                : $"/api/Metadata/publication-status?libraryIds={libraryIds}";
            return GetAsync<List<AgeRatingDto>>(path);
        }

        /// <summary>
        /// GET /api/Metadata/languages - Fetches all languages
        /// </summary>
        public Task<List<LanguageDto>?> GetLanguagesAsync(string? libraryIds = null)
        {
            var path = string.IsNullOrEmpty(libraryIds) 
                ? "/api/Metadata/languages" 
                : $"/api/Metadata/languages?libraryIds={libraryIds}";
            return GetAsync<List<LanguageDto>>(path);
        }

        /// <summary>
        /// GET /api/Metadata/people - Fetches people from the instance
        /// </summary>
        public Task<List<PersonDto>?> GetPeopleAsync(string? libraryIds = null)
        {
            var path = string.IsNullOrEmpty(libraryIds) 
                ? "/api/Metadata/people" 
                : $"/api/Metadata/people?libraryIds={libraryIds}";
            return GetAsync<List<PersonDto>>(path);
        }

        #endregion

        #region Want To Read - /api/want-to-read

        /// <summary>
        /// GET /api/want-to-read - Check if series is in want to read
        /// </summary>
        public Task<bool> IsInWantToReadAsync(int seriesId)
        {
            return GetAsync<bool>($"/api/want-to-read?seriesId={seriesId}");
        }

        /// <summary>
        /// POST /api/want-to-read/add-series - Add series to want to read
        /// </summary>
        public Task AddToWantToReadAsync(params int[] seriesIds)
        {
            return PostAsync("/api/want-to-read/add-series", new { seriesIds = seriesIds.ToList() });
        }

        /// <summary>
        /// POST /api/want-to-read/remove-series - Remove series from want to read
        /// </summary>
        public Task RemoveFromWantToReadAsync(params int[] seriesIds)
        {
            return PostAsync("/api/want-to-read/remove-series", new { seriesIds = seriesIds.ToList() });
        }

        #endregion

        #region Image - /api/Image

        /// <summary>
        /// Gets the URL for a series cover image
        /// </summary>
        public string GetSeriesCoverUrl(int seriesId, string? apiKey = null)
        {
            var path = $"/api/Image/series-cover?seriesId={seriesId}";
            if (!string.IsNullOrEmpty(apiKey))
            {
                path += $"&apiKey={apiKey}";
            }
            return $"{_baseUrl}{path}";
        }

        /// <summary>
        /// Gets the URL for a volume cover image
        /// </summary>
        public string GetVolumeCoverUrl(int volumeId, string? apiKey = null)
        {
            var path = $"/api/Image/volume-cover?volumeId={volumeId}";
            if (!string.IsNullOrEmpty(apiKey))
            {
                path += $"&apiKey={apiKey}";
            }
            return $"{_baseUrl}{path}";
        }

        /// <summary>
        /// Gets the URL for a chapter cover image
        /// </summary>
        public string GetChapterCoverUrl(int chapterId, string? apiKey = null)
        {
            var path = $"/api/Image/chapter-cover?chapterId={chapterId}";
            if (!string.IsNullOrEmpty(apiKey))
            {
                path += $"&apiKey={apiKey}";
            }
            return $"{_baseUrl}{path}";
        }

        /// <summary>
        /// Gets the URL for a library cover image
        /// </summary>
        public string GetLibraryCoverUrl(int libraryId, string? apiKey = null)
        {
            var path = $"/api/Image/library-cover?libraryId={libraryId}";
            if (!string.IsNullOrEmpty(apiKey))
            {
                path += $"&apiKey={apiKey}";
            }
            return $"{_baseUrl}{path}";
        }

        #endregion

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
