using ConsoleAppXMLSerialization;
using System.Xml.Serialization;

public class Program
{
    private static void Main(string[] args)
    {
        var department = new Department
        {
            DepartmentName = "IT",
            Employees =
            [
                new() { EmpoyeeName = "Alice" },
                new() { EmpoyeeName = "Bob" }
            ]
        };

        var serializer = new XmlSerializer(typeof(Department));
        var filePath = Path.Combine(AppContext.BaseDirectory, "department.xml");

        using (var stream = File.Create(filePath))
        {
            serializer.Serialize(stream, department);
        }

        Department? deserializedDepartment;
        using (var stream = File.OpenRead(filePath))
        {
            deserializedDepartment = serializer.Deserialize(stream) as Department;
        }

        Console.WriteLine($"Deserialized Department: {deserializedDepartment!.DepartmentName}");
        foreach (var emp in deserializedDepartment.Employees)
        {
            Console.WriteLine($"Employee: {emp.EmpoyeeName}");
        }
    }
}