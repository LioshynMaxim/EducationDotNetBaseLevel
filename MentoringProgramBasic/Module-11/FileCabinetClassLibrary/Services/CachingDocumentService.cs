using FileCabinetClassLibrary.Interfaces;
using FileCabinetClassLibrary.Models;

namespace FileCabinetClassLibrary.Services
{
    public class CachingDocumentService : ICachingService
    {
        private class CacheEntry
        {
            public object Value { get; set; }
            public DateTime? ExpirationTime { get; set; }
            public bool ShouldCache { get; set; }
        }

        private readonly Dictionary<string, CacheEntry> _cache = new();
        private readonly TimeSpan _defaultExpiration;
        private readonly Dictionary<string, TimeSpan?> _documentTypeCacheConfig;

        public CachingDocumentService(
            TimeSpan? defaultExpiration = null,
            Dictionary<string, TimeSpan?> documentTypeCacheConfig = null)
        {
            _defaultExpiration = defaultExpiration ?? TimeSpan.FromMinutes(5);
            _documentTypeCacheConfig = documentTypeCacheConfig ?? new Dictionary<string, TimeSpan?>();
        }

        public void SetValue(string key, object value)
        {
            var (expiration, shouldCache) = CalculateExpiration(value);
            
            if (!shouldCache)
            {
                return;
            }
            
            _cache[key] = new CacheEntry
            {
                Value = value,
                ExpirationTime = expiration,
                ShouldCache = true
            };
        }

        public bool TryGetValue(string key, out object value)
        {
            value = null;

            if (!_cache.TryGetValue(key, out var entry))
            {
                return false;
            }

            if (entry.ExpirationTime.HasValue && DateTime.UtcNow > entry.ExpirationTime.Value)
            {
                _cache.Remove(key);
                return false;
            }

            value = entry.Value;
            return true;
        }

        public void RemoveValue(string key)
        {
            _cache.Remove(key);
        }

        public void ClearCache()
        {
            _cache.Clear();
        }

        private (DateTime?, bool) CalculateExpiration(object value)
        {
            if (value == null)
            {
                return (DateTime.UtcNow.Add(_defaultExpiration), true);
            }

            var documentType = GetDocumentType(value);

            if (_documentTypeCacheConfig.TryGetValue(documentType, out var config))
            {
                if (config == TimeSpan.Zero)
                {
                    return (null, false);
                }

                if (config == null)
                {
                    return (DateTime.MaxValue, true);
                }

                return (DateTime.UtcNow.Add(config.Value), true);
            }

            return (DateTime.UtcNow.Add(_defaultExpiration), true);
        }

        private string GetDocumentType(object value)
        {
            return value switch
            {
                BookCard => "book",
                PatentCard => "patent",
                MagazineCard => "magazine",
                LocalizedBookCard => "localized_book",
                _ => "unknown"
            };
        }
    }
}
