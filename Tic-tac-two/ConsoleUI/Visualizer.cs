using GameBrain;

namespace ConsoleUI;

public static class Visualizer
{
    public static void DrawBoard(TicTacTwoBrain gameInstance)
    {
        for (var y = 0; y < gameInstance.DimY; y++)
        {
            for (var x = 0; x < gameInstance.DimX; x++)
            {
                if (0 < y && y < 4 && (x == 1 || x == 2)) 
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
            
                Console.Write(" " + DrawGamePiece(gameInstance.GameBoard[x, y]) + " ");
                if (x == gameInstance.DimX - 1) continue;
                Console.Write("|");
                Console.ResetColor();
            }
            Console.WriteLine();
        
            if (y == gameInstance.DimY - 1) continue;
            for (var x = 0; x < gameInstance.DimX; x++)
            {
                if ((y == 2 || y ==1) && x > 0 && x < 4)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Console.Write("---");
                if (x != gameInstance.DimX - 1)
                {
                    Console.Write("+");
                }
                Console.ResetColor();
            }
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