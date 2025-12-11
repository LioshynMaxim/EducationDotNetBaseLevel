namespace ReflectionClassLibrary;

public interface IConfigurationProviderPlugin
{
    string ProviderName { get; }
    IConfigurationProvider CreateProvider();
}