using DAL;
using GameBrain;
using MenuSystem;

namespace Tic_tac_two2;

public class GameControllerAi
{
    private static IConfigRepository _configRepository = default!;
    private static IGameRepository _gameRepository = default!;
    
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

            if (input.ToUpper() == "X")
            {
                return GameLoop(gameInstance, EGamePiece.X);
            }
            if (input.ToUpper() == "O")
            {
                AIMove(gameInstance, EGamePiece.X);
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
            
            var winner = Game(gameInstance, piece);

            if (winner == null)
            {
                return "r";
            }

            choice = GameController.EndGame(winner, gameInstance);

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

            if (userChoice == "-")
            {
                return EGamePiece.Empty;
            }
            
            var move = MakeMove(gameInstance, userChoice, playerPieces, piece);

            if (move == "q")
            {
                break;
            }

            var winnerPiece = gameInstance.WinningCondition();

            if (winnerPiece != EGamePiece.Empty)
            {
                return winnerPiece;
            }
            
            var pieceAI = piece == EGamePiece.X ? EGamePiece.O: EGamePiece.X;
            
            if (!AIMove(gameInstance, pieceAI))
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

    public static bool AIMove(TicTacTwoBrain gameInstance, EGamePiece piece)
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
            var newPiece = gameInstance.AINewPiece(piece);
            if (newPiece.Count != 0)
            {
                gameInstance.MakeAMove(newPiece[0], newPiece[1]);
                return true;
            }
        } 
        Random r = new Random();
        var random = r.Next(0, 2);
        if (random == 0)
        {
            var grid = gameInstance.AIMoveGrid();
            if (grid.Count != 0)
            {
                gameInstance.MoveGrid(grid[0], grid[1]);
                return true;
            }
            var movePiece = gameInstance.AIMovePiece(piece);
            if (movePiece.Count != 0)
            {
                gameInstance.ChangePieceLocation(movePiece[0], movePiece[1], movePiece[2], movePiece[3]);
                return true;
            }
            return false;
        } 
        var movePiece2 = gameInstance.AIMovePiece(piece);
        if (movePiece2.Count != 0)
        {
            gameInstance.ChangePieceLocation(movePiece2[0], movePiece2[1], movePiece2[2], movePiece2[3]);
            return true;
        }
        var grid2 = gameInstance.AIMoveGrid();
        if (grid2.Count == 0) return false;
        gameInstance.MoveGrid(grid2[0], grid2[1]);
        return true;
    }
    
    public static string AILoop(IConfigRepository configRepository, IGameRepository gameRepository)
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
        
        chosenConfig.GameType = EGameType.AI;
        
        var gameInstance= new TicTacTwoBrain(chosenConfig);

        return AIGameLoop(gameInstance);
    }
    
    public static string AIGameLoop(TicTacTwoBrain gameInstance)
    {
        string choice;
        do
        {
            Console.Clear();
            
            var winner = AIGame(gameInstance);

            if (winner == null)
            {
                return "r";
            }

            choice = GameController.EndGame(winner, gameInstance);

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
    
    private static EGamePiece? AIGame(TicTacTwoBrain gameInstance)
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

            var result = GetUserChoice(gameInstance);

            if (result == "N")
            {
                if (!AIMove(gameInstance, gameInstance.NextMoveBy))
                {
                    return EGamePiece.Empty;
                }
            } else if (result == "S")
            {
                var name = _gameRepository.SaveGame(gameInstance.GameState, gameInstance.GetGameConfigName(), EGamePiece.X);
                var passwords = _gameRepository.GetPasswords(name);
                Console.WriteLine($"Saved game. Game name: {name}, password: {passwords[0]}");
                result = GetUserChoice(gameInstance);
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
            
            //Thread.Sleep(2000);

        } while (true);
    }
    
    private static string GetUserChoice(TicTacTwoBrain gameInstance)
    {
        do
        {
            Console.WriteLine("Choose one of the following options: ");
            
            var choices = new List<MenuItem>();
            
            choices.Add(new MenuItem()
            {
                Title = "Next move",
                Shortcut = "N",
                MenuItemAction = () => 0.ToString()
            });
            
            choices.Add(new MenuItem()
            {
                Title = "Save game",
                Shortcut = "S",
                MenuItemAction = () => 0.ToString()
            });
            
            choices.Add(new MenuItem()
            {
                Title = "Quit game",
                Shortcut = "Q",
                MenuItemAction = () => 0.ToString()
            });
            
            if (choices.Count == 2)
            {
                return "-";
            }
            
            foreach (MenuItem item in choices)
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
            if (userChoice == "S")
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
            if (userChoice == "Q")
            {
                return "q";
            }
            var coordinates = GameController.GetCoordinates(userChoice, gameInstance);
            if (userChoice == "N" && gameInstance.MakeAMove(coordinates[0], coordinates[1]))
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

        return "";
    }
}