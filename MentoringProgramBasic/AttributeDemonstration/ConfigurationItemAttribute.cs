using ReflectionClassLibrary;

namespace AttributeDemonstration;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ConfigurationItemAttribute : Attribute
{
    public string SettingName { get; }
    public ConfigurationProviderType ProviderType { get; }

    public ConfigurationItemAttribute(string settingName, ConfigurationProviderType providerType)
    {
        if (string.IsNullOrWhiteSpace(settingName))
        {
            throw new ArgumentException("Setting name cannot be null or empty", nameof(settingName));
        }

        SettingName = settingName;
        ProviderType = providerType;
    }
}
