using DAL;
using GameBrain;
using MenuSystem;

namespace Tic_tac_two2;

public static class GameController
{
    private static IConfigRepository _configRepository = default!;
    private static IGameRepository _gameRepository = default!;
    
    private const string MainShortcut = "M";
    private const string ReturnShortcut = "R";
    private const string ExitShortcut = "E";
    private const string NoChoice = "-";

    private static readonly MenuItem MenuNewPiece = new MenuItem()
    {
        Title = "Put a new piece on the grid",
        Shortcut = "N",
        MenuItemAction = () => 0.ToString()
    };

    private static readonly MenuItem MenuMovePiece = new MenuItem()
    {
        Title = "Move one of your pieces to another spot in the grid",
        Shortcut = "P",
        MenuItemAction = () => 0.ToString()
    };

    private static readonly MenuItem MenuMoveGrid = new MenuItem()
    {
        Title = "Move grid one spot horizontally, vertically or diagonally",
        Shortcut = "G",
        MenuItemAction = () => 0.ToString()
    };

    private static readonly MenuItem MenuSave = new MenuItem()
    {
        Title = "Save game",
        Shortcut = "S",
        MenuItemAction = () => 0.ToString()
    };

    private static readonly MenuItem MenuQuit = new MenuItem()
    {
        Title = "Quit game",
        Shortcut = "Q",
        MenuItemAction = () => 0.ToString()
    };
    
    public static string MainLoop(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
        
        var chosenConfigShortcut = ChooseConfiguration();

        if (!int.TryParse(chosenConfigShortcut, out var configNo))
        {
            return chosenConfigShortcut;
        }

        var chosenConfig = _configRepository.GetConfigurationByName(_configRepository.GetConfigurationNames()[configNo]);
        
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

            if (winner == null)
            {
                return MainShortcut;
            }

            choice = EndGame(winner, gameInstance);

            if (choice == ReturnShortcut)
            {
                gameInstance.ResetGame();
            }
            else
            {
                choice = MainShortcut;
            }

        } while (choice == ReturnShortcut);

        return MainShortcut;
    }
    
    private static EGamePiece? Game(TicTacTwoBrain gameInstance)
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
            
            var move = MakeMove(gameInstance, userChoice, playerPieces);

            if (move == MenuQuit.Shortcut)
            {
                break;
            }

            var winnerPiece = gameInstance.WinningCondition();

            if (winnerPiece != EGamePiece.Empty)
            {
                return winnerPiece;
            }

        } while (true);

        return null;
    }

    public static string Display(TicTacTwoBrain gameInstance, string name, int playerPieces)
    {
        Console.WriteLine($"{name}'s turn");
        
        Console.WriteLine();
        
        Console.WriteLine($"{gameInstance.PlayerX} has {gameInstance.PlayerXPieces} pieces left");
        
        Console.WriteLine($"{gameInstance.PlayerO} has {gameInstance.PlayerOPieces} pieces left");

        var userChoice = DisplayChoices(gameInstance, playerPieces);

        return userChoice;
    }

    public static string DisplayChoices(TicTacTwoBrain gameInstance, int playerPieces)
    {
        Console.WriteLine();
        
        var userChoice = GetUserChoice(playerPieces, gameInstance);

        switch (userChoice)
        {
            case NoChoice:
            case "S":
            case "Q":
                break;
            case "P":
                Console.WriteLine("Give old coordinates and new coordinates for your piece <x,y>;<x,y>:");
                break;
            case "G":
                Console.WriteLine("Give grid coordinates <x,y>:");
                break;
            case "N":
                Console.WriteLine("Give the coordinates of the new piece <x,y>:");
                break;
        }

        return userChoice;
    }

    public static string EndGame(EGamePiece? player, TicTacTwoBrain gameInstance)
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
            var playerName = player == EGamePiece.X ? gameInstance.PlayerX : gameInstance.PlayerO;
            Console.WriteLine($"{playerName} won!");
        }
        
        Console.WriteLine("Would you like to reset game (R) or return to main menu (M)");
        
        do
        {
            var input = Console.ReadLine()!;
            switch (input.ToUpper())
            {
                case ReturnShortcut:
                    return ReturnShortcut;
                case MainShortcut:
                   return MainShortcut;
                default:
                    Console.WriteLine("Invalid input!");
                    break;
            }
        } while (true);
    }

    public static string ChooseConfiguration()
    {
        var configMenuItems = new List<MenuItem>();

        for (var i = 0; i < _configRepository.GetConfigurationNames().Count; i++)
        {
            var returnValue = i.ToString();
            configMenuItems.Add(new MenuItem()
            {
                Title = _configRepository.GetConfigurationNames()[i],
                Shortcut = (i+1).ToString(),
                MenuItemAction = () => returnValue
            });
        }
    
        var configMenu = new Menu(
            EMenuLevel.Deep, 
            "TIC-TAC-TWO - choose game config", 
            configMenuItems,
            isCustomMenu: true);

       return configMenu.Run();
    }

    private static string GetUserChoice(int playerPieces, TicTacTwoBrain gameInstance)
    {
        do
        {
            Console.WriteLine("Choose one of the following options: ");
            
            var choices = new List<MenuItem>();
            
            if (playerPieces > 0 && !gameInstance.IsGridFull())
            {
                choices.Add(MenuNewPiece);
            }
            
            var piece = gameInstance.NextMoveBy == EGamePiece.X ? EGamePiece.X : EGamePiece.O;
            if (gameInstance.EnoughMovesForMoreOptions() && !gameInstance.IsGridFull() && gameInstance.CheckPieceInBoard(piece))
            {
                choices.Add(MenuMovePiece);
            }

            if (gameInstance.EnoughMovesForMoreOptions() && gameInstance.GridSize < gameInstance.DimX)
            {
                choices.Add(MenuMoveGrid);
            }
            
            choices.Add(MenuSave);
            
            choices.Add(MenuQuit);
            
            if (choices.Count == 2)
            {
                return NoChoice;
            }
            
            foreach (var item in choices)
            {
                Console.WriteLine(item.Shortcut + ") " + item.Title);
            }
            
            var input = Console.ReadLine()!;
            
            if (choices.Select(item => item.Shortcut).Contains(input.ToUpper()))
            {
                return input.ToUpper();
            }
            
        } while (true);
    }

    public static List<int> GetCoordinates(string userChoice, TicTacTwoBrain gameInstance)
    {
        do
        {
            var input = Console.ReadLine()!;
            if (userChoice == MenuMovePiece.Shortcut)
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

    public static string MakeMove(TicTacTwoBrain gameInstance, string userChoice, int playerPieces)
    {
        var madeMove = false;
        do
        {
            if (userChoice == MenuSave.Shortcut)
            {
                var name = _gameRepository.SaveGame(gameInstance.GameState, gameInstance.GetGameConfigName(), EGamePiece.Empty);
                var passwords = _gameRepository.GetPasswords(name);
                Console.WriteLine($"Saved game. Game name: {name}, X password: {passwords[0]}, O password: {passwords[1]}");
                userChoice = DisplayChoices(gameInstance, playerPieces);
            }
            if (userChoice == MenuQuit.Shortcut)
            {
                return MenuQuit.Shortcut;
            }
            var coordinates = GetCoordinates(userChoice, gameInstance);
            if (userChoice == MenuNewPiece.Shortcut && gameInstance.MakeAMove(coordinates[0], coordinates[1]))
            {
                madeMove = true;
            } else if (userChoice == MenuMovePiece.Shortcut &&
                      gameInstance.ChangePieceLocation(coordinates[0], coordinates[1], coordinates[2],
                          coordinates[3]))
            {
                madeMove = true;
            } else if (userChoice == MenuMoveGrid.Shortcut && gameInstance.MoveGrid(coordinates[0], coordinates[1]))
            {
                madeMove = true;
            }
            else
            {
                Console.WriteLine("Wrong coordinates");
            }
        } while (madeMove == false);

        return "";
    }
    
    public static string LoadGame(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
        
        var gameMenuItems = new List<MenuItem>();

        for (var i = 0; i < _gameRepository.GetGameNames().Count; i++)
        {
            var returnValue = i.ToString();
            gameMenuItems.Add(new MenuItem()
            {
                Title = _gameRepository.GetGameNames()[i],
                Shortcut = (i+1).ToString(),
                MenuItemAction = () => returnValue
            });
        }

        if (gameMenuItems.Count == 0)
        {
            return ReturnShortcut;
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

        var chosenGameName = _gameRepository.GetGameNames()[configNo];
        
        var chosenGame = _gameRepository.LoadGame(chosenGameName);

        if (chosenGame == null)
        {
            return ReturnShortcut;
        }
        
        var gameInstance= new TicTacTwoBrain(chosenGame);

        var result = gameInstance.GameType == EGameType.TwoPlayer ? ValidatePasswords(chosenGameName) : ValidatePasswordsAi(gameInstance, chosenGameName);

        if (result == ExitShortcut)
        {
            return ReturnShortcut;
        }

        return gameInstance.GameType switch
        {
            EGameType.TwoPlayer => GameLoop(gameInstance),
            EGameType.OnePlayer => GameControllerAi.GameLoop(gameInstance, EGamePiece.Empty),
            _ => GameControllerAi.AiGameLoop(gameInstance)
        };
    }

    private static string ValidatePasswords(string gameName)
    {
        var passwords = _gameRepository.GetPasswords(gameName);
        do
        {
            Console.WriteLine("Give X password or exit (E): ");
            var inputX = Console.ReadLine()!;
            if (inputX == passwords[0])
            {
                do
                {
                    Console.WriteLine("Give O password or exit (E): ");
                    var inputO = Console.ReadLine()!;
                    if (inputO == passwords[1])
                    {
                        return "";
                    }

                    if (inputO.Equals(ExitShortcut, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return ExitShortcut;
                    }
                    Console.WriteLine("Wrong password");
                } while (true);
            }

            if (inputX.Equals(ExitShortcut, StringComparison.CurrentCultureIgnoreCase))
            {
                return ExitShortcut;
            }
            Console.WriteLine("Wrong password");
        } while (true);
    }
    
    private static string ValidatePasswordsAi(TicTacTwoBrain gameInstance, string gameName)
    {
        var passwords = _gameRepository.GetPasswords(gameName);
        do
        {
            if (gameInstance.GameType == EGameType.OnePlayer && passwords[0] != NoChoice)
            {
                Console.WriteLine("Give X password or exit (E): ");
            } else if (gameInstance.GameType == EGameType.OnePlayer && passwords[1] != NoChoice)
            {
                Console.WriteLine("Give O password or exit (E): ");
            }
            else
            {
                Console.WriteLine("Give observer password or exit (E): ");
            }
            var input = Console.ReadLine()!;

            if (gameInstance.GameType == EGameType.OnePlayer && passwords[1] != NoChoice)
            {
                if (input == passwords[1])
                {
                    return "";
                }
            }
            else
            {
                if (input == passwords[0])
                {
                    return "";
                }
            }

            if (input.Equals(ExitShortcut, StringComparison.CurrentCultureIgnoreCase))
            {
                return ExitShortcut;
            }
            Console.WriteLine("Wrong password");
        } while (true);
    }
}