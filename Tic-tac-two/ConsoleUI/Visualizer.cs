using GameBrain;

namespace ConsoleUI;

public static class Visualizer
{
    public static void DrawBoard(TicTacTwoBrain gameInstance)
    {
        var coordinates = gameInstance.GridCoordinates;
        var gridSize = gameInstance.GridSize;
        
        Console.Write("  ");
        for (var x = 0; x < gameInstance.DimX; x++)
        {
            Console.Write($" {x}  ");
        }
        Console.WriteLine(); 
        
        for (var y = 0; y < gameInstance.DimY; y++)
        {
            Console.Write($" {y}");
            for (var x = 0; x < gameInstance.DimX; x++)
            {
                if (y >= coordinates[1] && y <= coordinates[1] + gridSize - 1 && x >= coordinates[0] && x <= coordinates[0] + gridSize - 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
            
                Console.Write(" " + DrawGamePiece(gameInstance.GameBoard[x][y]) + " ");
                if (x == gameInstance.DimX - 1) continue;
                if (y <= coordinates[1] + gridSize - 1 && x == coordinates[0] + gridSize - 1)
                {
                    Console.ResetColor();
                }
                Console.Write("|");
                Console.ResetColor();
            }
            Console.ResetColor();
            Console.WriteLine();
        
            if (y == gameInstance.DimY - 1) continue;
            Console.Write("  ");
            for (var x = 0; x < gameInstance.DimX; x++)
            {
                if (y >= coordinates[1] && y <= coordinates[1] + gridSize - 2 && x >= coordinates[0] && x <= coordinates[0] + gridSize - 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Console.Write("---");
                if (y <= coordinates[1] + gridSize - 2 && x == coordinates[0] + gridSize - 1)
                {
                    Console.ResetColor();
                }
                if (x != gameInstance.DimX - 1)
                {
                    Console.Write("+");
                }
                Console.ResetColor();
            }
            Console.ResetColor();
            Console.WriteLine();
        }
    }
    
    private static string DrawGamePiece(EGamePiece piece) =>
        piece switch
        {
            EGamePiece.O => "O",
            EGamePiece.X => "X",
            _ => " "
        };
}