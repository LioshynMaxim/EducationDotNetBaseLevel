namespace UnitTestingClassLibraryHarryPotter.Tests
{
    [TestClass]
    public sealed class HarryPotterBasketTests
    {
        private HarryPotterBasket _basket = null!;

        [TestInitialize]
        public void Setup()
        {
            _basket = new HarryPotterBasket();
        }

        [TestMethod]
        public void EmptyBasket_ReturnsZero()
        {
            // Arrange & Act
            decimal price = _basket.CalculatePrice();

            // Assert
            Assert.AreEqual(0m, price);
        }

        [TestMethod]
        public void SingleBook_CostsEightEuros()
        {
            // Arrange & Act
            decimal price = _basket.CalculatePrice(1);

            // Assert
            Assert.AreEqual(8.0m, price);
        }

        [TestMethod]
        public void TwoSameBooks_NoDiscount()
        {
            // Arrange & Act
            decimal price = _basket.CalculatePrice(1, 1);

            // Assert
            Assert.AreEqual(16.0m, price);
        }

        [TestMethod]
        public void TwoDifferentBooks_FivePercentDiscount()
        {
            // Arrange & Act
            decimal price = _basket.CalculatePrice(1, 2);

            // Assert
            // 2 * 8 * 0.95 = 15.20
            Assert.AreEqual(15.20m, price);
        }

        [TestMethod]
        public void ThreeDifferentBooks_TenPercentDiscount()
        {
            // Arrange & Act
            decimal price = _basket.CalculatePrice(1, 2, 3);

            // Assert
            // 3 * 8 * 0.90 = 21.60
            Assert.AreEqual(21.60m, price);
        }

        [TestMethod]
        public void FourDifferentBooks_TwentyPercentDiscount()
        {
            // Arrange & Act
            decimal price = _basket.CalculatePrice(1, 2, 3, 4);

            // Assert
            // 4 * 8 * 0.80 = 25.60
            Assert.AreEqual(25.60m, price);
        }

        [TestMethod]
        public void FiveDifferentBooks_TwentyFivePercentDiscount()
        {
            // Arrange & Act
            decimal price = _basket.CalculatePrice(1, 2, 3, 4, 5);

            // Assert
            // 5 * 8 * 0.75 = 30.00
            Assert.AreEqual(30.00m, price);
        }

        [TestMethod]
        public void ThreeDifferentBooksPlusOneDuplicate_OptimalGrouping()
        {
            // Arrange & Act
            decimal price = _basket.CalculatePrice(1, 1, 2, 3);

            // Assert
            // Best: (1,2,3) + (1) = 21.60 + 8.00 = 29.60
            Assert.AreEqual(29.60m, price);
        }

        [TestMethod]
        public void ExampleFromKata_CorrectPrice()
        {
            // Arrange
            // 2 copies of the first book
            // 2 copies of the second book
            // 2 copies of the third book
            // 1 copy of the fourth book
            // 1 copy of the fifth book

            // Act
            decimal price = _basket.CalculatePrice(1, 1, 2, 2, 3, 3, 4, 5);

            // Assert
            // According to kata description: Answer is 51.60 EUR
            // Grouping: (1,2,3,4,5) + (1,2,3) = 30.00 + 21.60 = 51.60
            Assert.AreEqual(51.60m, price);
        }

        [TestMethod]
        public void TwoBooksThreeCopiesEach_OptimalGrouping()
        {
            // Arrange & Act
            decimal price = _basket.CalculatePrice(1, 1, 1, 2, 2, 2);

            // Assert
            // Best: (1,2) + (1,2) + (1,2) = 15.20 + 15.20 + 15.20 = 45.60
            Assert.AreEqual(45.60m, price);
        }

        [TestMethod]
        public void TenBooks_TwoCopiesEach()
        {
            // Arrange & Act
            decimal price = _basket.CalculatePrice(1, 1, 2, 2, 3, 3, 4, 4, 5, 5);

            // Assert
            // Best: (1,2,3,4,5) + (1,2,3,4,5) = 30.00 + 30.00 = 60.00
            Assert.AreEqual(60.00m, price);
        }

        [TestMethod]
        public void LargeBasket_CalculatesCorrectly()
        {
            // Arrange & Act
            decimal price = _basket.CalculatePrice(1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 5);

            // Assert
            // 11 books: optimize grouping
            // Possible: (1,2,3,4,5) + (1,2,3) + (1,2,3) = 30.00 + 21.60 + 21.60 = 73.20
            decimal expected = 73.20m;
            Assert.AreEqual(expected, price);
        }

        [TestMethod]
        public void EdgeCase_ThreeSameBooksPlusTwoDifferent()
        {
            // Arrange & Act
            decimal price = _basket.CalculatePrice(1, 1, 1, 2, 3);

            // Assert
            // Best: (1,2,3) + (1) + (1) = 21.60 + 8.00 + 8.00 = 37.60
            Assert.AreEqual(37.60m, price);
        }

        [TestMethod]
        public void UsingList_CalculatesCorrectly()
        {
            // Arrange
            var books = new List<int> { 1, 2, 3, 4, 5 };

            // Act
            decimal price = _basket.CalculatePriceFromList(books);

            // Assert
            Assert.AreEqual(30.00m, price);
        }

        [TestMethod]
        public void WithRounding_ReturnsCorrectValue()
        {
            // Arrange & Act
            decimal price = _basket.CalculatePriceWithDetails(1, 2, 3);

            // Assert
            Assert.AreEqual(21.60m, price);
        }

        [TestMethod]
        public void NullArray_ReturnsZero()
        {
            // Arrange & Act
            decimal price = _basket.CalculatePrice(null!);

            // Assert
            Assert.AreEqual(0m, price);
        }
    }
}
