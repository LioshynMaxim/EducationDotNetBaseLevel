namespace AttributeDemonstration;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Configuration Attribute Demo ===\n");

        var component = new MyConfigComponent();

        Console.WriteLine("--- Loading Settings ---");
        component.LoadSettings();

        Console.WriteLine($"App Name: {component.AppName}");
        Console.WriteLine($"Max Connections: {component.MaxConnections}");
        Console.WriteLine($"Timeout: {component.Timeout}");
        Console.WriteLine($"Version: {component.Version}");

        Console.WriteLine("\n--- Modifying Settings ---");
        component.AppName = "Updated Application";
        component.MaxConnections = 150;
        component.Timeout = TimeSpan.FromMinutes(10);
        component.Version = 2.5f;

        Console.WriteLine("\n--- Saving Settings ---");
        component.SaveSettings();

        Console.WriteLine("\n--- Creating New Instance and Loading ---");
        var component2 = new MyConfigComponent();
        component2.LoadSettings();

        Console.WriteLine($"App Name: {component2.AppName}");
        Console.WriteLine($"Max Connections: {component2.MaxConnections}");
        Console.WriteLine($"Timeout: {component2.Timeout}");
        Console.WriteLine($"Version: {component2.Version}");

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}