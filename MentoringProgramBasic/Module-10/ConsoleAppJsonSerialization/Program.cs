using ConsoleAppJsonSerialization;
using System.Text.Json;

public class Program
{
    private static void Main(string[] args)
    {
        var department = new Department
        {
            DepartmentName = "IT",
            Employees =
            [
                new Employee { EmpoyeeName = "Alice" },
                new Employee { EmpoyeeName = "Bob" }
            ]
        };

        var filePath = "department.json";

        var json = JsonSerializer.Serialize(department, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);

        var jsonFromFile = File.ReadAllText(filePath);
        var deserializedDepartment = JsonSerializer.Deserialize<Department>(jsonFromFile);

        Console.WriteLine($"Deserialized Department: {deserializedDepartment!.DepartmentName}");
        foreach (var emp in deserializedDepartment.Employees)
        {
            Console.WriteLine($"Employee: {emp.EmpoyeeName}");
        }
    }
}