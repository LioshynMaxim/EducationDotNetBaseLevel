using System.Runtime.Serialization;

namespace ConsoleAppCustomBinarySerialization;

[Serializable]
public class Person : ISerializable
{
    public string Name { get; set; }
    public int Age { get; set; }

    public Person()
    {
        Name = string.Empty;
    }

    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }

    protected Person(SerializationInfo info, StreamingContext context)
    {
        Name = info.GetString(nameof(Name)) ?? string.Empty;
        Age = info.GetInt32(nameof(Age));
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(Name), Name);
        info.AddValue(nameof(Age), Age);
    }
}
