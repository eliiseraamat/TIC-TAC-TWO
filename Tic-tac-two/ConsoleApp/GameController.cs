using DAL;
using GameBrain;
using MenuSystem;

namespace Tic_tac_two2;

public static class GameController
{
    //private static readonly IConfigRepository ConfigRepository = new ConfigRepositoryJson();
    private static readonly IConfigRepository ConfigRepository = new ConfigRepositoryDb();
    //private static readonly IGameRepository GameRepository = new GameRepositoryJson();
    private static readonly IGameRepository GameRepository = new GameRepositoryDb();
    
    public static string MainLoop()
    {
        var chosenConfigShortcut = ChooseConfiguration();

        if (!int.TryParse(chosenConfigShortcut, out var configNo))
        {
            return chosenConfigShortcut;
        }

        var chosenConfig = ConfigRepository.GetConfigurationByName(ConfigRepository.GetConfigurationNames()[configNo]);
        
        if (chosenConfig.Name == "Customize")
        {
            chosenConfig = OptionsController.MainLoop(chosenConfig);
        }
        
        var gameInstance= new TicTacTwoBrain(chosenConfig);

        return GameLoop(gameInstance);

    }

    private static string GameLoop(TicTacTwoBrain gameInstance)
    {
        string choice;
        do
        {
            Console.Clear();
            
            var winner = Game(gameInstance);

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
    
    private static EGamePiece Game(TicTacTwoBrain gameInstance)
    {
        do
        {
            Console.Clear();
            
            Console.WriteLine("TIC-TAC-TWO");
            
            ConsoleUI.Visualizer.DrawBoard(gameInstance);
            
            var player = gameInstance.NextMoveBy;
            
            var name = player == EGamePiece.X ? gameInstance.PlayerX : gameInstance.PlayerO;
            
            var playerPieces = player == EGamePiece.X ? gameInstance.PlayerXPieces : gameInstance.PlayerOPieces;

            var userChoice = Display(gameInstance, name, playerPieces);

            if (userChoice == "-")
            {
                return EGamePiece.Empty;
            }
            
            MakeMove(gameInstance, userChoice, playerPieces);

            var winnerPiece = gameInstance.WinningCondition();

            if (winnerPiece != EGamePiece.Empty)
            {
                return winnerPiece;
            }

        } while (true);
    }

    private static string Display(TicTacTwoBrain gameInstance, string name, int playerPieces)
    {
        Console.WriteLine($"{name}'s turn");
        
        Console.WriteLine();
        
        Console.WriteLine($"{gameInstance.PlayerX} has {gameInstance.PlayerXPieces} pieces left");
        
        Console.WriteLine($"{gameInstance.PlayerO} has {gameInstance.PlayerOPieces} pieces left");

        var userChoice = DisplayChoices(gameInstance, playerPieces);

        return userChoice;
    }

    private static string DisplayChoices(TicTacTwoBrain gameInstance, int playerPieces)
    {
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
            case "S":
                break;
            case "P":
                Console.WriteLine("Give old coordinates and new coordinates for your piece <x,y>;<x,y>:");
                break;
            case "G":
                Console.WriteLine("Give grid coordinates <x,y>:");
                break;
            case "N" :
                Console.WriteLine("Give the coordinates of the new piece <x,y>:");
                break;
            default:
                Console.WriteLine("Give the coordinates of the new piece <x,y> or save game (S):");
                break;
        }

        return userChoice;
    }

    private static string EndGame(EGamePiece player, TicTacTwoBrain gameInstance)
    {
        Console.Clear();
            
        Console.WriteLine("TIC-TAC-TWO");
            
        ConsoleUI.Visualizer.DrawBoard(gameInstance);

        if (player == EGamePiece.Empty)
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
            
            choices.Add("S");
            Console.WriteLine("S) Save game");

            if (choices.Count == 1)
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
        do
        {
            var input = Console.ReadLine()!;
            if (userChoice == "" && input.ToUpper().Equals("S", StringComparison.CurrentCultureIgnoreCase))
            {
                GameRepository.SaveGame(gameInstance.GetGameStateJson(), gameInstance.GetGameConfigName());
                Console.WriteLine("Saved game, give coordinates");
                continue;
            }
            if (userChoice == "P")
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

    private static void MakeMove(TicTacTwoBrain gameInstance, string userChoice, int playerPieces)
    {
        var madeMove = false;
        do
        {
            if (userChoice == "S")
            {
                GameRepository.SaveGame(gameInstance.GetGameStateJson(), gameInstance.GetGameConfigName());
                Console.WriteLine("Saved game");
                userChoice = DisplayChoices(gameInstance, playerPieces);
            }
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
    
    public static string LoadGame()
    {
        var gameMenuItems = new List<MenuItem>();

        for (var i = 0; i < GameRepository.GetGameNames().Count; i++)
        {
            var returnValue = i.ToString();
            gameMenuItems.Add(new MenuItem()
            {
                Title = GameRepository.GetGameNames()[i],
                Shortcut = (i+1).ToString(),
                MenuItemAction = () => returnValue
            });
        }

        if (gameMenuItems.Count == 0)
        {
            return "r";
        }
    
        var configMenu = new Menu(
            EMenuLevel.Secondary, 
            "TIC-TAC-TWO - load game", 
            gameMenuItems,
            isCustomMenu: true);
        
        var chosenGameShortcut = configMenu.Run();
        
        if (!int.TryParse(chosenGameShortcut, out var configNo))
        {
            return chosenGameShortcut;
        }

        var chosenGameName = GameRepository.GetGameNames()[configNo];
        
        var chosenGame = GameRepository.LoadGame(chosenGameName);
        
        var gameInstance= new TicTacTwoBrain(chosenGame);

        return GameLoop(gameInstance);
    }
}