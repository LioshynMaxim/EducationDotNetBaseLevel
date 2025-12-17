using FileCabinetClassLibrary.Interfaces;
using FileCabinetClassLibrary.Models;
using System.Text.Json;

namespace FileCabinetClassLibrary.Services
{
    public class StorageJsonService : IStorageService
    {
        private readonly List<BaseCard> _cards = [];
        private readonly JsonSerializerOptions _jsonOptions;

        public StorageJsonService()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public void AddCard(BaseCard card) => _cards.Add(card);

        public void DeleteCard(string id)
        {
            var card = _cards.FirstOrDefault(c => c.Id == id);
            if (card != null)
            {
                _cards.Remove(card);
            }
        }

        public IEnumerable<BaseCard> GetAllCards() => _cards.AsReadOnly();

        public BaseCard GetCardById(string id) => _cards.FirstOrDefault(c => c.Id == id);

        public BaseCard GetCardByPredicate(Func<BaseCard, bool> predicate) => _cards.FirstOrDefault(predicate);

        public void LoadFromStorage(string filePath)
        {
            try
            {
                var jsonFromFile = File.ReadAllText(filePath);
                var deserializedCards = JsonSerializer.Deserialize<List<BaseCard>>(jsonFromFile, _jsonOptions);
                if (deserializedCards != null)
                {
                    _cards.Clear();
                    _cards.AddRange(deserializedCards);
                }
            }
            catch (FileNotFoundException)
            {
                // File does not exist, skip loading
            }
            catch (DirectoryNotFoundException)
            {
                // Directory does not exist, skip loading
            }
        }

        public void SaveToStorage(string filePath)
        {
            var json = JsonSerializer.Serialize(_cards, _jsonOptions);
            File.WriteAllText(filePath, json);
        }

        public void UpdateCard(BaseCard card)
        {
            var cardForUpdate = _cards.FirstOrDefault(c => c.Id == card.Id);
            if (cardForUpdate != null)
            {
                _cards.Remove(cardForUpdate);
                _cards.Add(card);
            }
        }
    }
}
