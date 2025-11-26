namespace UnitTestingClassLibraryOddEven
{
    public class OddEvenPrinter
    {
        public bool IsEven(int number) => number % 2 == 0;

        public bool IsOdd(int number) => number % 2 != 0 && !IsPrime(number);

        public IEnumerable<string> PrintDefault() => PrintRange(1, 100);

        public string GetNumberClassification(int number)
        {
            if (number <= 1 || IsPrime(number))
            {
                return number.ToString();
            }

            return IsEven(number) ? "Even" : "Odd";
        }

        public IEnumerable<string> PrintRange(int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                yield return GetNumberClassification(i);
            }
        }

        public bool IsPrime(int number)
        {
            if (number == 2)
            {
                return true;
            }

            if (number <= 1 || number % 2 == 0)
            {
                return false;
            }

            int limit = (int)Math.Sqrt(number);
            for (int i = 3; i <= limit; i += 2)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }

            return true;
        }

    }
}