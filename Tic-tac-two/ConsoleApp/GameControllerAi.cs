using DAL;
using GameBrain;
using MenuSystem;

namespace Tic_tac_two2;

public class GameControllerAi
{
    private static IConfigRepository _configRepository = default!;
    private static IGameRepository _gameRepository = default!;
    
    private const string MainShortcut = "M";
    private const string ReturnShortcut = "R";
    private const string NoChoice = "-";
    private const string PieceX = "X";
    private const string PieceO = "O";

    private static readonly MenuItem MenuNextMove = new MenuItem()
    {
        Title = "Next move",
        Shortcut = "N",
        MenuItemAction = () => 0.ToString()
    };
    
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

        chosenConfig.GameType = EGameType.OnePlayer;
        
        var gameInstance= new TicTacTwoBrain(chosenConfig);

        do
        {
            Console.WriteLine("Choose game piece X or O: ");

            var input = Console.ReadLine()!;

            if (input.Equals(PieceX, StringComparison.CurrentCultureIgnoreCase))
            {
                return GameLoop(gameInstance, EGamePiece.X);
            }
            if (input.Equals(PieceO, StringComparison.CurrentCultureIgnoreCase))
            {
                //AiMove(gameInstance, EGamePiece.X);
                return GameLoop(gameInstance, EGamePiece.O);
            }
            Console.WriteLine("Wrong input");

        } while (true);
        
    }
    
    private static string ChooseConfiguration()
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
            EMenuLevel.Secondary, 
            "TIC-TAC-TWO - choose game config", 
            configMenuItems,
            isCustomMenu: true);

        return configMenu.Run();
    }
    
    public static string GameLoop(TicTacTwoBrain gameInstance, EGamePiece piece)
    {
        string choice;
        do
        {
            Console.Clear();

            if (piece == EGamePiece.O)
            {
                AiMove(gameInstance, EGamePiece.X);
            }
            
            var winner = Game(gameInstance, piece);

            if (winner == null)
            {
                return MainShortcut;
            }

            choice = GameController.EndGame(winner, gameInstance);

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
    
    private static EGamePiece? Game(TicTacTwoBrain gameInstance, EGamePiece piece)
    {
        do
        {
            Console.Clear();
            
            Console.WriteLine("TIC-TAC-TWO");
            
            ConsoleUI.Visualizer.DrawBoard(gameInstance);

            var name = piece == EGamePiece.X ? gameInstance.PlayerX: gameInstance.PlayerO;
            
            var playerPieces = piece == EGamePiece.X ?gameInstance.PlayerXPieces: gameInstance.PlayerOPieces;

            var userChoice = GameController.Display(gameInstance, name, playerPieces);

            if (userChoice == NoChoice)
            {
                return EGamePiece.Empty;
            }
            
            var move = MakeMove(gameInstance, userChoice, playerPieces, piece);

            if (move == MenuQuit.Shortcut)
            {
                break;
            }

            var winnerPiece = gameInstance.WinningCondition();

            if (winnerPiece != EGamePiece.Empty)
            {
                return winnerPiece;
            }
            
            var pieceAi = piece == EGamePiece.X ? EGamePiece.O: EGamePiece.X;
            
            if (!AiMove(gameInstance, pieceAi))
            {
                return EGamePiece.Empty;
            }
            
            var winnerPiece2 = gameInstance.WinningCondition();

            if (winnerPiece2 != EGamePiece.Empty)
            {
                return winnerPiece2;
            }

        } while (true);

        return null;
    }

    public static bool AiMove(TicTacTwoBrain gameInstance, EGamePiece piece)
    {
        var count = piece == EGamePiece.X ? gameInstance.PlayerXPieces : gameInstance.PlayerOPieces;
        if (!(count > 0 && !gameInstance.IsGridFull()) &&
            !(gameInstance.EnoughMovesForMoreOptions() && !gameInstance.IsGridFull()) &&
            !(gameInstance.EnoughMovesForMoreOptions() && gameInstance.GridSize < gameInstance.DimX))
        {
            return false;
        }
        if (count > 0)
        {
            var newPiece = gameInstance.AiNewPiece(piece);
            if (newPiece.Count != 0)
            {
                gameInstance.MakeAMove(newPiece[0], newPiece[1]);
                return true;
            }
        }

        var outOfGrid = gameInstance.GetPieceOutOfGrid(piece);

        if (outOfGrid.Count > 0)
        {
            var movePiece = gameInstance.AiMovePiece(piece);
            if (movePiece.Count != 0)
            {
                gameInstance.ChangePieceLocation(movePiece[0], movePiece[1], movePiece[2], movePiece[3]);
                return true;
            }
        }
        var grid = gameInstance.AiMoveGrid();
        if (grid.Count != 0)
        {
            gameInstance.MoveGrid(grid[0], grid[1]);
            return true;
        }
        var movePiece2 = gameInstance.AiMovePiece(piece);
        if (movePiece2.Count == 0) return false;
        gameInstance.ChangePieceLocation(movePiece2[0], movePiece2[1], movePiece2[2], movePiece2[3]);
        return true;
    }
    
    public static string AiLoop(IConfigRepository configRepository, IGameRepository gameRepository)
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
        
        chosenConfig.GameType = EGameType.Ai;
        
        var gameInstance= new TicTacTwoBrain(chosenConfig);

        return AiGameLoop(gameInstance);
    }
    
    public static string AiGameLoop(TicTacTwoBrain gameInstance)
    {
        string choice;
        do
        {
            Console.Clear();
            
            var winner = AiGame(gameInstance);

            if (winner == null)
            {
                return MainShortcut;
            }

            choice = GameController.EndGame(winner, gameInstance);

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
    
    private static EGamePiece? AiGame(TicTacTwoBrain gameInstance)
    {
        do
        {
            Console.Clear();
            
            Console.WriteLine("TIC-TAC-TWO");
            
            ConsoleUI.Visualizer.DrawBoard(gameInstance);
            
            var player = gameInstance.NextMoveBy == EGamePiece.X ? gameInstance.PlayerX : gameInstance.PlayerO;
            
            Console.WriteLine($"{player}'s turn");
        
            Console.WriteLine();
        
            Console.WriteLine($"{gameInstance.PlayerX} has {gameInstance.PlayerXPieces} pieces left");
        
            Console.WriteLine($"{gameInstance.PlayerO} has {gameInstance.PlayerOPieces} pieces left");

            var result = GetUserChoice();

            if (result == MenuNextMove.Shortcut)
            {
                if (!AiMove(gameInstance, gameInstance.NextMoveBy))
                {
                    return EGamePiece.Empty;
                }
            } else if (result == MenuSave.Shortcut)
            {
                var name = _gameRepository.SaveGame(gameInstance.GameState, gameInstance.GetGameConfigName(), EGamePiece.X);
                var passwords = _gameRepository.GetPasswords(name);
                Console.WriteLine($"Saved game. Game name: {name}, password: {passwords[0]}");
                GetUserChoice();
            }
            else
            {
                return null;
            }
            
            var winnerPiece2 = gameInstance.WinningCondition();

            if (winnerPiece2 != EGamePiece.Empty)
            {
                return winnerPiece2;
            }

        } while (true);
    }
    
    private static string GetUserChoice()
    {
        do
        {
            Console.WriteLine("Choose one of the following options: ");
            
            var choices = new List<MenuItem>
            {
                MenuNextMove,
                MenuSave,
                MenuQuit
            };

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
    
    public static string MakeMove(TicTacTwoBrain gameInstance, string userChoice, int playerPieces, EGamePiece piece)
    {
        var madeMove = false;
        do
        {
            if (userChoice == MenuSave.Shortcut)
            {
                string name;
                if (piece == EGamePiece.O)
                {
                    name = _gameRepository.SaveGame(gameInstance.GameState, gameInstance.GetGameConfigName(), EGamePiece.O);
                    var passwords = _gameRepository.GetPasswords(name);
                    Console.WriteLine($"Saved game. Game name: {name}, O password: {passwords[1]}");
                }
                else
                {
                    name = _gameRepository.SaveGame(gameInstance.GameState, gameInstance.GetGameConfigName(), EGamePiece.X);
                    var passwords = _gameRepository.GetPasswords(name);
                    Console.WriteLine($"Saved game. Game name: {name}, X password: {passwords[0]}");
                }
                userChoice = GameController.DisplayChoices(gameInstance, playerPieces);
            }
            if (userChoice == MenuQuit.Shortcut)
            {
                return MenuQuit.Shortcut;
            }
            var coordinates = GameController.GetCoordinates(userChoice, gameInstance);
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
}