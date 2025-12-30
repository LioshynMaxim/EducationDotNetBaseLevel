using FileCabinetClassLibrary.Models;

namespace FileCabinetClassLibrary.Interfaces
{
    public interface IStorageService
    {
        IEnumerable<BaseCard> GetAllCards();
        BaseCard GetCardById(string id);
        BaseCard GetCardByPredicate(Func<BaseCard, bool> predicate);
        void AddCard(BaseCard card);
        void UpdateCard(BaseCard card);
        void DeleteCard(string id);
        void SaveToStorage(string filePath);
        void LoadFromStorage(string filePath);
    }
}
