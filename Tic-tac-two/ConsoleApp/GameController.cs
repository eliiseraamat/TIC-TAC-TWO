using DAL;
using GameBrain;
using MenuSystem;

namespace Tic_tac_two2;

public static class GameController
{
    private static readonly IConfigRepository ConfigRepository = new ConfigRepositoryJson();
    private static readonly IGameRepository GameRepository = new GameRepositoryJson();
    
    public static string MainLoop()
    {
        var chosenConfigShortcut = ChooseConfiguration();
        
        GameConfiguration chosenConfig;
 
        if (!int.TryParse(chosenConfigShortcut, out var configNo))
        {
            ChooseConfiguration();
        }

        chosenConfig = ConfigRepository.GetConfigurationByName(ConfigRepository.GetConfigurationNames()[configNo]);
        
        if (chosenConfig.Name == "Customize")
        {
            chosenConfig = OptionsController.MainLoop(chosenConfig);
        }
        
        var gameInstance= new TicTacTwoBrain(chosenConfig);
        
        string choice;
        do
        {
            Console.Clear();
            
            var winner = GameLoop(gameInstance);

            choice = EndGame(winner, gameInstance);

            if (choice == "R")
            {
                gameInstance.ResetGame();
            }
            else
            {
                choice = "M";
            }

        } while (choice == "R");

        return "r";

    }

    private static string GameLoop(TicTacTwoBrain gameInstance)
    {
        do
        {
            Console.Clear();
            
            Console.WriteLine("TIC-TAC-TWO");
            
            ConsoleUI.Visualizer.DrawBoard(gameInstance);
            
            var player = gameInstance.NextMoveBy;
            
            var name = player == EGamePiece.X ? gameInstance.PlayerX : gameInstance.PlayerO;
            
            var playerPieces = player == EGamePiece.X ? gameInstance.PlayerXPieces : gameInstance.PlayerOPieces;

            var userChoice = DisplayChoices(gameInstance, name, playerPieces);

            if (userChoice == "-")
            {
                return default!;
            }
            
            MakeMove(gameInstance, userChoice);

            if (gameInstance.WinningCondition())
            {
                return name;
            }

        } while (true);
    }

    private static string DisplayChoices(TicTacTwoBrain gameInstance, string name, int playerPieces)
    {
        Console.WriteLine($"{name}'s turn");
        
        Console.WriteLine();
        
        Console.WriteLine($"{gameInstance.PlayerX} has {gameInstance.PlayerXPieces} pieces left");
        
        Console.WriteLine($"{gameInstance.PlayerO} has {gameInstance.PlayerOPieces} pieces left");
        
        Console.WriteLine();
        
        var userChoice = "";
        
        if (gameInstance.EnoughMovesForMoreOptions())
        {
            userChoice = GetUserChoice(playerPieces, gameInstance);
        }
        
        switch (userChoice)
        {
            case "-":
                break;
            case "P":
                Console.WriteLine("Give old coordinates and new coordinates for your piece <x,y>;<x,y> or save:");
                break;
            case "G":
                Console.WriteLine("Give grid coordinates <x,y> or save:");
                break;
            default:
                Console.WriteLine("Give the coordinates of the new piece <x,y> or save:");
                break;
        }

        return userChoice;
    }

    private static string EndGame(string player, TicTacTwoBrain gameInstance)
    {
        Console.Clear();
            
        Console.WriteLine("TIC-TAC-TWO");
            
        ConsoleUI.Visualizer.DrawBoard(gameInstance);

        if (player == default!)
        {
            Console.WriteLine("It's a draw!");
        }
        else
        {
            Console.WriteLine($"{player} won!");
        }
        
        Console.WriteLine("Would you like to reset game (R) or return to main menu (M)");
        
        do
        {
            var input = Console.ReadLine()!;
            switch (input.ToUpper())
            {
                case "R":
                    return "R";
                case "M":
                   return "M";
                default:
                    Console.WriteLine("Invalid input!");
                    break;
            }
        } while (true);
    }

    private static string ChooseConfiguration()
    {
        var configMenuItems = new List<MenuItem>();

        for (var i = 0; i < ConfigRepository.GetConfigurationNames().Count; i++)
        {
            var returnValue = i.ToString();
            configMenuItems.Add(new MenuItem()
            {
                Title = ConfigRepository.GetConfigurationNames()[i],
                Shortcut = (i+1).ToString(),
                MenuItemAction = () => returnValue
            });
        }
        
        // configMenuItems.Add(new MenuItem()
        // {
        //     Title = "Customize",
        //     Shortcut = "C",
        //     MenuItemAction = () => "C"
        // });
    
        var configMenu = new Menu(
            EMenuLevel.Secondary, 
            "TIC-TAC-TWO - choose game config", 
            configMenuItems,
            isCustomMenu: true);

       return configMenu.Run();
    }

    private static string GetUserChoice(int playerPieces, TicTacTwoBrain gameInstance)
    {
        do
        {
            var choices = new List<string>();
            
            Console.WriteLine("Choose one of the following options:");

            if (playerPieces > 0 && !gameInstance.IsGridFull())
            {
                choices.Add("N");
                Console.WriteLine("N) Put a new piece on the grid");
            }

            if (!gameInstance.IsGridFull())
            {
                choices.Add("P");
                Console.WriteLine("P) Move one of your pieces to another spot in the grid.");
            }

            if (gameInstance.GridSize < gameInstance.DimX)
            {
                choices.Add("G");
                Console.WriteLine("G) Move grid one spot horizontally, vertically or diagonally");
            }

            if (choices.Count == 0)
            {
                return "-";
            }
            
            var input = Console.ReadLine()!;
            
            if (choices.Contains(input.ToUpper()))
            {
                return input.ToUpper();
            }
            
        } while (true);
    }

    private static List<int> GetCoordinates(string userChoice, TicTacTwoBrain gameInstance)
    {
        // TODO: save uude kohta???
        do
        {
            var input = Console.ReadLine()!;
            if (input.Equals("save", StringComparison.CurrentCultureIgnoreCase))
            {
                GameRepository.SaveGame(gameInstance.GetGameStateJson(), gameInstance.GetGameConfigName());
            } 
            else if (userChoice == "P")
            {
                try
                {
                    var inputSplit = input.Split(";");
                    var coordinates = new List<int>();
                    var oldPlace = inputSplit[0].Split(",");
                    var newPlace = inputSplit[1].Split(",");
                    if (inputSplit.Length != 2 || oldPlace.Length != 2 || newPlace.Length != 2)
                    {
                        Console.WriteLine("Invalid input");
                        continue;
                    }
                    coordinates.Add(int.Parse(oldPlace[0]));
                    coordinates.Add(int.Parse(oldPlace[1]));
                    coordinates.Add(int.Parse(newPlace[0]));
                    coordinates.Add(int.Parse(newPlace[1]));
                    if (coordinates[0] < 0 || coordinates[1] < 0 || coordinates[2] < 0 || coordinates[3] < 0)
                    {
                        Console.WriteLine("Invalid input");
                    }
                    else
                    {
                        return coordinates;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid input");
                } 
            }
            else
            {
                try
                {
                    var inputSplit = input.Split(",");
                    var coordinates = new List<int>();
                    if (inputSplit.Length != 2)
                    {
                        Console.WriteLine("Invalid input");
                        continue;
                    }
                    coordinates.Add(int.Parse(inputSplit[0]));
                    coordinates.Add(int.Parse(inputSplit[1]));
                    
                    if (coordinates[0] < 0 || coordinates[1] < 0)
                    {
                        Console.WriteLine("Invalid input");
                    }
                    else
                    {
                        return coordinates;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid input");
                }
            }

        } while (true);
    }

    private static void MakeMove(TicTacTwoBrain gameInstance, string userChoice)
    {
        var madeMove = false;
        do
        {
            var coordinates = GetCoordinates(userChoice, gameInstance);
            if ((userChoice == "" || userChoice == "N") && gameInstance.MakeAMove(coordinates[0], coordinates[1]))
            {
                madeMove = true;
            } else if (userChoice == "P" &&
                      gameInstance.ChangePieceLocation(coordinates[0], coordinates[1], coordinates[2],
                          coordinates[3]))
            {
                madeMove = true;
            } else if (userChoice == "G" && gameInstance.MoveGrid(coordinates[0], coordinates[1]))
            {
                madeMove = true;
            }
            else
            {
                Console.WriteLine("Wrong coordinates");
            }
        } while (madeMove == false);
    }
}