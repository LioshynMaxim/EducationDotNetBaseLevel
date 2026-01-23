using System.Runtime.Serialization.Formatters.Binary;

namespace ConsoleAppBinarySerialization
{
    class Program
    {
        static void Main(string[] args)
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

            var filePath = "department.bin";

            // Serialize
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(fs, department);
            }

            // Deserialize
            Department deserializedDepartment;
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                var formatter = new BinaryFormatter();
                deserializedDepartment = (Department)formatter.Deserialize(fs);
            }

            Console.WriteLine($"Deserialized Department: {deserializedDepartment.DepartmentName}");
            foreach (var emp in deserializedDepartment.Employees)
            {
                Console.WriteLine($"Employee: {emp.EmpoyeeName}");
            }
        }
    }
}
