namespace UnitTestingClassLibraryOddEven.Tests
{
    [TestClass]
    public sealed class OddEvenPrinterTests
    {
        private OddEvenPrinter _printer = null!;

        [TestInitialize]
        public void Setup()
        {
            _printer = new OddEvenPrinter();
        }

        #region Single Number Classification Tests

        [TestMethod]
        public void GetNumberClassification_EvenNumber_ReturnsEven()
        {
            // Arrange & Act
            string result = _printer.GetNumberClassification(4);

            // Assert
            Assert.AreEqual("Even", result);
        }

        [TestMethod]
        public void GetNumberClassification_OddNonPrime_ReturnsOdd()
        {
            // Arrange & Act
            string result = _printer.GetNumberClassification(9);

            // Assert
            Assert.AreEqual("Odd", result);
        }

        [TestMethod]
        public void GetNumberClassification_PrimeNumber_ReturnsNumber()
        {
            // Arrange & Act
            string result = _printer.GetNumberClassification(7);

            // Assert
            Assert.AreEqual("7", result);
        }

        [TestMethod]
        public void GetNumberClassification_Two_ReturnsTwo()
        {
            // Arrange & Act
            string result = _printer.GetNumberClassification(2);

            // Assert
            Assert.AreEqual("2", result);
        }

        [TestMethod]
        public void GetNumberClassification_One_ReturnsOne()
        {
            // Arrange & Act
            string result = _printer.GetNumberClassification(1);

            // Assert
            Assert.AreEqual("1", result);
        }

        #endregion

        #region IsPrime Tests

        [TestMethod]
        public void IsPrime_Two_ReturnsTrue()
        {
            // Arrange & Act
            bool result = _printer.IsPrime(2);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsPrime_Three_ReturnsTrue()
        {
            // Arrange & Act
            bool result = _printer.IsPrime(3);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsPrime_Seven_ReturnsTrue()
        {
            // Arrange & Act
            bool result = _printer.IsPrime(7);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsPrime_Eleven_ReturnsTrue()
        {
            // Arrange & Act
            bool result = _printer.IsPrime(11);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsPrime_Four_ReturnsFalse()
        {
            // Arrange & Act
            bool result = _printer.IsPrime(4);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsPrime_Nine_ReturnsFalse()
        {
            // Arrange & Act
            bool result = _printer.IsPrime(9);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsPrime_One_ReturnsFalse()
        {
            // Arrange & Act
            bool result = _printer.IsPrime(1);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsPrime_Zero_ReturnsFalse()
        {
            // Arrange & Act
            bool result = _printer.IsPrime(0);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsPrime_NegativeNumber_ReturnsFalse()
        {
            // Arrange & Act
            bool result = _printer.IsPrime(-5);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region IsEven Tests

        [TestMethod]
        public void IsEven_EvenNumber_ReturnsTrue()
        {
            // Arrange & Act
            bool result = _printer.IsEven(4);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsEven_OddNumber_ReturnsFalse()
        {
            // Arrange & Act
            bool result = _printer.IsEven(7);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsEven_Zero_ReturnsTrue()
        {
            // Arrange & Act
            bool result = _printer.IsEven(0);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion

        #region IsOdd Tests

        [TestMethod]
        public void IsOdd_OddNonPrimeNumber_ReturnsTrue()
        {
            // Arrange & Act
            bool result = _printer.IsOdd(9);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsOdd_EvenNumber_ReturnsFalse()
        {
            // Arrange & Act
            bool result = _printer.IsOdd(4);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsOdd_PrimeNumber_ReturnsFalse()
        {
            // Arrange & Act
            bool result = _printer.IsOdd(7);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region Range Printing Tests

        [TestMethod]
        public void PrintRange_SmallRange_ReturnsCorrectSequence()
        {
            // Arrange & Act
            var results = _printer.PrintRange(1, 10).ToList();

            // Assert
            Assert.AreEqual(10, results.Count);
            Assert.AreEqual("1", results[0]);      // 1 - number (not prime by convention)
            Assert.AreEqual("2", results[1]);      // 2 - prime
            Assert.AreEqual("3", results[2]);      // 3 - prime
            Assert.AreEqual("Even", results[3]);   // 4 - even
            Assert.AreEqual("5", results[4]);      // 5 - prime
            Assert.AreEqual("Even", results[5]);   // 6 - even
            Assert.AreEqual("7", results[6]);      // 7 - prime
            Assert.AreEqual("Even", results[7]);   // 8 - even
            Assert.AreEqual("Odd", results[8]);    // 9 - odd (not prime)
            Assert.AreEqual("Even", results[9]);   // 10 - even
        }

        [TestMethod]
        public void PrintDefault_ReturnsHundredElements()
        {
            // Arrange & Act
            var results = _printer.PrintDefault().ToList();

            // Assert
            Assert.AreEqual(100, results.Count);
        }

        [TestMethod]
        public void PrintDefault_ContainsCorrectPrimes()
        {
            // Arrange & Act
            var results = _printer.PrintDefault().ToList();

            // Assert - Check some known primes
            Assert.AreEqual("2", results[1]);   // 2
            Assert.AreEqual("3", results[2]);   // 3
            Assert.AreEqual("5", results[4]);   // 5
            Assert.AreEqual("7", results[6]);   // 7
            Assert.AreEqual("11", results[10]); // 11
            Assert.AreEqual("13", results[12]); // 13
        }

        [TestMethod]
        public void PrintDefault_ContainsCorrectEvens()
        {
            // Arrange & Act
            var results = _printer.PrintDefault().ToList();

            // Assert - Check some evens
            Assert.AreEqual("Even", results[3]);  // 4
            Assert.AreEqual("Even", results[5]);  // 6
            Assert.AreEqual("Even", results[7]);  // 8
            Assert.AreEqual("Even", results[9]);  // 10
        }

        [TestMethod]
        public void PrintDefault_ContainsCorrectOdds()
        {
            // Arrange & Act
            var results = _printer.PrintDefault().ToList();

            // Assert - Check some odds (non-prime)
            Assert.AreEqual("Odd", results[8]);   // 9
            Assert.AreEqual("Odd", results[14]);  // 15
            Assert.AreEqual("Odd", results[20]);  // 21
            Assert.AreEqual("Odd", results[24]);  // 25
        }

        [TestMethod]
        public void PrintRange_SameStartAndEnd_ReturnsSingleElement()
        {
            // Arrange & Act
            var results = _printer.PrintRange(5, 5).ToList();

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("5", results[0]);
        }

        #endregion

        #region Edge Cases Tests

        [TestMethod]
        public void GetNumberClassification_LargePrime_ReturnsNumber()
        {
            // Arrange & Act
            string result = _printer.GetNumberClassification(97);

            // Assert
            Assert.AreEqual("97", result);
        }

        [TestMethod]
        public void GetNumberClassification_LargeEven_ReturnsEven()
        {
            // Arrange & Act
            string result = _printer.GetNumberClassification(100);

            // Assert
            Assert.AreEqual("Even", result);
        }

        [TestMethod]
        public void GetNumberClassification_LargeOdd_ReturnsOdd()
        {
            // Arrange & Act
            string result = _printer.GetNumberClassification(99);

            // Assert
            Assert.AreEqual("Odd", result);
        }

        #endregion

        #region Specific Number Tests

        [TestMethod]
        public void SpecificNumbers_Classification_IsCorrect()
        {
            // Test specific numbers from 1-20
            var expectedResults = new Dictionary<int, string>
            {
                { 1, "1" },
                { 2, "2" },
                { 3, "3" },
                { 4, "Even" },
                { 5, "5" },
                { 6, "Even" },
                { 7, "7" },
                { 8, "Even" },
                { 9, "Odd" },
                { 10, "Even" },
                { 11, "11" },
                { 12, "Even" },
                { 13, "13" },
                { 14, "Even" },
                { 15, "Odd" },
                { 16, "Even" },
                { 17, "17" },
                { 18, "Even" },
                { 19, "19" },
                { 20, "Even" }
            };

            foreach (var kvp in expectedResults)
            {
                string result = _printer.GetNumberClassification(kvp.Key);
                Assert.AreEqual(kvp.Value, result, $"Number {kvp.Key} classification failed");
            }
        }

        #endregion
    }
}