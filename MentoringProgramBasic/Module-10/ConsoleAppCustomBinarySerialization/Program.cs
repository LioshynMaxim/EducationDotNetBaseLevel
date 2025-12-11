using ConsoleAppCustomBinarySerialization;
using System.Runtime.Serialization.Formatters.Binary;

var person = new Person("Alice", 28);
var filePath = Path.Combine(AppContext.BaseDirectory, "person.bin");

using (var stream = File.Create(filePath))
{
    var formatter = new BinaryFormatter();
    formatter.Serialize(stream, person);
}

Person restored;
using (var stream = File.OpenRead(filePath))
{
    var formatter = new BinaryFormatter();
    restored = (Person)formatter.Deserialize(stream);
}

Console.WriteLine($"Restored: {restored.Name}, {restored.Age}");
