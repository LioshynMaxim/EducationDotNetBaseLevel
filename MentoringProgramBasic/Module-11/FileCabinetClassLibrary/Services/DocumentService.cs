using FileCabinetClassLibrary.Interfaces;
using FileCabinetClassLibrary.Models;

namespace FileCabinetClassLibrary.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IStorageService _storageService;
        private readonly ICachingService _cachingService;

        public DocumentService(
            IStorageService storageService, 
            ICachingService cachingService) 
        { 
            _storageService = storageService;
            _cachingService = cachingService;
        }

        public IEnumerable<BaseCard> SearchById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return [];
            }

            var cacheKey = id.ToLowerInvariant();

            if (_cachingService.TryGetValue(cacheKey, out var cachedValue))
            {
                return [cachedValue as BaseCard];
            }

            var allCards = _storageService.GetAllCards();
            var result = allCards.FirstOrDefault(c => c.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

            if (result != null)
            {
                _cachingService.SetValue(cacheKey, result);
            }

            return result != null ? [result] : [];
        }
    }
}
