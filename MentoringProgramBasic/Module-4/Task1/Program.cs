using System;

while (true)
{
    try
    {
        var input = Console.ReadLine();
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentException("Input string is empty.");
        }

        Console.WriteLine(input[0]);
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}
