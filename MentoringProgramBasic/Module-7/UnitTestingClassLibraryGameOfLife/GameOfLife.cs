namespace UnitTestingClassLibraryGameOfLife;

public class GameOfLife
{
    private readonly bool[,] _grid;
    private readonly int _rows;
    private readonly int _columns;

    public GameOfLife(int rows, int columns)
    {
        _rows = rows;
        _columns = columns;
        _grid = new bool[rows, columns];
    }

    public GameOfLife(bool[,] initialGrid)
    {
        _rows = initialGrid.GetLength(0);
        _columns = initialGrid.GetLength(1);
        _grid = (bool[,])initialGrid.Clone();
    }

    public void SetCell(int row, int column, bool isAlive)
    {
        if (row >= 0 && row < _rows && column >= 0 && column < _columns)
        {
            _grid[row, column] = isAlive;
        }
    }

    public bool GetCell(int row, int column)
    {
        if (row >= 0 && row < _rows && column >= 0 && column < _columns)
        {
            return _grid[row, column];
        }
        return false;
    }

    public int CountLiveNeighbours(int row, int column)
    {
        int count = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;

                int newRow = row + i;
                int newCol = column + j;

                if (newRow >= 0 && newRow < _rows && newCol >= 0 && newCol < _columns)
                {
                    if (_grid[newRow, newCol])
                    {
                        count++;
                    }
                }
            }
        }
        return count;
    }

    public GameOfLife NextGeneration()
    {
        bool[,] nextGrid = new bool[_rows, _columns];

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                int liveNeighbours = CountLiveNeighbours(row, col);
                bool currentCell = _grid[row, col];

                if (currentCell)
                {
                    // Rule 1: Any live cell with fewer than two live neighbours dies
                    // Rule 2: Any live cell with more than three live neighbours dies
                    // Rule 3: Any live cell with two or three live neighbours lives
                    nextGrid[row, col] = liveNeighbours == 2 || liveNeighbours == 3;
                }
                else
                {
                    // Rule 4: Any dead cell with exactly three live neighbours becomes alive
                    nextGrid[row, col] = liveNeighbours == 3;
                }
            }
        }

        return new GameOfLife(nextGrid);
    }

    public string ToGridString()
    {
        string result = $"{_rows} {_columns}\r\n";
        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                result += _grid[row, col] ? "*" : ".";
            }
            if (row < _rows - 1)
            {
                result += "\r\n";
            }
        }
        return result;
    }

    public static GameOfLife Parse(string input)
    {
        //string[] lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        string[] lines = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length == 0)
        {
            throw new ArgumentException("Input cannot be empty");
        }

        string[] dimensions = lines[0].Split(' ');
        int rows = int.Parse(dimensions[0]);
        int columns = int.Parse(dimensions[1]);

        var game = new GameOfLife(rows, columns);

        for (int i = 1; i <= rows && i < lines.Length; i++)
        {
            string line = lines[i];
            for (int j = 0; j < columns && j < line.Length; j++)
            {
                game.SetCell(i - 1, j, line[j] == '*');
            }
        }

        return game;
    }
}