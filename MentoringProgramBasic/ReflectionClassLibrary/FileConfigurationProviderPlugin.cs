namespace ReflectionClassLibrary;

public class FileConfigurationProviderPlugin : IConfigurationProviderPlugin
{
    public string ProviderName => "File";

    public IConfigurationProvider CreateProvider() => new FileConfigurationProvider();
}
