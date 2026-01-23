namespace UnitTestingClassLibraryHarryPotter
{
    public class HarryPotterBasket
    {
        private const decimal BookPrice = 8.0m;
        private readonly Dictionary<int, decimal> _discounts = new()
        {
            { 1, 0.0m },
            { 2, 0.05m },
            { 3, 0.10m },
            { 4, 0.20m },
            { 5, 0.25m }
        };

        public decimal CalculatePrice(params int[] books)
        {
            if (books == null || books.Length == 0)
            {
                return 0m;
            }

            var bookCounts = books
                .GroupBy(book => book)
                .ToDictionary(group => group.Key, group => group.Count());

            return CalculateOptimalPrice(bookCounts);
        }

        public decimal CalculatePriceFromList(List<int> books) => CalculatePrice(books.ToArray());

        public decimal CalculatePriceWithDetails(params int[] books)
        {
            decimal price = CalculatePrice(books);
            return Math.Round(price, 2);
        }

        private decimal CalculateOptimalPrice(Dictionary<int, int> bookCounts)
        {
            decimal minPrice = CalculatePriceGreedy(bookCounts);
            decimal optimizedPrice = CalculatePriceOptimized(bookCounts);
            return Math.Min(minPrice, optimizedPrice);
        }

        private decimal CalculatePriceGreedy(Dictionary<int, int> bookCounts)
        {
            var counts = new List<int>(bookCounts.Values);
            decimal totalPrice = 0m;

            while (counts.Any(c => c > 0))
            {
                int uniqueBooks = counts.Count(c => c > 0);
                
                if (uniqueBooks == 0) break;

                decimal groupPrice = BookPrice * uniqueBooks * (1 - _discounts[uniqueBooks]);
                totalPrice += groupPrice;

                for (int i = 0; i < counts.Count; i++)
                {
                    if (counts[i] > 0)
                    {
                        counts[i]--;
                    }
                }
            }

            return totalPrice;
        }

        private decimal CalculatePriceOptimized(Dictionary<int, int> bookCounts)
        {
            var counts = new List<int>(bookCounts.Values);
            decimal totalPrice = 0m;

            while (counts.Any(c => c > 0))
            {
                int uniqueBooks = counts.Count(c => c > 0);

                if (uniqueBooks == 0)
                {
                    break;
                }

                if (uniqueBooks == 5 && counts.Count(c => c >= 2) >= 3)
                {
                    int booksWithMultipleCopies = counts.Count(c => c >= 2);
                    if (booksWithMultipleCopies >= 4)
                    {
                        uniqueBooks = 4;
                    }
                }

                decimal groupPrice = BookPrice * uniqueBooks * (1 - _discounts[uniqueBooks]);
                totalPrice += groupPrice;

                int booksProcessed = 0;
                for (int i = 0; i < counts.Count && booksProcessed < uniqueBooks; i++)
                {
                    if (counts[i] > 0)
                    {
                        counts[i]--;
                        booksProcessed++;
                    }
                }
            }

            return totalPrice;
        }
    }
}
