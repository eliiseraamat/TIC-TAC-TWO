using DAL;
using GameBrain;

namespace Tic_tac_two2;

public static class OptionsController
{
    public static GameConfiguration MainLoop(GameConfiguration chosenConfig)
    {
        Console.Clear();
        
        Console.WriteLine("Choose board size (min 3)\ngrid size (min 3)\nnumber of pieces for each player (min 3)\nnumber of pieces in line to win (min 3)\nnumber of moves for one player after which the player can move grid: <a, b, c, d, e>");

        var input = GetInput();

        chosenConfig.BoardSize = input[0];
        chosenConfig.GridSize = input[1];
        chosenConfig.Pieces = input[2];
        chosenConfig.WinCondition = input[3];
        chosenConfig.MovePieceAfterMoves = input[4];
        
        Console.WriteLine("Choose grid coordinates <x, y>:");

        var coordinates = GetCoordinates(input[0], input[1]);

        chosenConfig.GridCoordinates = coordinates;
        
        Console.WriteLine("Choose name for player X:");
        
        var playerX = Console.ReadLine()!;

        chosenConfig.PlayerX = playerX;
        
        Console.WriteLine("Choose name for player O:");
        
        var playerO = Console.ReadLine()!;
        
        chosenConfig.PlayerO = playerO;
        
        return chosenConfig;
    }

    private static List<int> GetInput()
    {
        do
        {
            var input = Console.ReadLine()!;
            try
            {
                var inputSplit = input.Split(',');
                var list = inputSplit.Select(number => int.Parse(number)).ToList();
                if (list[1] > list[0])
                {
                    Console.WriteLine("Grid size cannot be bigger than board size!");
                } 
                else if (list[3] > list[1])
                {
                    Console.WriteLine("Number of pieces in line to win cannot be greater than grid size!"); 
                } 
                else if (list[3] > list[2])
                {
                    Console.WriteLine("Number of pieces in line to win cannot be greater than number of pieces for each player !"); 
                }
                else if (list is [>= 3, >= 3, >= 3, >= 3, >= 0])
                {
                    return list;
                }
                else
                {
                    Console.WriteLine("Invalid input");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid input.");
            }
        } while (true);
    }

    private static List<int> GetCoordinates(int boardSize, int gridSize)
    {
        do
        {
            var input = Console.ReadLine()!;
            try
            {
                var inputSplit = input.Split(",");
                if (inputSplit.Length == 2)
                {
                    var list = inputSplit.Select(number => int.Parse(number)).ToList();

                    if (list[0] >= 0 && list[1] >= 0 && list[0] + gridSize - 1 < boardSize && list[1] + gridSize - 1 < boardSize)
                    {
                        return list;
                    }
                    Console.WriteLine("Grid must fit inside the board!");
                }
                else
                {
                    Console.WriteLine("Invalid input.");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid input.");
            }
        } while (true);
    }

    public static string SetStartingPiece()
    {
        Console.Clear();
        Console.WriteLine("Choose if X starts (X) or O starts (O):");
    
        var chosenPiece = GetStartingPiece();
    
        if (chosenPiece == "X")
        {
            TicTacTwoBrain.SetStartingPiece(EGamePiece.X);
        }
        else
        {
            TicTacTwoBrain.SetStartingPiece(EGamePiece.O);
        }
        return "";
    }

    private static string GetStartingPiece()
    {
        do
        {
            var input = Console.ReadLine()!;
            if (input.Equals("X", StringComparison.CurrentCultureIgnoreCase) || input.Equals("O", StringComparison.CurrentCultureIgnoreCase))
            {
                return input.ToUpper();
            }
            Console.WriteLine("Invalid input.");
        } while (true);
    }
}