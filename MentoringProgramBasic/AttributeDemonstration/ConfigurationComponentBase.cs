using ReflectionClassLibrary;
using System.Globalization;
using System.Reflection;

namespace AttributeDemonstration;

public abstract class ConfigurationComponentBase
{
    private readonly Dictionary<ConfigurationProviderType, IConfigurationProvider> _providers;

    protected ConfigurationComponentBase()
    {
        _providers = new Dictionary<ConfigurationProviderType, IConfigurationProvider>();
        LoadPlugins();
    }

    private void LoadPlugins()
    {
        try
        {
            var assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReflectionClassLibrary.dll");
            
            if (!File.Exists(assemblyPath))
            {
                Console.WriteLine($"Plugin assembly not found at: {assemblyPath}");
                return;
            }

            var assembly = Assembly.LoadFrom(assemblyPath);
            Console.WriteLine($"Loaded assembly: {assembly.FullName}");

            var pluginTypes = assembly.GetTypes()
                .Where(t => typeof(IConfigurationProviderPlugin).IsAssignableFrom(t) 
                            && !t.IsInterface 
                            && !t.IsAbstract);

            foreach (var pluginType in pluginTypes)
            {
                if (Activator.CreateInstance(pluginType) is not IConfigurationProviderPlugin plugin)
                {
                    Console.WriteLine("Plugin is null.");
                    continue;
                }

                var provider = plugin.CreateProvider();
                
                if (Enum.TryParse<ConfigurationProviderType>(plugin.ProviderName, out var providerType))
                {
                    _providers[providerType] = provider;
                    Console.WriteLine($"Registered plugin: {plugin.ProviderName} ({pluginType.Name})");
                }
                else
                {
                    Console.WriteLine($"Warning: Unknown provider type '{plugin.ProviderName}' from {pluginType.Name}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading plugins: {ex.Message}");
        }
    }

    public void LoadSettings()
    {
        var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var attribute = property.GetCustomAttribute<ConfigurationItemAttribute>();
            if (attribute == null)
            {
                continue;
            }

            if (!property.CanWrite)
            {
                Console.WriteLine($"Warning: Property {property.Name} is read-only and cannot be loaded.");
                continue;
            }

            if (!_providers.TryGetValue(attribute.ProviderType, out var provider))
            {
                Console.WriteLine($"Provider '{attribute.ProviderType}' not found for property {property.Name}.");
                continue;
            }

            var stringValue = provider.ReadSetting(attribute.SettingName);

            if (stringValue == null)
            {
                Console.WriteLine($"Setting '{attribute.SettingName}' not found in {attribute.ProviderType} provider.");
                continue;
            }

            try
            {
                var convertedValue = ConvertFromString(stringValue, property.PropertyType);
                property.SetValue(this, convertedValue);
                Console.WriteLine($"Loaded: {property.Name} = {convertedValue} (from {attribute.ProviderType})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading {property.Name}: {ex.Message}");
            }
        }
    }

    public void SaveSettings()
    {
        var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var attribute = property.GetCustomAttribute<ConfigurationItemAttribute>();
            if (attribute == null)
            {
                continue;
            }

            if (!property.CanRead)
            {
                Console.WriteLine($"Warning: Property {property.Name} is write-only and cannot be saved.");
                continue;
            }

            if (!_providers.TryGetValue(attribute.ProviderType, out var provider))
            {
                Console.WriteLine($"Provider '{attribute.ProviderType}' not found for property {property.Name}.");
                continue;
            }

            try
            {
                var value = property.GetValue(this);
                var stringValue = ConvertToString(value ?? string.Empty);
                
                provider.WriteSetting(attribute.SettingName, stringValue);
                Console.WriteLine($"Saved: {property.Name} = {stringValue} (to {attribute.ProviderType})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving {property.Name}: {ex.Message}");
            }
        }

        foreach (var provider in _providers.Values)
        {
            provider.Save();
        }
    }

    private static object ConvertFromString(string value, Type targetType)
    {
        if (targetType == typeof(string))
        {
            return value;
        }
        if (targetType == typeof(int))
        {
            return int.Parse(value, CultureInfo.InvariantCulture);
        }
        if (targetType == typeof(float))
        {
            return float.Parse(value, CultureInfo.InvariantCulture);
        }
        if (targetType == typeof(TimeSpan))
        {
            return TimeSpan.Parse(value, CultureInfo.InvariantCulture);
        }

        throw new NotSupportedException($"Type {targetType.Name} is not supported.");
    }

    private static string ConvertToString(object value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        return value is IFormattable formattable 
            ? formattable.ToString(null, CultureInfo.InvariantCulture) 
            : value.ToString();
    }
}