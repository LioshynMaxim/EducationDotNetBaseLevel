namespace FileCabinetClassLibrary.Tests
{
    using FileCabinetClassLibrary.Interfaces;
    using FileCabinetClassLibrary.Models;
    using FileCabinetClassLibrary.Services;
    using Moq;

    [TestClass]
    public sealed class DocumentServiceTests
    {
        private Mock<IStorageService> _mockStorageService;
        private Mock<ICachingService> _mockCachingService;
        private DocumentService _documentService;

        [TestInitialize]
        public void Setup()
        {
            _mockStorageService = new Mock<IStorageService>();
            _mockCachingService = new Mock<ICachingService>();
            _documentService = new DocumentService(_mockStorageService.Object, _mockCachingService.Object);
        }

        #region Task 1: Basic Search Functionality

        [TestMethod]
        public void SearchById_WithMatchingBookCard_ReturnsBookCard()
        {
            // Arrange
            var bookCard = new BookCard
            {
                Id = "book_#123",
                ISBN = "978-3-16-148410-0",
                Title = "Test Book",
                Authors = new List<string> { "Author One" },
                NumberOfPages = 250,
                Publisher = "Test Publisher",
                DatePublished = new DateTime(2020, 1, 1)
            };

            _mockStorageService.Setup(s => s.GetAllCards())
                // Returns a list with a single book card
                .Returns(new List<BaseCard> { bookCard });

            // Act
            var result = _documentService.SearchById("book_#123").ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("book_#123", result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(BookCard));
            var book = result[0] as BookCard;
            Assert.AreEqual("Test Book", book.Title);
        }

        [TestMethod]
        public void SearchById_WithMatchingPatentCard_ReturnsPatentCard()
        {
            // Arrange
            var patentCard = new PatentCard
            {
                Id = "patent_#456",
                Title = "Innovative Patent",
                Authors = new List<string> { "Inventor One", "Inventor Two" },
                DatePublished = new DateTime(2019, 6, 15),
                ExpirationDate = new DateTime(2039, 6, 15),
                UniqueId = "US12345678"
            };

            _mockStorageService.Setup(s => s.GetAllCards())
                // Returns a list with a single patent card
                .Returns(new List<BaseCard> { patentCard });

            // Act
            var result = _documentService.SearchById("patent_#456").ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("patent_#456", result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(PatentCard));
            var patent = result[0] as PatentCard;
            Assert.AreEqual("US12345678", patent.UniqueId);
        }

        [TestMethod]
        public void SearchById_WithNoMatches_ReturnsEmptyCollection()
        {
            // Arrange
            var bookCard = new BookCard { Id = "book_#123" };
            _mockStorageService.Setup(s => s.GetAllCards())
                // Returns a list with a book card that does not match the search ID
                .Returns(new List<BaseCard> { bookCard });

            // Act
            var result = _documentService.SearchById("book_#999").ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void SearchById_IsCaseInsensitive()
        {
            // Arrange
            var bookCard = new BookCard { Id = "book_#123" };
            _mockStorageService.Setup(s => s.GetAllCards())
                // Returns a list with a book card
                .Returns(new List<BaseCard> { bookCard });

            // Act
            var resultLower = _documentService.SearchById("BOOK_#123").ToList();
            var resultMixed = _documentService.SearchById("Book_#123").ToList();

            // Assert
            Assert.AreEqual(1, resultLower.Count);
            Assert.AreEqual(1, resultMixed.Count);
        }

        [TestMethod]
        public void SearchById_WithMultipleCards_ReturnsSingleMatch()
        {
            // Arrange
            var cards = new List<BaseCard>
            {
                new BookCard { Id = "book_#123" },
                new BookCard { Id = "book_#124" },
                new PatentCard { Id = "patent_#456" }
            };

            _mockStorageService.Setup(s => s.GetAllCards())
                // Returns a list with multiple cards
                .Returns(cards);

            // Act
            var result = _documentService.SearchById("book_#123").ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("book_#123", result[0].Id);
        }

        #endregion

        #region Task 2: Magazine Support

        [TestMethod]
        public void SearchById_WithMatchingMagazineCard_ReturnsMagazineCard()
        {
            // Arrange
            var magazineCard = new MagazineCard
            {
                Id = "magazine_#789",
                Title = "Tech Weekly",
                Publisher = "Tech Publications",
                ReleaseNumber = 42,
                PublishDate = new DateTime(2023, 10, 15)
            };

            _mockStorageService.Setup(s => s.GetAllCards())
                // Returns a list with a single magazine card
                .Returns(new List<BaseCard> { magazineCard });

            // Act
            var result = _documentService.SearchById("magazine_#789").ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("magazine_#789", result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(MagazineCard));
            var magazine = result[0] as MagazineCard;
            Assert.AreEqual("Tech Weekly", magazine.Title);
            Assert.AreEqual(42, magazine.ReleaseNumber);
        }

        [TestMethod]
        public void SearchById_WithMixedDocumentTypes_ReturnsMagazineCorrectly()
        {
            // Arrange
            var cards = new List<BaseCard>
            {
                new BookCard { Id = "book_#123", Title = "A Book" },
                new MagazineCard
                {
                    Id = "magazine_#100",
                    Title = "Science Today",
                    Publisher = "Science Press",
                    ReleaseNumber = 15,
                    PublishDate = new DateTime(2023, 9, 1)
                },
                new PatentCard { Id = "patent_#456", Title = "A Patent" }
            };

            _mockStorageService.Setup(s => s.GetAllCards())
                // Returns a list with mixed document types
                .Returns(cards);

            // Act
            var result = _documentService.SearchById("magazine_#100").ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.IsInstanceOfType(result[0], typeof(MagazineCard));
            var magazine = result[0] as MagazineCard;
            Assert.AreEqual("Science Today", magazine.Title);
        }

        [TestMethod]
        public void MagazineCard_HasAllRequiredProperties()
        {
            // Arrange & Act
            var magazine = new MagazineCard
            {
                Id = "magazine_#1",
                Title = "Test Magazine",
                Publisher = "Test Publisher",
                ReleaseNumber = 5,
                PublishDate = new DateTime(2023, 1, 1)
            };

            // Assert
            Assert.AreEqual("magazine_#1", magazine.Id);
            Assert.AreEqual("Test Magazine", magazine.Title);
            Assert.AreEqual("Test Publisher", magazine.Publisher);
            Assert.AreEqual(5, magazine.ReleaseNumber);
            Assert.AreEqual(new DateTime(2023, 1, 1), magazine.PublishDate);
        }

        #endregion

        #region Integration Tests

        [TestMethod]
        public void SearchById_WithAllDocumentTypes_ReturnsCorrectType()
        {
            // Arrange
            var cards = new List<BaseCard>
            {
                new BookCard
                {
                    Id = "book_#001",
                    ISBN = "978-0-000-00001-0",
                    Title = "Book 1",
                    Authors = new List<string> { "Author A" },
                    NumberOfPages = 300,
                    Publisher = "Pub 1",
                    DatePublished = new DateTime(2020, 1, 1)
                },
                new PatentCard
                {
                    Id = "patent_#002",
                    Title = "Patent 1",
                    Authors = new List<string> { "Inventor A" },
                    DatePublished = new DateTime(2015, 1, 1),
                    ExpirationDate = new DateTime(2035, 1, 1),
                    UniqueId = "US0000001"
                },
                new MagazineCard
                {
                    Id = "magazine_#003",
                    Title = "Magazine 1",
                    Publisher = "Magazine Pub",
                    ReleaseNumber = 1,
                    PublishDate = new DateTime(2023, 1, 1)
                }
            };

            _mockStorageService.Setup(s => s.GetAllCards())
                // Returns a list with all document types
                .Returns(cards);

            // Act & Assert
            var bookResult = _documentService.SearchById("book_#001").FirstOrDefault();
            Assert.IsNotNull(bookResult);
            Assert.IsInstanceOfType(bookResult, typeof(BookCard));

            var patentResult = _documentService.SearchById("patent_#002").FirstOrDefault();
            Assert.IsNotNull(patentResult);
            Assert.IsInstanceOfType(patentResult, typeof(PatentCard));

            var magazineResult = _documentService.SearchById("magazine_#003").FirstOrDefault();
            Assert.IsNotNull(magazineResult);
            Assert.IsInstanceOfType(magazineResult, typeof(MagazineCard));
        }

        [TestMethod]
        public void DocumentService_EmptyStorage_ReturnsEmptyCollection()
        {
            // Arrange
            _mockStorageService.Setup(s => s.GetAllCards())
                // Returns an empty list
                .Returns(new List<BaseCard>());

            // Act
            var result = _documentService.SearchById("any_#123").ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        #endregion
    }

    [TestClass]
    public sealed class StorageServiceTests
    {
        [TestMethod]
        public void StorageService_CanAddCard()
        {
            // Arrange
            var bookCard = new BookCard
            {
                Id = "book_#123",
                ISBN = "978-3-16-148410-0",
                Title = "Test Book",
                Authors = new List<string> { "Author" },
                NumberOfPages = 200,
                Publisher = "Publisher",
                DatePublished = new DateTime(2020, 1, 1)
            };

            // Act & Assert
            Assert.IsNotNull(bookCard.Id);
            Assert.AreEqual("book_#123", bookCard.Id);
        }

        [TestMethod]
        public void StorageService_CanStoreMultipleCardTypes()
        {
            // Arrange
            var cards = new List<BaseCard>
            {
                new BookCard { Id = "book_#1" },
                new PatentCard { Id = "patent_#2" },
                new MagazineCard { Id = "magazine_#3" }
            };

            // Act
            var bookCount = cards.OfType<BookCard>().Count();
            var patentCount = cards.OfType<PatentCard>().Count();
            var magazineCount = cards.OfType<MagazineCard>().Count();

            // Assert
            Assert.AreEqual(1, bookCount);
            Assert.AreEqual(1, patentCount);
            Assert.AreEqual(1, magazineCount);
        }
    }

    [TestClass]
    public sealed class MagazineCardTests
    {
        [TestMethod]
        public void MagazineCard_InheritsFromBaseCard()
        {
            // Arrange & Act
            var magazine = new MagazineCard { Id = "magazine_#1" };

            // Assert
            Assert.IsInstanceOfType(magazine, typeof(BaseCard));
        }

        [TestMethod]
        public void MagazineCard_HasCorrectProperties()
        {
            // Arrange
            var magazine = new MagazineCard
            {
                Id = "magazine_#100",
                Title = "National Geographic",
                Publisher = "National Geographic Society",
                ReleaseNumber = 5,
                PublishDate = new DateTime(2023, 5, 1)
            };

            // Assert
            Assert.AreEqual("magazine_#100", magazine.Id);
            Assert.AreEqual("National Geographic", magazine.Title);
            Assert.AreEqual("National Geographic Society", magazine.Publisher);
            Assert.AreEqual(5, magazine.ReleaseNumber);
            Assert.AreEqual(new DateTime(2023, 5, 1), magazine.PublishDate);
        }
    }

    [TestClass]
    public sealed class CachingDocumentServiceTests
    {
        private CachingDocumentService _cachingService;

        [TestInitialize]
        public void Setup()
        {
            _cachingService = new CachingDocumentService();
        }

        #region SetValue/TryGetValue Tests

        [TestMethod]
        public void SetValue_StoresValueInCache()
        {
            // Act
            _cachingService.SetValue("test_key", "test_value");

            // Assert
            Assert.IsTrue(_cachingService.TryGetValue("test_key", out var value));
            Assert.AreEqual("test_value", value);
        }

        [TestMethod]
        public void SetValue_WithMultipleValues_StoresAllValues()
        {
            // Act
            _cachingService.SetValue("key1", "value1");
            _cachingService.SetValue("key2", "value2");
            _cachingService.SetValue("key3", "value3");

            // Assert
            Assert.IsTrue(_cachingService.TryGetValue("key1", out var value1));
            Assert.IsTrue(_cachingService.TryGetValue("key2", out var value2));
            Assert.IsTrue(_cachingService.TryGetValue("key3", out var value3));
            Assert.AreEqual("value1", value1);
            Assert.AreEqual("value2", value2);
            Assert.AreEqual("value3", value3);
        }

        [TestMethod]
        public void SetValue_OverwritesPreviousValue()
        {
            // Act
            _cachingService.SetValue("key", "value1");
            _cachingService.SetValue("key", "value2");

            // Assert
            Assert.IsTrue(_cachingService.TryGetValue("key", out var value));
            Assert.AreEqual("value2", value);
        }

        [TestMethod]
        public void TryGetValue_WithNonexistentKey_ReturnsFalse()
        {
            // Act
            var result = _cachingService.TryGetValue("nonexistent_key", out var value);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(value);
        }

        [TestMethod]
        public void TryGetValue_WithExistingKey_ReturnsTrue()
        {
            // Arrange
            _cachingService.SetValue("existing_key", "existing_value");

            // Act
            var result = _cachingService.TryGetValue("existing_key", out var value);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("existing_value", value);
        }

        [TestMethod]
        public void SetValue_StoresComplexObjects()
        {
            // Arrange
            var bookCard = new BookCard { Id = "book_#123", Title = "Test Book" };

            // Act
            _cachingService.SetValue("book_object", bookCard);

            // Assert
            Assert.IsTrue(_cachingService.TryGetValue("book_object", out var retrievedValue));
            var retrievedCard = retrievedValue as BookCard;
            Assert.IsNotNull(retrievedCard);
            Assert.AreEqual("book_#123", retrievedCard.Id);
        }

        #endregion

        #region RemoveValue Tests

        [TestMethod]
        public void RemoveValue_RemovesValueFromCache()
        {
            // Arrange
            _cachingService.SetValue("key_to_remove", "value");

            // Act
            _cachingService.RemoveValue("key_to_remove");

            // Assert
            Assert.IsFalse(_cachingService.TryGetValue("key_to_remove", out _));
        }

        [TestMethod]
        public void RemoveValue_WithNonexistentKey_DoesNotThrowException()
        {
            // Act & Assert - should not throw
            _cachingService.RemoveValue("nonexistent_key");
        }

        [TestMethod]
        public void RemoveValue_RemovesOnlySpecificKey()
        {
            // Arrange
            _cachingService.SetValue("key1", "value1");
            _cachingService.SetValue("key2", "value2");

            // Act
            _cachingService.RemoveValue("key1");

            // Assert
            Assert.IsFalse(_cachingService.TryGetValue("key1", out _));
            Assert.IsTrue(_cachingService.TryGetValue("key2", out _));
        }

        #endregion

        #region ClearCache Tests

        [TestMethod]
        public void ClearCache_RemovesAllValues()
        {
            // Arrange
            _cachingService.SetValue("key1", "value1");
            _cachingService.SetValue("key2", "value2");

            // Act
            _cachingService.ClearCache();

            // Assert
            Assert.IsFalse(_cachingService.TryGetValue("key1", out _));
            Assert.IsFalse(_cachingService.TryGetValue("key2", out _));
        }

        [TestMethod]
        public void ClearCache_OnEmptyCache_DoesNotThrowException()
        {
            // Act & Assert - should not throw
            _cachingService.ClearCache();
        }

        #endregion

        #region Document Type Cache Configuration Tests

        [TestMethod]
        public void CachingService_WithDefaultExpiration_CachesForConfiguredTime()
        {
            // Arrange
            var cachingService = new CachingDocumentService(TimeSpan.FromMilliseconds(100));
            var bookCard = new BookCard { Id = "book_#123", Title = "Test Book" };

            // Act
            cachingService.SetValue("book", bookCard);
            Assert.IsTrue(cachingService.TryGetValue("book", out var value1));

            Thread.Sleep(150);
            var result = cachingService.TryGetValue("book", out var value2);

            // Assert - Cache should expire after 100ms
            Assert.IsTrue(value1 is BookCard);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CachingService_WithDocumentTypeConfig_UsesTypeSpecificExpiration()
        {
            // Arrange
            var config = new Dictionary<string, TimeSpan?>
            {
                { "book", TimeSpan.FromMilliseconds(100) },
                { "magazine", null },
                { "patent", TimeSpan.Zero }
            };
            var cachingService = new CachingDocumentService(TimeSpan.FromSeconds(5), config);

            var bookCard = new BookCard { Id = "book_#123" };
            var magazineCard = new MagazineCard { Id = "magazine_#456" };
            var patentCard = new PatentCard { Id = "patent_#789" };

            // Act
            cachingService.SetValue("book", bookCard);
            cachingService.SetValue("magazine", magazineCard);
            cachingService.SetValue("patent", patentCard);

            // Assert - Book should expire quickly
            Assert.IsTrue(cachingService.TryGetValue("book", out _));
            Thread.Sleep(150);
            Assert.IsFalse(cachingService.TryGetValue("book", out _));

            // Magazine should not expire (null means live long cache)
            Assert.IsTrue(cachingService.TryGetValue("magazine", out _));

            // Patent should not be cached (TimeSpan.Zero)
            Assert.IsFalse(cachingService.TryGetValue("patent", out _));
        }

        [TestMethod]
        public void CachingService_WithZeroTimespan_DoesNotCache()
        {
            // Arrange
            var config = new Dictionary<string, TimeSpan?>
            {
                { "book", TimeSpan.Zero }
            };
            var cachingService = new CachingDocumentService(TimeSpan.FromSeconds(5), config);
            var bookCard = new BookCard { Id = "book_#123" };

            // Act
            cachingService.SetValue("book", bookCard);

            // Assert - Should not be cached
            Assert.IsFalse(cachingService.TryGetValue("book", out _));
        }

        [TestMethod]
        public void CachingService_WithNullExpiration_LiveLongCache()
        {
            // Arrange
            var config = new Dictionary<string, TimeSpan?>
            {
                { "magazine", null }
            };
            var cachingService = new CachingDocumentService(TimeSpan.FromMilliseconds(50), config);
            var magazineCard = new MagazineCard { Id = "magazine_#100" };

            // Act
            cachingService.SetValue("magazine", magazineCard);
            Assert.IsTrue(cachingService.TryGetValue("magazine", out var value1));

            Thread.Sleep(100);
            var result = cachingService.TryGetValue("magazine", out var value2);

            // Assert - Magazine should not expire
            Assert.IsTrue(result);
            Assert.IsNotNull(value2);
        }

        #endregion

        #region Interface Implementation

        [TestMethod]
        public void CachingService_ImplementsICachingService()
        {
            // Assert
            Assert.IsInstanceOfType(_cachingService, typeof(ICachingService));
        }

        #endregion
    }
}
