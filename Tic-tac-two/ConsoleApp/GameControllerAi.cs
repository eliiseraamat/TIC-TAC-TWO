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
        
        var gameInstance= new TicTacTwoBrain(chosenConfig);

        return GameLoop(gameInstance);
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
    
    private static string GameLoop(TicTacTwoBrain gameInstance)
    {
        string choice;
        do
        {
            Console.Clear();
            
            var winner = Game(gameInstance);

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
    
    private static EGamePiece? Game(TicTacTwoBrain gameInstance)
    {
        do
        {
            Console.Clear();
            
            Console.WriteLine("TIC-TAC-TWO");
            
            ConsoleUI.Visualizer.DrawBoard(gameInstance);

            var name = gameInstance.PlayerX;
            
            var playerPieces = gameInstance.PlayerXPieces;

            var userChoice = GameController.Display(gameInstance, name, playerPieces);

            if (userChoice == "-")
            {
                return EGamePiece.Empty;
            }
            
            var move = GameController.MakeMove(gameInstance, userChoice, playerPieces);

            if (move == "q")
            {
                break;
            }

            var winnerPiece = gameInstance.WinningCondition();

            if (winnerPiece != EGamePiece.Empty)
            {
                return winnerPiece;
            }
            
            AIMove(gameInstance);
            
            var winnerPiece2 = gameInstance.WinningCondition();

            if (winnerPiece2 != EGamePiece.Empty)
            {
                return winnerPiece2;
            }

        } while (true);

        return null;
    }
    
    private static void AIMove(TicTacTwoBrain gameInstance)
    {
        if (gameInstance.PlayerOPieces > 0)
        {
            var newPiece = gameInstance.AINewPiece(EGamePiece.O);
            if (newPiece.Count != 0)
            {
                gameInstance.MakeAMove(newPiece[0], newPiece[1]);
            }

            var grid = gameInstance.AIMoveGrid();
            if (grid.Count != 0)
            {
                gameInstance.MoveGrid(grid[0], grid[1]);
            }
        }
        var movePiece = gameInstance.AIMovePiece(EGamePiece.O);
        if (movePiece.Count != 0)
        {
            gameInstance.ChangePieceLocation(movePiece[0], movePiece[1], movePiece[2], movePiece[3]);
        }
    }
}