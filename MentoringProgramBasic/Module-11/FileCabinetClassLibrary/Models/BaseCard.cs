using System.Text.Json.Serialization;

namespace FileCabinetClassLibrary.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(BookCard), "book")]
[JsonDerivedType(typeof(PatentCard), "patent")]
[JsonDerivedType(typeof(MagazineCard), "magazine")]
public abstract class BaseCard
{
    public string Id { get; set; }
}
