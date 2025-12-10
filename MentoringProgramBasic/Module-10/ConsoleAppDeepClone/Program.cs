using ConsoleAppDeepClone;
using System.Text.Json;

var original = new Department
{
    DepartmentName = "IT",
    Employees =
    [
        new Employee { EmployeeName = "Alice" },
        new Employee { EmployeeName = "Bob" }
    ]
};

var cloned = DeepClone(original);

original.DepartmentName = "HR";
original.Employees[0].EmployeeName = "Eve";

PrintDepartment("Original", original);
PrintDepartment("Cloned", cloned);

static Department DeepClone(Department source)
{
    var json = JsonSerializer.Serialize(source);
    return JsonSerializer.Deserialize<Department>(json)!;
}

static void PrintDepartment(string title, Department department)
{
    Console.WriteLine($"{title} Department: {department.DepartmentName}");
    foreach (var employee in department.Employees)
    {
        Console.WriteLine($" - {employee.EmployeeName}");
    }
    Console.WriteLine();
}
