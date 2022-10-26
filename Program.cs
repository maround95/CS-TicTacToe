class Program {
  private static readonly int GRID_SIZE = 3; // Change to play on a bigger grid. (Min: 2)

  static void Main(string[] args) {
    char[] board = Enumerable.Range(0, GRID_SIZE * GRID_SIZE).Select(_ => ' ').ToArray();
    string[] legend = Enumerable.Range(0, GRID_SIZE * GRID_SIZE).Select(n => n.ToString()).ToArray();
    bool xTurn = true;
    char winner;
    string error = default(string);

    do {
      Console.Clear();
      if (error != null) Console.WriteLine($"Error: {error}\n");
      error = default(string);

      Console.WriteLine($"Legend:\n{Render(legend)}\n");
      Console.WriteLine($"Current Board:\n{Render(board)}\n");

      char currentPlayer = xTurn ? 'X' : 'O';

      Console.Write($"{currentPlayer}'s Turn. Pick an index: ");
      string userInput = Console.ReadLine();
      if (!int.TryParse(userInput, out int n) || n < 0 || n >= GRID_SIZE * GRID_SIZE) {
        error = $"Invalid index '{userInput}'";
        continue;
      } else if (board[n] != ' ') {
        error = $"Index {n} is already filled!";
        continue;
      }

      board[n] = currentPlayer;
      xTurn = !xTurn;
    } while(!CheckWinner(board, out winner));

    Console.Clear();
    Console.WriteLine(winner != default(char) ? $"{winner} wins!" : "It's a tie");
    Console.WriteLine($"\nFinal Board:\n{Render(board)}");
  }

  static string Render(string[] grid) {
    int width = grid.Select(s => s.Length).Max();
    return String.Join("\n", Enumerable.Chunk(grid, GRID_SIZE)
      .Select(row => string.Join("|", row.Select(s => s.PadLeft(width + 1).PadRight(width + 2))))
      .SelectMany(row => new string[] { row, new string('-', row.Length) })
      .SkipLast(1));
  }

  static string Render(char[] grid) {
    return Render(grid.Select(c => c.ToString()).ToArray());
  }

  static bool CheckWinner(char[] grid, out char winner) {
    winner = default(char);
    if (grid.Where(c => c != ' ').Count() == GRID_SIZE * GRID_SIZE) return true;

    Func<char[][], char> checkRow = arr => arr
      .Where(r => r.Where(c => c == r[0] && c != ' ').Count() == GRID_SIZE)
      .Select(r => r[0])
      .FirstOrDefault();

    Func<char[][], char> checkDiagonal = arr => {
      char[] diagonal = arr.Select((row, i) => row.Skip(i).First()).Where(c => c != ' ').ToArray();
      return diagonal.Distinct().Count() == 1 && diagonal.Length == GRID_SIZE ?
        diagonal[0] : default(char);
    };

    char[][] chunked = Enumerable.Chunk(grid, GRID_SIZE).ToArray();
    char[][] mirrored = chunked.Select(r => r.Reverse().ToArray()).ToArray();

    foreach (char c in new char[] { checkRow(chunked), checkRow(Transpose(chunked)),
        checkDiagonal(chunked), checkDiagonal(mirrored) }) {
      if (c != default(char)) { winner = c; return true; };
    }

    return false;
  }

  static T[][] Transpose<T>(T[][] arr) {
    T[][] res = new T[arr[0].Length][];
    for (int i = 0; i < arr[0].Length; i++) {
      res[i] = new T[arr.Length];
      for (int j = 0; j < arr.Length; j++) {
        res[i][j] = arr[j][i];
      }
    }
    return res;
  }
}
