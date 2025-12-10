using ReflectionClassLibrary;

namespace AttributeDemonstration;

public class MyConfigComponent : ConfigurationComponentBase
{
    [ConfigurationItem("ApplicationName", ConfigurationProviderType.ConfigurationManager)]
    public string AppName { get; set; } = "My Application";

    [ConfigurationItem("MaxConnections", ConfigurationProviderType.File)]
    public int MaxConnections { get; set; } = 100;

    [ConfigurationItem("RequestTimeout", ConfigurationProviderType.File)]
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(5);

    [ConfigurationItem("Version", ConfigurationProviderType.ConfigurationManager)]
    public float Version { get; set; } = 1.0f;

    public MyConfigComponent()
    {
    }
}