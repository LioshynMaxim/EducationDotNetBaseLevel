using System;

namespace Module2_Task1;

public static class Utilities
{
    /// <summary>
    /// Sorts an array in ascending order using bubble sort.
    /// </summary>
    /// <param name="numbers">Numbers to sort.</param>
    public static void Sort(int[] numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);

        int temp;
        for (int i = 0; i < numbers.Length; i++)
        {
            for (int j = 0; j < numbers.Length; j++)
            {
                if (numbers[i] < numbers[j])
                {
                    temp = numbers[i];
                    numbers[i] = numbers[j];
                    numbers[j] = temp;
                }
            }
        }
    }

    /// <summary>
    /// Searches for the index of a product in an <paramref name="products"/> 
    /// based on a <paramref name="predicate"/>.
    /// </summary>
    /// <param name="products">Products used for searching.</param>
    /// <param name="predicate">Product predicate.</param>
    /// <returns>If match found then returns index of product in <paramref name="products"/>
    /// otherwise -1.</returns>
    public static int IndexOf(Product[] products, Predicate<Product> predicate)
    {
        ArgumentNullException.ThrowIfNull(products);
        ArgumentNullException.ThrowIfNull(predicate);

        for (int i = 0; i <= products.Length - 1; i++)
        {
            var product = products[i];
            if (predicate(product))
            {
                return i;
            }
        }

        return -1;
    }
}
