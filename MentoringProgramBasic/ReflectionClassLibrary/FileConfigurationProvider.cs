namespace ReflectionClassLibrary;


internal class FileConfigurationProvider : IConfigurationProvider
{
    private readonly string _filePath;
    private readonly Dictionary<string, string> _settings = new(StringComparer.OrdinalIgnoreCase);

    public FileConfigurationProvider(string filePath = "config.txt")
    {
        _filePath = filePath;
        LoadFromFile();
    }

    private void LoadFromFile()
    {
        if (!File.Exists(_filePath))
        {
            return;
        }

        try
        {
            var lines = File.ReadAllLines(_filePath);
            foreach (var line in lines.Where(l => !string.IsNullOrWhiteSpace(l)))
            {
                var parts = line.Split(['='], 2);
                if (parts.Length == 2)
                {
                    _settings[parts[0].Trim()] = parts[1].Trim();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading configuration file: {ex.Message}");
        }
    }

    public string? ReadSetting(string key) => 
        _settings.TryGetValue(key, out var value) 
        ? value 
        : null;

    public void WriteSetting(string key, string value) => _settings[key] = value;

    public void Save()
    {
        try
        {
            var lines = _settings.Select(kvp => $"{kvp.Key}={kvp.Value}");
            File.WriteAllLines(_filePath, lines);
            Console.WriteLine($"Settings saved to {_filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving configuration file: {ex.Message}");
        }
    }
}
