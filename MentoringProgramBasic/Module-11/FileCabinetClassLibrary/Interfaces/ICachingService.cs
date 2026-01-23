using FileCabinetClassLibrary.Models;

namespace FileCabinetClassLibrary.Interfaces;

public interface ICachingService
{
    void SetValue(string key, object value);
    bool TryGetValue(string key, out object value);
    void RemoveValue(string key);
    void ClearCache();
}
