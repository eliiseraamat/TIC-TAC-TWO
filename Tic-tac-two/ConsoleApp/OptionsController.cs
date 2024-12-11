using GameBrain;

namespace Tic_tac_two2;

public static class OptionsController
{
    public static GameConfiguration MainLoop(GameConfiguration chosenConfig)
    {
        Console.Clear();

        var minValue = 3;
        var maxValue = 40;

        Console.WriteLine($"Choose board size ({minValue} - {maxValue}): ");
        
        var boardSize = GetInput(minValue, maxValue);
        chosenConfig.BoardSize = boardSize;
        
        Console.WriteLine($"Choose grid size {(minValue == boardSize ? $"({minValue})" : $"({minValue} - {boardSize})")}: ");
        
        var gridSize = GetInput(minValue, boardSize);
        chosenConfig.GridSize = gridSize;

        var piecesNr = (int)Math.Ceiling((boardSize * boardSize / 2m));
        
        Console.WriteLine($"Choose number of pieces for each player {(minValue == piecesNr ? $"({minValue})" : $"({minValue} - {piecesNr})")}: ");
        
        chosenConfig.Pieces = GetInput(minValue, piecesNr);
        
        Console.WriteLine($"Choose number of pieces in line to win {(minValue == gridSize ? $"({minValue})" : $"({minValue} - {gridSize})")}: ");
        
        chosenConfig.WinCondition = GetInput(minValue, gridSize);
        
        Console.WriteLine($"Choose number of moves for one player after which the player have more options: ");
        
        chosenConfig.MovePieceAfterMoves = GetInput(0, int.MaxValue);
        
        Console.WriteLine("Choose grid coordinates <x, y>:");

        var coordinates = GetCoordinates(boardSize, gridSize);
        chosenConfig.GridCoordinates = coordinates;
        
        Console.WriteLine("Choose name for player X:");
        
        var playerX = Console.ReadLine()!;
        chosenConfig.PlayerX = playerX;

        var playerO = getPlayerName(playerX);
        chosenConfig.PlayerO = playerO;

        return chosenConfig;
    }
    
    private static int GetInput(int minValue, int maxValue)
    {
        do
        {
            var input = Console.ReadLine()!;
            try
            {
                var number = int.Parse(input);
                if (number >= minValue && number <= maxValue)
                {
                    return number;
                }
                Console.WriteLine("Please choose number in given range, choose again");
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid input, choose again");
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

    private static string getPlayerName(string playerX)
    {
        Console.WriteLine("Choose name for player 0:");
        do
        {
            var playerO = Console.ReadLine()!;
            
            if (playerO != playerX)
            {
                return playerO;
            }
            Console.WriteLine("Player 0 name can't be the same as player X name, choose again:");
            
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