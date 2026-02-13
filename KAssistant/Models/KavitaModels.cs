using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KAssistant.Models
{
    // Authentication Models
    public class LoginRequest
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthenticationResponse
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class UserDto
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("token")]
        public string? Token { get; set; }

        [JsonPropertyName("refreshToken")]
        public string? RefreshToken { get; set; }

        [JsonPropertyName("apiKey")]
        public string? ApiKey { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

    // Server Info Models
    public class ServerInfoResponse
    {
        [JsonPropertyName("kavitaVersion")]
        public string KavitaVersion { get; set; } = string.Empty;

        [JsonPropertyName("isServerAccessible")]
        public bool IsServerAccessible { get; set; }

        [JsonPropertyName("os")]
        public string OS { get; set; } = string.Empty;

        [JsonPropertyName("dotNetVersion")]
        public string DotNetVersion { get; set; } = string.Empty;
    }

    // Library Models
    public class Library
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("folderWatching")]
        public bool FolderWatching { get; set; }

        [JsonPropertyName("lastScanned")]
        public DateTime LastScanned { get; set; }
    }

    // Series Models
    public class Series
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("originalName")]
        public string? OriginalName { get; set; }

        [JsonPropertyName("localizedName")]
        public string? LocalizedName { get; set; }

        [JsonPropertyName("sortName")]
        public string? SortName { get; set; }

        [JsonPropertyName("summary")]
        public string? Summary { get; set; }

        [JsonPropertyName("created")]
        public DateTime Created { get; set; }

        [JsonPropertyName("lastModified")]
        public DateTime LastModified { get; set; }

        [JsonPropertyName("coverImageLocked")]
        public bool CoverImageLocked { get; set; }

        [JsonPropertyName("libraryId")]
        public int LibraryId { get; set; }

        [JsonPropertyName("pagesRead")]
        public int PagesRead { get; set; }
        
        [JsonPropertyName("pages")]
        public int Pages { get; set; }
        
        [JsonPropertyName("wordCount")]
        public long WordCount { get; set; }
        
        [JsonPropertyName("minHoursToRead")]
        public int MinHoursToRead { get; set; }
        
        [JsonPropertyName("maxHoursToRead")]
        public int MaxHoursToRead { get; set; }
        
        [JsonPropertyName("avgHoursToRead")]
        public int AvgHoursToRead { get; set; }
        
        [JsonPropertyName("format")]
        public int Format { get; set; }
        
        [JsonPropertyName("folderPath")]
        public string? FolderPath { get; set; }
        
        [JsonPropertyName("lastChapterAdded")]
        public DateTime LastChapterAdded { get; set; }
        
        [JsonPropertyName("latestReadDate")]
        public DateTime? LatestReadDate { get; set; }
        
        [JsonPropertyName("userRating")]
        public decimal? UserRating { get; set; }
        
        [JsonPropertyName("userReview")]
        public string? UserReview { get; set; }
        
        [JsonPropertyName("coverImage")]
        public string? CoverImage { get; set; }
        
        [JsonPropertyName("lowestFolderPath")]
        public string? LowestFolderPath { get; set; }
    }

    // Series Detail DTO
    public class SeriesDetailDto
    {
        [JsonPropertyName("specials")]
        public List<ChapterDto>? Specials { get; set; }

        [JsonPropertyName("chapters")]
        public List<ChapterDto>? Chapters { get; set; }

        [JsonPropertyName("volumes")]
        public List<VolumeDto>? Volumes { get; set; }

        [JsonPropertyName("storylineChapters")]
        public List<ChapterDto>? StorylineChapters { get; set; }

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        [JsonPropertyName("unreadCount")]
        public int UnreadCount { get; set; }
    }

    // Pagination Models
    public class PaginatedResult<T>
    {
        [JsonPropertyName("currentPage")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("result")]
        public List<T> Result { get; set; } = new();
    }

    // User Models
    public class User
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("isAdmin")]
        public bool IsAdmin { get; set; }

        [JsonPropertyName("isPending")]
        public bool IsPending { get; set; }

        [JsonPropertyName("created")]
        public DateTime Created { get; set; }

        [JsonPropertyName("lastActive")]
        public DateTime LastActive { get; set; }
    }

    // Collection Models
    public class Collection
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("summary")]
        public string Summary { get; set; } = string.Empty;

        [JsonPropertyName("promoted")]
        public bool Promoted { get; set; }

        [JsonPropertyName("seriesCount")]
        public int SeriesCount { get; set; }
    }

    // Reading List Models
    public class ReadingList
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("summary")]
        public string Summary { get; set; } = string.Empty;

        [JsonPropertyName("promoted")]
        public bool Promoted { get; set; }

        [JsonPropertyName("itemCount")]
        public int ItemCount { get; set; }

        [JsonPropertyName("ageRating")]
        public string AgeRating { get; set; } = string.Empty;
    }

    public class ReadingListDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("summary")]
        public string Summary { get; set; } = string.Empty;

        [JsonPropertyName("promoted")]
        public bool Promoted { get; set; }

        [JsonPropertyName("coverImageLocked")]
        public bool CoverImageLocked { get; set; }

        [JsonPropertyName("itemCount")]
        public int ItemCount { get; set; }
    }

    public class ReadingListItemDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("order")]
        public int Order { get; set; }

        [JsonPropertyName("chapterId")]
        public int ChapterId { get; set; }

        [JsonPropertyName("seriesId")]
        public int SeriesId { get; set; }

        [JsonPropertyName("volumeId")]
        public int VolumeId { get; set; }

        [JsonPropertyName("seriesName")]
        public string SeriesName { get; set; } = string.Empty;

        [JsonPropertyName("chapterNumber")]
        public string ChapterNumber { get; set; } = string.Empty;

        [JsonPropertyName("volumeNumber")]
        public int VolumeNumber { get; set; }

        [JsonPropertyName("pagesRead")]
        public int PagesRead { get; set; }

        [JsonPropertyName("pagesTotal")]
        public int PagesTotal { get; set; }
    }

    // Stats Models
    public class ServerStats
    {
        [JsonPropertyName("totalFiles")]
        public int TotalFiles { get; set; }

        [JsonPropertyName("totalSeries")]
        public int TotalSeries { get; set; }

        [JsonPropertyName("totalVolumes")]
        public int TotalVolumes { get; set; }

        [JsonPropertyName("totalChapters")]
        public int TotalChapters { get; set; }

        [JsonPropertyName("totalSize")]
        public long TotalSize { get; set; }
    }

    public class UserStats
    {
        [JsonPropertyName("totalPagesRead")]
        public int TotalPagesRead { get; set; }

        [JsonPropertyName("totalWordsRead")]
        public long TotalWordsRead { get; set; }

        [JsonPropertyName("timeSpentReading")]
        public long TimeSpentReading { get; set; }

        [JsonPropertyName("chaptersRead")]
        public int ChaptersRead { get; set; }
    }

    // Volume and Chapter Models
    public class Volume
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("pages")]
        public int Pages { get; set; }

        [JsonPropertyName("seriesId")]
        public int SeriesId { get; set; }
    }

    public class VolumeDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("minNumber")]
        public float MinNumber { get; set; }

        [JsonPropertyName("maxNumber")]
        public float MaxNumber { get; set; }

        [JsonPropertyName("pages")]
        public int Pages { get; set; }

        [JsonPropertyName("pagesRead")]
        public int PagesRead { get; set; }

        [JsonPropertyName("wordCount")]
        public long WordCount { get; set; }

        [JsonPropertyName("seriesId")]
        public int SeriesId { get; set; }

        [JsonPropertyName("chapters")]
        public List<ChapterDto>? Chapters { get; set; }

        [JsonPropertyName("coverImage")]
        public string? CoverImage { get; set; }

        [JsonPropertyName("coverImageLocked")]
        public bool CoverImageLocked { get; set; }

        [JsonPropertyName("lastModified")]
        public DateTime LastModified { get; set; }

        [JsonPropertyName("created")]
        public DateTime Created { get; set; }
    }

    public class Chapter
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("number")]
        public string Number { get; set; } = string.Empty;

        [JsonPropertyName("pages")]
        public int Pages { get; set; }

        [JsonPropertyName("volumeId")]
        public int VolumeId { get; set; }
    }

    public class ChapterDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("minNumber")]
        public string MinNumber { get; set; } = string.Empty;

        [JsonPropertyName("maxNumber")]
        public string MaxNumber { get; set; } = string.Empty;

        [JsonPropertyName("range")]
        public string Range { get; set; } = string.Empty;

        [JsonPropertyName("pages")]
        public int Pages { get; set; }

        [JsonPropertyName("pagesRead")]
        public int PagesRead { get; set; }

        [JsonPropertyName("wordCount")]
        public long WordCount { get; set; }

        [JsonPropertyName("volumeId")]
        public int VolumeId { get; set; }

        [JsonPropertyName("isSpecial")]
        public bool IsSpecial { get; set; }

        [JsonPropertyName("specialIndex")]
        public int SpecialIndex { get; set; }

        [JsonPropertyName("sortOrder")]
        public float SortOrder { get; set; }

        [JsonPropertyName("releaseDate")]
        public DateTime? ReleaseDate { get; set; }

        [JsonPropertyName("coverImage")]
        public string? CoverImage { get; set; }

        [JsonPropertyName("coverImageLocked")]
        public bool CoverImageLocked { get; set; }

        [JsonPropertyName("lastModified")]
        public DateTime LastModified { get; set; }

        [JsonPropertyName("created")]
        public DateTime Created { get; set; }

        [JsonPropertyName("files")]
        public List<MangaFileDto>? Files { get; set; }

        [JsonPropertyName("summary")]
        public string? Summary { get; set; }

        [JsonPropertyName("ageRating")]
        public int AgeRating { get; set; }

        [JsonPropertyName("titleName")]
        public string TitleName { get; set; } = string.Empty;
    }

    public class MangaFileDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("filePath")]
        public string FilePath { get; set; } = string.Empty;

        [JsonPropertyName("pages")]
        public int Pages { get; set; }

        [JsonPropertyName("format")]
        public int Format { get; set; }

        [JsonPropertyName("bytes")]
        public long Bytes { get; set; }
    }

    public class ChapterInfoDto
    {
        [JsonPropertyName("chapterNumber")]
        public string ChapterNumber { get; set; } = string.Empty;

        [JsonPropertyName("volumeNumber")]
        public string VolumeNumber { get; set; } = string.Empty;

        [JsonPropertyName("volumeId")]
        public int VolumeId { get; set; }

        [JsonPropertyName("seriesName")]
        public string SeriesName { get; set; } = string.Empty;

        [JsonPropertyName("seriesFormat")]
        public int SeriesFormat { get; set; }

        [JsonPropertyName("seriesId")]
        public int SeriesId { get; set; }

        [JsonPropertyName("libraryId")]
        public int LibraryId { get; set; }

        [JsonPropertyName("libraryType")]
        public int LibraryType { get; set; }

        [JsonPropertyName("pages")]
        public int Pages { get; set; }

        [JsonPropertyName("fileName")]
        public string FileName { get; set; } = string.Empty;

        [JsonPropertyName("isSpecial")]
        public bool IsSpecial { get; set; }

        [JsonPropertyName("subtitle")]
        public string Subtitle { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
    }

    // Metadata Models
    public class SeriesMetadata
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("seriesId")]
        public int SeriesId { get; set; }

        [JsonPropertyName("summary")]
        public string? Summary { get; set; }

        [JsonPropertyName("genres")]
        public List<GenreTag>? Genres { get; set; }

        [JsonPropertyName("tags")]
        public List<Tag>? Tags { get; set; }

        [JsonPropertyName("ageRating")]
        public int AgeRating { get; set; }

        [JsonPropertyName("publicationStatus")]
        public int PublicationStatus { get; set; }

        [JsonPropertyName("language")]
        public string? Language { get; set; }
        
        [JsonPropertyName("releaseYear")]
        public int ReleaseYear { get; set; }
        
        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }
        
        [JsonPropertyName("maxCount")]
        public int MaxCount { get; set; }
        
        [JsonPropertyName("writers")]
        public List<Person>? Writers { get; set; }
        
        [JsonPropertyName("pencillers")]
        public List<Person>? Pencillers { get; set; }
        
        [JsonPropertyName("inkers")]
        public List<Person>? Inkers { get; set; }
        
        [JsonPropertyName("colorists")]
        public List<Person>? Colorists { get; set; }
        
        [JsonPropertyName("letterers")]
        public List<Person>? Letterers { get; set; }
        
        [JsonPropertyName("coverArtists")]
        public List<Person>? CoverArtists { get; set; }
        
        [JsonPropertyName("editors")]
        public List<Person>? Editors { get; set; }
        
        [JsonPropertyName("publishers")]
        public List<Person>? Publishers { get; set; }
        
        [JsonPropertyName("characters")]
        public List<Person>? Characters { get; set; }
        
        [JsonPropertyName("translators")]
        public List<Person>? Translators { get; set; }
    }

    public class UpdateSeriesMetadataDto
    {
        [JsonPropertyName("seriesMetadata")]
        public SeriesMetadata SeriesMetadata { get; set; } = new();
    }
    
    public class GenreTag
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
    }

    public class GenreTagDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
    }
    
    public class Tag
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
    }

    public class TagDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
    }
    
    public class Person
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonPropertyName("role")]
        public int Role { get; set; }
    }

    public class PersonDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonPropertyName("role")]
        public int Role { get; set; }

        [JsonPropertyName("coverImage")]
        public string? CoverImage { get; set; }

        [JsonPropertyName("coverImageLocked")]
        public bool CoverImageLocked { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    public class AgeRatingDto
    {
        [JsonPropertyName("value")]
        public int Value { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
    }

    public class LanguageDto
    {
        [JsonPropertyName("isoCode")]
        public string IsoCode { get; set; } = string.Empty;
        
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
    }

    // Test Result Model
    public class ApiTestResult
    {
        public string TestName { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }
        public TimeSpan Duration { get; set; }
    }

    // Request Models for POST endpoints
    public class SeriesFilterDto
    {
        [JsonPropertyName("libraryIds")]
        public List<int> LibraryIds { get; set; } = new();

        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }
    }

    public class RecentlyAddedFilterDto
    {
        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }
    }

    public class OnDeckFilterDto
    {
        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("libraryId")]
        public int LibraryId { get; set; }
    }

    public class SearchDto
    {
        [JsonPropertyName("queryString")]
        public string QueryString { get; set; } = string.Empty;
    }

    // V2 Filter DTO based on OpenAPI spec
    public class FilterV2Dto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("statements")]
        public List<FilterStatementDto>? Statements { get; set; }

        [JsonPropertyName("combination")]
        public int Combination { get; set; }

        [JsonPropertyName("sortOptions")]
        public SortOptionsDto? SortOptions { get; set; }

        [JsonPropertyName("limitTo")]
        public int LimitTo { get; set; }
    }

    public class FilterStatementDto
    {
        [JsonPropertyName("comparison")]
        public int Comparison { get; set; }

        [JsonPropertyName("field")]
        public int Field { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;
    }

    public class SortOptionsDto
    {
        [JsonPropertyName("sortField")]
        public int SortField { get; set; }

        [JsonPropertyName("isAscending")]
        public bool IsAscending { get; set; } = true;
    }

    // Search Result Models
    public class SearchResultGroupDto
    {
        [JsonPropertyName("series")]
        public List<SearchResultDto>? Series { get; set; }

        [JsonPropertyName("collections")]
        public List<SearchResultDto>? Collections { get; set; }

        [JsonPropertyName("readingLists")]
        public List<SearchResultDto>? ReadingLists { get; set; }

        [JsonPropertyName("persons")]
        public List<SearchResultDto>? Persons { get; set; }

        [JsonPropertyName("genres")]
        public List<SearchResultDto>? Genres { get; set; }

        [JsonPropertyName("tags")]
        public List<SearchResultDto>? Tags { get; set; }

        [JsonPropertyName("files")]
        public List<SearchResultDto>? Files { get; set; }

        [JsonPropertyName("chapters")]
        public List<SearchResultDto>? Chapters { get; set; }

        [JsonPropertyName("libraries")]
        public List<SearchResultDto>? Libraries { get; set; }
    }

    public class SearchResultDto
    {
        [JsonPropertyName("seriesId")]
        public int SeriesId { get; set; }

        [JsonPropertyName("libraryId")]
        public int LibraryId { get; set; }

        [JsonPropertyName("libraryName")]
        public string LibraryName { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("originalName")]
        public string? OriginalName { get; set; }

        [JsonPropertyName("sortName")]
        public string? SortName { get; set; }

        [JsonPropertyName("localizedName")]
        public string? LocalizedName { get; set; }

        [JsonPropertyName("format")]
        public int Format { get; set; }
    }

    // Progress Models
    public class ProgressDto
    {
        [JsonPropertyName("volumeId")]
        public int VolumeId { get; set; }

        [JsonPropertyName("chapterId")]
        public int ChapterId { get; set; }

        [JsonPropertyName("pageNum")]
        public int PageNum { get; set; }

        [JsonPropertyName("seriesId")]
        public int SeriesId { get; set; }

        [JsonPropertyName("libraryId")]
        public int LibraryId { get; set; }

        [JsonPropertyName("bookScrollId")]
        public string? BookScrollId { get; set; }

        [JsonPropertyName("lastModified")]
        public DateTime LastModified { get; set; }

        [JsonPropertyName("lastModifiedUtc")]
        public DateTime LastModifiedUtc { get; set; }
    }

    public class MarkReadDto
    {
        [JsonPropertyName("seriesId")]
        public int SeriesId { get; set; }
    }

    // Related Series
    public class RelatedSeriesDto
    {
        [JsonPropertyName("sequels")]
        public List<Series>? Sequels { get; set; }

        [JsonPropertyName("prequels")]
        public List<Series>? Prequels { get; set; }

        [JsonPropertyName("spinOffs")]
        public List<Series>? SpinOffs { get; set; }

        [JsonPropertyName("adaptations")]
        public List<Series>? Adaptations { get; set; }

        [JsonPropertyName("sideStories")]
        public List<Series>? SideStories { get; set; }

        [JsonPropertyName("characters")]
        public List<Series>? Characters { get; set; }

        [JsonPropertyName("contains")]
        public List<Series>? Contains { get; set; }

        [JsonPropertyName("others")]
        public List<Series>? Others { get; set; }

        [JsonPropertyName("alternativeSettings")]
        public List<Series>? AlternativeSettings { get; set; }

        [JsonPropertyName("alternativeVersions")]
        public List<Series>? AlternativeVersions { get; set; }

        [JsonPropertyName("doujinshis")]
        public List<Series>? Doujinshis { get; set; }

        [JsonPropertyName("parent")]
        public List<Series>? Parent { get; set; }

        [JsonPropertyName("editions")]
        public List<Series>? Editions { get; set; }

        [JsonPropertyName("annuals")]
        public List<Series>? Annuals { get; set; }
    }

    // Hour Estimate
    public class HourEstimateRangeDto
    {
        [JsonPropertyName("minHours")]
        public int MinHours { get; set; }

        [JsonPropertyName("maxHours")]
        public int MaxHours { get; set; }

        [JsonPropertyName("avgHours")]
        public int AvgHours { get; set; }
    }

    // User Preferences
    public class UserPreferencesDto
    {
        [JsonPropertyName("readingDirection")]
        public int ReadingDirection { get; set; }

        [JsonPropertyName("scalingOption")]
        public int ScalingOption { get; set; }

        [JsonPropertyName("pageSplitOption")]
        public int PageSplitOption { get; set; }

        [JsonPropertyName("autoCloseMenu")]
        public bool AutoCloseMenu { get; set; }

        [JsonPropertyName("showScreenHints")]
        public bool ShowScreenHints { get; set; }

        [JsonPropertyName("readerMode")]
        public int ReaderMode { get; set; }

        [JsonPropertyName("layoutMode")]
        public int LayoutMode { get; set; }

        [JsonPropertyName("emulateBook")]
        public bool EmulateBook { get; set; }

        [JsonPropertyName("backgroundColor")]
        public string BackgroundColor { get; set; } = string.Empty;

        [JsonPropertyName("swipeToPaginate")]
        public bool SwipeToPaginate { get; set; }

        [JsonPropertyName("bookReaderMargin")]
        public int BookReaderMargin { get; set; }

        [JsonPropertyName("bookReaderLineSpacing")]
        public int BookReaderLineSpacing { get; set; }

        [JsonPropertyName("bookReaderFontSize")]
        public int BookReaderFontSize { get; set; }

        [JsonPropertyName("bookReaderFontFamily")]
        public string BookReaderFontFamily { get; set; } = string.Empty;

        [JsonPropertyName("bookReaderTapToPaginate")]
        public bool BookReaderTapToPaginate { get; set; }

        [JsonPropertyName("bookReaderReadingDirection")]
        public int BookReaderReadingDirection { get; set; }

        [JsonPropertyName("bookReaderWritingStyle")]
        public int BookReaderWritingStyle { get; set; }

        [JsonPropertyName("theme")]
        public int Theme { get; set; }

        [JsonPropertyName("blurUnreadSummaries")]
        public bool BlurUnreadSummaries { get; set; }

        [JsonPropertyName("promptForDownloadSize")]
        public bool PromptForDownloadSize { get; set; }

        [JsonPropertyName("noTransitions")]
        public bool NoTransitions { get; set; }

        [JsonPropertyName("collapseSeriesRelationships")]
        public bool CollapseSeriesRelationships { get; set; }

        [JsonPropertyName("shareReviews")]
        public bool ShareReviews { get; set; }

        [JsonPropertyName("locale")]
        public string Locale { get; set; } = string.Empty;
    }

    // Server Job Models
    public class JobDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("cron")]
        public string Cron { get; set; } = string.Empty;

        [JsonPropertyName("lastExecution")]
        public DateTime? LastExecution { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateNotificationDto
    {
        [JsonPropertyName("currentVersion")]
        public string CurrentVersion { get; set; } = string.Empty;

        [JsonPropertyName("updateVersion")]
        public string UpdateVersion { get; set; } = string.Empty;

        [JsonPropertyName("updateBody")]
        public string UpdateBody { get; set; } = string.Empty;

        [JsonPropertyName("updateTitle")]
        public string UpdateTitle { get; set; } = string.Empty;

        [JsonPropertyName("updateUrl")]
        public string UpdateUrl { get; set; } = string.Empty;

        [JsonPropertyName("publishDate")]
        public DateTime PublishDate { get; set; }

        [JsonPropertyName("isPrerelease")]
        public bool IsPrerelease { get; set; }

        [JsonPropertyName("isDocker")]
        public bool IsDocker { get; set; }

        [JsonPropertyName("isReleaseEqual")]
        public bool IsReleaseEqual { get; set; }

        [JsonPropertyName("isReleaseNewer")]
        public bool IsReleaseNewer { get; set; }
    }

    // Refresh Series Request
    public class RefreshSeriesDto
    {
        [JsonPropertyName("libraryId")]
        public int LibraryId { get; set; }

        [JsonPropertyName("seriesId")]
        public int SeriesId { get; set; }

        [JsonPropertyName("forceUpdate")]
        public bool ForceUpdate { get; set; }

        [JsonPropertyName("forceColorscape")]
        public bool ForceColorscape { get; set; }
    }
}
