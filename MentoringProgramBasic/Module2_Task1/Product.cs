using System;

namespace Module2_Task1;

public class Product(string name, double price)
{
    public string Name { get; set; } = name;

    public double Price { get; set; } = price;

    public static bool operator ==(Product left, Product right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Name == right.Name && left.Price == right.Price;
    }

    public static bool operator !=(Product left, Product right) => !(left == right);

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (obj is null || obj.GetType() != GetType())
            return false;

        var other = (Product)obj;
        return Name == other.Name && Price == other.Price;
    }

    public override int GetHashCode() => HashCode.Combine(Name, Price);
}
