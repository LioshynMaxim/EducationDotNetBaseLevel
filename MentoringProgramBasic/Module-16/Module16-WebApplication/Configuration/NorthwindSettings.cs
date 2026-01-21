namespace Module16_WebApplication.Configuration;

public class NorthwindSettings
{
    public const string SectionName = "NorthwindSettings";
    
    public string ConnectionString { get; set; } = string.Empty;
    public int MaxProductsToShow { get; set; }
}
