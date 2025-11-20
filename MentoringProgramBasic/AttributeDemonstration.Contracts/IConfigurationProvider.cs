namespace AttributeDemonstration.Contracts
{
    public interface IConfigurationProvider
    {
        string ReadSetting(string key);
        void WriteSetting(string key, string value);
        void Save();
    }
}