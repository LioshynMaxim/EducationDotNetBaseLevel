using Xunit;

namespace UnitTestingClassLibraryGameOfLife
{
    public class GameOfLifeTests
    {
        [Fact]
        public void Constructor_CreatesEmptyGrid()
        {
            // Arrange & Act
            var game = new GameOfLife(4, 8);

            // Assert
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Assert.False(game.GetCell(row, col));
                }
            }
        }

        [Fact]
        public void SetCell_SetsLivingCell()
        {
            // Arrange
            var game = new GameOfLife(4, 8);

            // Act
            game.SetCell(1, 3, true);

            // Assert
            Assert.True(game.GetCell(1, 3));
        }

        [Fact]
        public void CountLiveNeighbours_NoNeighbours_ReturnsZero()
        {
            // Arrange
            var game = new GameOfLife(4, 8);
            game.SetCell(1, 3, true);

            // Act
            int count = game.CountLiveNeighbours(0, 0);

            // Assert
            Assert.Equal(0, count);
        }

        [Fact]
        public void CountLiveNeighbours_WithNeighbours_ReturnsCorrectCount()
        {
            // Arrange
            var game = new GameOfLife(4, 8);
            game.SetCell(1, 3, true);
            game.SetCell(2, 2, true);
            game.SetCell(2, 3, true);

            // Act
            int count = game.CountLiveNeighbours(1, 3);

            // Assert
            Assert.Equal(2, count);
        }

        [Fact]
        public void Rule1_LiveCellWithFewerThanTwoNeighbours_Dies()
        {
            // Arrange
            var game = new GameOfLife(3, 3);
            game.SetCell(1, 1, true);
            game.SetCell(0, 0, true);

            // Act
            var nextGen = game.NextGeneration();

            // Assert
            Assert.False(nextGen.GetCell(1, 1));
        }

        [Fact]
        public void Rule2_LiveCellWithMoreThanThreeNeighbours_Dies()
        {
            // Arrange
            var game = new GameOfLife(3, 3);
            game.SetCell(1, 1, true);
            game.SetCell(0, 0, true);
            game.SetCell(0, 1, true);
            game.SetCell(1, 0, true);
            game.SetCell(2, 0, true);
            game.SetCell(2, 1, true);

            // Act
            var nextGen = game.NextGeneration();

            // Assert
            Assert.False(nextGen.GetCell(1, 1));
        }

        [Fact]
        public void Rule3_LiveCellWithTwoOrThreeNeighbours_Lives()
        {
            // Arrange
            var game = new GameOfLife(3, 3);
            game.SetCell(1, 1, true);
            game.SetCell(0, 1, true);
            game.SetCell(1, 0, true);

            // Act
            var nextGen = game.NextGeneration();

            // Assert
            Assert.True(nextGen.GetCell(1, 1));
        }

        [Fact]
        public void Rule4_DeadCellWithThreeNeighbours_BecomesAlive()
        {
            // Arrange
            var game = new GameOfLife(3, 3);
            game.SetCell(0, 1, true);
            game.SetCell(1, 0, true);
            game.SetCell(1, 2, true);

            // Act
            var nextGen = game.NextGeneration();

            // Assert
            Assert.True(nextGen.GetCell(1, 1));
        }

        [Fact]
        public void ExampleFromKata_ProducesExpectedResult()
        {
            // Arrange
            string input = 
@"4 8
........
....*...
...**...
........";

            string expected = 
@"4 8
........
...**...
...**...
........";

            // Act
            var game = GameOfLife.Parse(input);
            var nextGen = game.NextGeneration();
            string result = nextGen.ToGridString();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ToGridString_FormatsOutputCorrectly()
        {
            // Arrange
            var game = new GameOfLife(2, 3);
            game.SetCell(0, 1, true);
            game.SetCell(1, 0, true);

            // Act
            string result = game.ToGridString();

            // Assert
            string expected = "2 3\r\n.*.\r\n*..";
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Parse_ParsesInputStringCorrectly()
        {
            // Arrange
            string input = @"2 3
.*.
*..";

            // Act
            var game = GameOfLife.Parse(input);

            // Assert
            Assert.True(game.GetCell(0, 1));
            Assert.True(game.GetCell(1, 0));
            Assert.False(game.GetCell(0, 0));
        }

        [Fact]
        public void BlockPattern_RemainsStable()
        {
            // Arrange - Block pattern (2x2 square)
            var game = new GameOfLife(4, 4);
            game.SetCell(1, 1, true);
            game.SetCell(1, 2, true);
            game.SetCell(2, 1, true);
            game.SetCell(2, 2, true);

            // Act
            var nextGen = game.NextGeneration();

            // Assert - Block should remain unchanged
            Assert.True(nextGen.GetCell(1, 1));
            Assert.True(nextGen.GetCell(1, 2));
            Assert.True(nextGen.GetCell(2, 1));
            Assert.True(nextGen.GetCell(2, 2));
        }

        [Fact]
        public void BlinkerPattern_Oscillates()
        {
            // Arrange - Blinker pattern (horizontal line)
            var game = new GameOfLife(5, 5);
            game.SetCell(2, 1, true);
            game.SetCell(2, 2, true);
            game.SetCell(2, 3, true);

            // Act
            var nextGen = game.NextGeneration();

            // Assert - Should become vertical line
            Assert.False(nextGen.GetCell(2, 1));
            Assert.True(nextGen.GetCell(1, 2));
            Assert.True(nextGen.GetCell(2, 2));
            Assert.True(nextGen.GetCell(3, 2));
            Assert.False(nextGen.GetCell(2, 3));
        }
    }
}