namespace FileCabinetClassLibrary.Tests
{
    using FileCabinetClassLibrary.Interfaces;
    using FileCabinetClassLibrary.Models;
    using FileCabinetClassLibrary.Services;
    using System.IO;

    [TestClass]
    public sealed class StorageJsonServiceTests
    {
        private StorageJsonService _storageService;
        private string _testDirectory;

        [TestInitialize]
        public void Setup()
        {
            _storageService = new StorageJsonService();
            _testDirectory = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString());
            if (!Directory.Exists(_testDirectory))
            {
                Directory.CreateDirectory(_testDirectory);
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_testDirectory))
            {
                try
                {
                    Directory.Delete(_testDirectory, true);
                }
                catch (DirectoryNotFoundException)
                {
                    // Directory was already deleted, ignore
                }
                catch (IOException)
                {
                    // Directory is in use or files are locked, retry with delay
                    System.Threading.Thread.Sleep(100);
                    try
                    {
                        Directory.Delete(_testDirectory, true);
                    }
                    catch
                    {
                        // Ignore if still cannot delete
                    }
                }
            }
        }

        #region GetAllCards Tests

        [TestMethod]
        public void GetAllCards_WithNoCardsStored_ReturnsEmptyCollection()
        {
            // Act
            var result = _storageService.GetAllCards();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetAllCards_ReturnsIEnumerable()
        {
            // Act
            var result = _storageService.GetAllCards();

            // Assert
            Assert.IsInstanceOfType(result, typeof(IEnumerable<BaseCard>));
        }

        #endregion

        #region AddCard Tests

        [TestMethod]
        public void AddCard_WithValidBookCard_AddsCard()
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

            // Act
            _storageService.AddCard(bookCard);

            // Assert
            var cards = _storageService.GetAllCards();
            Assert.AreEqual(1, cards.Count());
        }

        [TestMethod]
        public void AddCard_WithValidPatentCard_AddsCard()
        {
            // Arrange
            var patentCard = new PatentCard
            {
                Id = "patent_#456",
                Title = "Innovative Patent",
                Authors = new List<string> { "Inventor One" },
                DatePublished = new DateTime(2019, 6, 15),
                ExpirationDate = new DateTime(2039, 6, 15),
                UniqueId = "US12345678"
            };

            // Act
            _storageService.AddCard(patentCard);

            // Assert
            var cards = _storageService.GetAllCards();
            Assert.AreEqual(1, cards.Count());
        }

        [TestMethod]
        public void AddCard_WithValidMagazineCard_AddsCard()
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

            // Act
            _storageService.AddCard(magazineCard);

            // Assert
            var cards = _storageService.GetAllCards();
            Assert.AreEqual(1, cards.Count());
        }

        [TestMethod]
        public void AddCard_WithMultipleCards_AddsAllCards()
        {
            // Arrange
            var bookCard = new BookCard { Id = "book_#1", Title = "Book 1" };
            var patentCard = new PatentCard { Id = "patent_#2", Title = "Patent 1" };
            var magazineCard = new MagazineCard { Id = "magazine_#3", Title = "Magazine 1" };

            // Act
            _storageService.AddCard(bookCard);
            _storageService.AddCard(patentCard);
            _storageService.AddCard(magazineCard);

            // Assert
            var cards = _storageService.GetAllCards().ToList();
            Assert.AreEqual(3, cards.Count);
            Assert.IsTrue(cards.Any(c => c.Id == "book_#1"));
            Assert.IsTrue(cards.Any(c => c.Id == "patent_#2"));
            Assert.IsTrue(cards.Any(c => c.Id == "magazine_#3"));
        }

        #endregion

        #region GetCardById Tests

        [TestMethod]
        public void GetCardById_WithValidId_ReturnsCard()
        {
            // Arrange
            var bookCard = new BookCard
            {
                Id = "book_#123",
                Title = "Test Book"
            };
            _storageService.AddCard(bookCard);

            // Act
            var result = _storageService.GetCardById("book_#123");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("book_#123", result.Id);
        }

        [TestMethod]
        public void GetCardById_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var bookCard = new BookCard { Id = "book_#123" };
            _storageService.AddCard(bookCard);

            // Act
            var result = _storageService.GetCardById("book_#999");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetCardById_WithEmptyStorage_ReturnsNull()
        {
            // Act
            var result = _storageService.GetCardById("any_#123");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetCardById_ReturnsCorrectCardType()
        {
            // Arrange
            var patentCard = new PatentCard
            {
                Id = "patent_#456",
                Title = "Patent",
                UniqueId = "US123"
            };
            _storageService.AddCard(patentCard);

            // Act
            var result = _storageService.GetCardById("patent_#456");

            // Assert
            Assert.IsInstanceOfType(result, typeof(PatentCard));
            var patent = result as PatentCard;
            Assert.AreEqual("US123", patent.UniqueId);
        }

        #endregion

        #region GetCardByPredicate Tests

        [TestMethod]
        public void GetCardByPredicate_WithMatchingCondition_ReturnsCard()
        {
            // Arrange
            var bookCard = new BookCard
            {
                Id = "book_#123",
                Title = "Test Book"
            };
            _storageService.AddCard(bookCard);

            // Act
            var result = _storageService.GetCardByPredicate(c => c.Id == "book_#123");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("book_#123", result.Id);
        }

        [TestMethod]
        public void GetCardByPredicate_WithNoMatch_ReturnsNull()
        {
            // Arrange
            var bookCard = new BookCard { Id = "book_#123" };
            _storageService.AddCard(bookCard);

            // Act
            var result = _storageService.GetCardByPredicate(c => c.Id == "book_#999");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetCardByPredicate_WithComplexCondition_ReturnsCorrectCard()
        {
            // Arrange
            _storageService.AddCard(new BookCard { Id = "book_#1", Title = "Book 1" });
            _storageService.AddCard(new BookCard { Id = "book_#2", Title = "Book 2" });
            _storageService.AddCard(new PatentCard { Id = "patent_#3" });

            // Act
            var result = _storageService.GetCardByPredicate(c => c is BookCard && c.Id == "book_#2");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BookCard));
            Assert.AreEqual("book_#2", result.Id);
        }

        [TestMethod]
        public void GetCardByPredicate_WithTypeCondition_ReturnsFirstMatchingType()
        {
            // Arrange
            _storageService.AddCard(new BookCard { Id = "book_#1" });
            _storageService.AddCard(new PatentCard { Id = "patent_#2" });
            _storageService.AddCard(new MagazineCard { Id = "magazine_#3" });

            // Act
            var result = _storageService.GetCardByPredicate(c => c is PatentCard);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PatentCard));
            Assert.AreEqual("patent_#2", result.Id);
        }

        [TestMethod]
        public void GetCardByPredicate_WithEmptyStorage_ReturnsNull()
        {
            // Act
            var result = _storageService.GetCardByPredicate(c => c.Id.Contains("book"));

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetCardByPredicate_WithPropertyCondition_ReturnsMatchingCard()
        {
            // Arrange
            var magazine1 = new MagazineCard { Id = "magazine_#1", Title = "Tech", ReleaseNumber = 5 };
            var magazine2 = new MagazineCard { Id = "magazine_#2", Title = "Science", ReleaseNumber = 10 };
            _storageService.AddCard(magazine1);
            _storageService.AddCard(magazine2);

            // Act
            var result = _storageService.GetCardByPredicate(c => c is MagazineCard m && m.ReleaseNumber > 7);

            // Assert
            Assert.IsNotNull(result);
            var magazine = result as MagazineCard;
            Assert.AreEqual(10, magazine.ReleaseNumber);
        }

        #endregion

        #region UpdateCard Tests

        [TestMethod]
        public void UpdateCard_WithValidCard_UpdatesExistingCard()
        {
            // Arrange
            var bookCard = new BookCard
            {
                Id = "book_#123",
                Title = "Original Title"
            };
            _storageService.AddCard(bookCard);

            var updatedCard = new BookCard
            {
                Id = "book_#123",
                Title = "Updated Title"
            };

            // Act
            _storageService.UpdateCard(updatedCard);

            // Assert
            var result = _storageService.GetCardById("book_#123");
            Assert.IsNotNull(result);
            var book = result as BookCard;
            Assert.AreEqual("Updated Title", book.Title);
        }

        [TestMethod]
        public void UpdateCard_WithNewCard_DoesNotIncreaseCount()
        {
            // Arrange
            var bookCard = new BookCard { Id = "book_#123", Title = "Book 1" };
            _storageService.AddCard(bookCard);

            var updatedCard = new BookCard { Id = "book_#123", Title = "Updated Book 1" };

            // Act
            _storageService.UpdateCard(updatedCard);

            // Assert
            var cards = _storageService.GetAllCards();
            Assert.AreEqual(1, cards.Count());
        }

        [TestMethod]
        public void UpdateCard_WithDifferentProperties_UpdatesAllProperties()
        {
            // Arrange
            var originalCard = new BookCard
            {
                Id = "book_#1",
                Title = "Original",
                ISBN = "123-456",
                NumberOfPages = 100
            };
            _storageService.AddCard(originalCard);

            var updatedCard = new BookCard
            {
                Id = "book_#1",
                Title = "Updated",
                ISBN = "789-012",
                NumberOfPages = 200
            };

            // Act
            _storageService.UpdateCard(updatedCard);

            // Assert
            var result = _storageService.GetCardById("book_#1") as BookCard;
            Assert.AreEqual("Updated", result.Title);
            Assert.AreEqual("789-012", result.ISBN);
            Assert.AreEqual(200, result.NumberOfPages);
        }

        #endregion

        #region DeleteCard Tests

        [TestMethod]
        public void DeleteCard_WithValidId_RemovesCard()
        {
            // Arrange
            var bookCard = new BookCard { Id = "book_#123" };
            _storageService.AddCard(bookCard);

            // Act
            _storageService.DeleteCard("book_#123");

            // Assert
            var result = _storageService.GetCardById("book_#123");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void DeleteCard_WithValidId_DecreasesCardCount()
        {
            // Arrange
            var bookCard1 = new BookCard { Id = "book_#1" };
            var bookCard2 = new BookCard { Id = "book_#2" };
            _storageService.AddCard(bookCard1);
            _storageService.AddCard(bookCard2);

            // Act
            _storageService.DeleteCard("book_#1");

            // Assert
            var cards = _storageService.GetAllCards();
            Assert.AreEqual(1, cards.Count());
        }

        [TestMethod]
        public void DeleteCard_WithInvalidId_DoesNotThrowException()
        {
            // Act & Assert
            _storageService.DeleteCard("nonexistent_#999");
        }

        [TestMethod]
        public void DeleteCard_WithInvalidId_DoesNotChangeCardCount()
        {
            // Arrange
            var bookCard = new BookCard { Id = "book_#1" };
            _storageService.AddCard(bookCard);
            var initialCount = _storageService.GetAllCards().Count();

            // Act
            _storageService.DeleteCard("nonexistent_#999");

            // Assert
            var finalCount = _storageService.GetAllCards().Count();
            Assert.AreEqual(initialCount, finalCount);
        }

        #endregion

        #region LoadFromStorage Tests

        [TestMethod]
        public void LoadFromStorage_WithValidJsonFile_LoadsCards()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "test_data.json");
            var testCard = new BookCard
            {
                Id = "book_#123",
                Title = "Test Book"
            };
            _storageService.AddCard(testCard);
            _storageService.SaveToStorage(filePath);

            // Create new instance to test loading
            var newStorageService = new StorageJsonService();

            // Act
            newStorageService.LoadFromStorage(filePath);

            // Assert
            var cards = newStorageService.GetAllCards();
            Assert.IsTrue(cards.Count() > 0);
        }

        [TestMethod]
        public void LoadFromStorage_WithInvalidFilePath_DoesNotThrowException()
        {
            // Act & Assert - should not throw any exception
            try
            {
                _storageService.LoadFromStorage("/invalid/path/that/does/not/exist.json");
            }
            catch (Exception ex)
            {
                Assert.Fail($"LoadFromStorage should not throw exception for invalid path, but threw: {ex.GetType().Name}");
            }
        }

        [TestMethod]
        public void LoadFromStorage_ClearsExistingData()
        {
            // Arrange
            var existingCard = new BookCard { Id = "book_#1" };
            _storageService.AddCard(existingCard);

            var filePath = Path.Combine(_testDirectory, "test_load.json");
            var newStorageService = new StorageJsonService();
            var newCard = new BookCard { Id = "book_#2", Title = "New Book" };
            newStorageService.AddCard(newCard);
            newStorageService.SaveToStorage(filePath);

            // Act
            _storageService.LoadFromStorage(filePath);

            // Assert - should only have the loaded card
            var cards = _storageService.GetAllCards();
            Assert.IsFalse(cards.Any(c => c.Id == "book_#1"));
        }

        #endregion

        #region SaveToStorage Tests

        [TestMethod]
        public void SaveToStorage_WithValidPath_CreatesJsonFile()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "storage.json");
            var bookCard = new BookCard { Id = "book_#123", Title = "Test Book" };
            _storageService.AddCard(bookCard);

            // Act
            _storageService.SaveToStorage(filePath);

            // Assert
            Assert.IsTrue(File.Exists(filePath));
        }

        [TestMethod]
        public void SaveToStorage_SavesAllCards()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "all_cards.json");
            _storageService.AddCard(new BookCard { Id = "book_#1" });
            _storageService.AddCard(new PatentCard { Id = "patent_#2" });
            _storageService.AddCard(new MagazineCard { Id = "magazine_#3" });

            // Act
            _storageService.SaveToStorage(filePath);

            // Assert
            Assert.IsTrue(File.Exists(filePath));
            var content = File.ReadAllText(filePath);
            Assert.IsFalse(string.IsNullOrEmpty(content));
        }

        [TestMethod]
        public void SaveToStorage_WithEmptyStorage_CreatesEmptyJsonFile()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "empty.json");

            // Act
            _storageService.SaveToStorage(filePath);

            // Assert
            Assert.IsTrue(File.Exists(filePath));
        }

        #endregion

        #region IStorageService Implementation Tests

        [TestMethod]
        public void StorageJsonService_ImplementsIStorageService()
        {
            // Act & Assert
            Assert.IsInstanceOfType(_storageService, typeof(IStorageService));
        }

        [TestMethod]
        public void StorageJsonService_CanBeInstantiatedWithoutParameters()
        {
            // Act
            var service = new StorageJsonService();

            // Assert
            Assert.IsNotNull(service);
        }

        #endregion

        #region Integration Tests

        [TestMethod]
        public void AddCard_GetCardById_ReturnsCorrectCard()
        {
            // Arrange
            var bookCard = new BookCard
            {
                Id = "book_#123",
                ISBN = "978-3-16-148410-0",
                Title = "Integration Test Book",
                Authors = new List<string> { "Author" },
                NumberOfPages = 300,
                Publisher = "Publisher",
                DatePublished = new DateTime(2020, 1, 1)
            };
            _storageService.AddCard(bookCard);

            // Act
            var retrievedCard = _storageService.GetCardById("book_#123");

            // Assert
            Assert.IsNotNull(retrievedCard);
            var book = retrievedCard as BookCard;
            Assert.AreEqual("Integration Test Book", book.Title);
            Assert.AreEqual(300, book.NumberOfPages);
        }

        [TestMethod]
        public void SaveToStorage_LoadFromStorage_PreservesData()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "preserve_test.json");
            var originalCard = new BookCard
            {
                Id = "book_#123",
                Title = "Original Book"
            };
            _storageService.AddCard(originalCard);
            _storageService.SaveToStorage(filePath);

            var loadService = new StorageJsonService();

            // Act
            loadService.LoadFromStorage(filePath);

            // Assert
            var loadedCard = loadService.GetCardById("book_#123");
            Assert.IsNotNull(loadedCard);
            var book = loadedCard as BookCard;
            Assert.AreEqual("Original Book", book.Title);
        }

        [TestMethod]
        public void AddUpdate_DeleteCard_SequentialOperations()
        {
            // Arrange
            var card1 = new BookCard { Id = "book_#1", Title = "Book 1" };
            var card2 = new BookCard { Id = "book_#2", Title = "Book 2" };

            // Act - Add
            _storageService.AddCard(card1);
            _storageService.AddCard(card2);
            Assert.AreEqual(2, _storageService.GetAllCards().Count());

            // Act - Update
            card1.Title = "Updated Book 1";
            _storageService.UpdateCard(card1);
            var updated = _storageService.GetCardById("book_#1") as BookCard;
            Assert.AreEqual("Updated Book 1", updated.Title);

            // Act - Delete
            _storageService.DeleteCard("book_#1");

            // Assert
            Assert.AreEqual(1, _storageService.GetAllCards().Count());
            Assert.IsNull(_storageService.GetCardById("book_#1"));
        }

        [TestMethod]
        public void GetAllCards_WithMultipleDifferentTypes_ReturnAllTypes()
        {
            // Arrange
            _storageService.AddCard(new BookCard { Id = "book_#1" });
            _storageService.AddCard(new PatentCard { Id = "patent_#2" });
            _storageService.AddCard(new MagazineCard { Id = "magazine_#3" });

            // Act
            var allCards = _storageService.GetAllCards().ToList();

            // Assert
            Assert.AreEqual(3, allCards.Count);
            Assert.AreEqual(1, allCards.OfType<BookCard>().Count());
            Assert.AreEqual(1, allCards.OfType<PatentCard>().Count());
            Assert.AreEqual(1, allCards.OfType<MagazineCard>().Count());
        }

        [TestMethod]
        public void GetCardByPredicate_AndGetCardById_ReturnSameCard()
        {
            // Arrange
            var bookCard = new BookCard { Id = "book_#123", Title = "Test Book" };
            _storageService.AddCard(bookCard);

            // Act
            var cardById = _storageService.GetCardById("book_#123");
            var cardByPredicate = _storageService.GetCardByPredicate(c => c.Id == "book_#123");

            // Assert
            Assert.AreEqual(cardById.Id, cardByPredicate.Id);
            Assert.AreEqual(cardById.GetType(), cardByPredicate.GetType());
        }

        #endregion
    }
}
