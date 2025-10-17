namespace Module3_ClassLibrary.Tests
{
    [TestClass]
    public class FileSystemVisitorTests
    {
        private string _testRoot = string.Empty;

        [TestInitialize]
        public void SetUp()
        {
            _testRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testRoot);
            Directory.CreateDirectory(Path.Combine(_testRoot, "DirA"));
            Directory.CreateDirectory(Path.Combine(_testRoot, "DirB"));
            File.WriteAllText(Path.Combine(_testRoot, "file1.txt"), "test");
            File.WriteAllText(Path.Combine(_testRoot, "DirA", "file2.txt"), "test");
            File.WriteAllText(Path.Combine(_testRoot, "DirB", "file3.log"), "test");
        }

        [TestCleanup]
        public void TearDown()
        {
            if (Directory.Exists(_testRoot))
            {
                Directory.Delete(_testRoot, true);
            }
        }

        [TestMethod]
        public void Traverse_ReturnsAllFilesAndDirectories()
        {
            var visitor = new FileSystemVisitor(_testRoot);
            var results = visitor.Traverse().ToList();
            CollectionAssert.Contains(results, Path.Combine(_testRoot, "DirA"));
            CollectionAssert.Contains(results, Path.Combine(_testRoot, "DirB"));
            CollectionAssert.Contains(results, Path.Combine(_testRoot, "file1.txt"));
            CollectionAssert.Contains(results, Path.Combine(_testRoot, "DirA", "file2.txt"));
            CollectionAssert.Contains(results, Path.Combine(_testRoot, "DirB", "file3.log"));
        }

        [TestMethod]
        public void Traverse_WithFilter_ReturnsOnlyFiltered()
        {
            var visitor = new FileSystemVisitor(_testRoot, path => path.EndsWith(".txt"));
            var results = visitor.Traverse().ToList();
            CollectionAssert.Contains(results, Path.Combine(_testRoot, "file1.txt"));
            CollectionAssert.Contains(results, Path.Combine(_testRoot, "DirA", "file2.txt"));
            Assert.AreEqual(2, results.Count);
        }

        [TestMethod]
        public void Traverse_AbortViaEvent_StopsTraversal()
        {
            var visitor = new FileSystemVisitor(_testRoot);
            bool aborted = false;
            visitor.FileFound += (s, e) => { e.Abort = true; aborted = true; };
            var results = visitor.Traverse().ToList();
            Assert.IsTrue(aborted);
            Assert.IsTrue(results.Count < 5);
        }

        [TestMethod]
        public void Traverse_ExcludeViaEvent_RemovesFromResults()
        {
            var visitor = new FileSystemVisitor(_testRoot);
            visitor.FileFound += (s, e) => { if (e.Path.EndsWith("file1.txt")) e.Exclude = true; };
            var results = visitor.Traverse().ToList();
            CollectionAssert.DoesNotContain(results, Path.Combine(_testRoot, "file1.txt"));
        }

        [TestMethod]
        public void Traverse_Events_AreRaised()
        {
            var visitor = new FileSystemVisitor(_testRoot);
            bool startRaised = false, finishRaised = false, fileFoundRaised = false, dirFoundRaised = false, filteredFileFoundRaised = false, filteredDirFoundRaised = false;
            visitor.Start += (s, e) => startRaised = true;
            visitor.Finish += (s, e) => finishRaised = true;
            visitor.FileFound += (s, e) => fileFoundRaised = true;
            visitor.DirectoryFound += (s, e) => dirFoundRaised = true;
            visitor.FilteredFileFound += (s, e) => filteredFileFoundRaised = true;
            visitor.FilteredDirectoryFound += (s, e) => filteredDirFoundRaised = true;

            var _ = visitor.Traverse().ToList();

            Assert.IsTrue(startRaised);
            Assert.IsTrue(finishRaised);
            Assert.IsTrue(fileFoundRaised);
            Assert.IsTrue(dirFoundRaised);
            Assert.IsTrue(filteredFileFoundRaised);
            Assert.IsTrue(filteredDirFoundRaised);
        }
    }
}
