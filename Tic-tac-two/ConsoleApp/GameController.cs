using DAL;
using GameBrain;
using MenuSystem;

namespace Tic_tac_two2;

public static class GameController
{
    private static readonly ConfigRepository ConfigRepository = new ConfigRepository();
    public static string MainLoop()
    {
        var chosenConfigShortcut = ChooseConfiguration();
        
        GameConfiguration chosenConfig;
        
        if (chosenConfigShortcut == "C")
        {
            chosenConfig = OptionsController.MainLoop();
        }
        else
        {
            if (!int.TryParse(chosenConfigShortcut, out var configNo))
            {
                ChooseConfiguration();
            }

            chosenConfig = ConfigRepository.GetConfigurationByName(ConfigRepository.GetConfigurationNames()[configNo]);
        }
        
        var gameInstance= new TicTacTwoBrain(chosenConfig);
        
        
        do
        {
            Console.Clear();
            
            Console.WriteLine("TIC-TAC-TWO");
            
            ConsoleUI.Visualizer.DrawBoard(gameInstance);
            
            EGamePiece player = gameInstance.NextMoveBy;
            
            var name = player == EGamePiece.X ? gameInstance.PlayerX : gameInstance.PlayerY;

            var userChoice = "";

            if (gameInstance.EnoughMovesForMoreOptions())
            {
                userChoice = GetUserChoice();
            }

            if (userChoice == "2")
            {
                Console.WriteLine($"Player {name}: give old coordinates and new coordinates for your piece <x,y>;<x,y>: ");
            }
            else
            {
                Console.WriteLine($"Player {name}: give coordinates <x,y>: ");
            }
            
            MakeMove(gameInstance, userChoice);

            if (gameInstance.WinningCondition())
            {
                Console.WriteLine($"Player {player} won!");
                return $"Player {player} won!";
            }

        } while (true);
    }

    public static GameConfiguration GetConfiguration()
    {
        var chosenConfigShortcut = ChooseConfiguration();
        
        if (!int.TryParse(chosenConfigShortcut, out var configNo))
        {
            ChooseConfiguration();
        }
    
        var chosenConfig = ConfigRepository.GetConfigurationByName(ConfigRepository.GetConfigurationNames()[configNo]);
        
        return chosenConfig;
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
        
        configMenuItems.Add(new MenuItem()
        {
            Title = "Customize",
            Shortcut = "C",
            MenuItemAction = () => "C"
        });
    
        var configMenu = new Menu(
            EMenuLevel.Secondary, 
            "TIC-TAC-TWO - choose game config", 
            configMenuItems,
            isCustomMenu: true);

       return configMenu.Run();
    }

    private static string GetUserChoice()
    {
        do
        {
            Console.WriteLine("1) Put new piece on grid");
            Console.WriteLine("2) Change one of your pieces to the grid");
            Console.WriteLine("3) Move grid one unit horizontally, vertically or diagonally");
            var input = Console.ReadLine()!;
            if (input == "1" || input == "2" || input == "3")
            {
                return input;
            }
            Console.WriteLine("Chose one of the following options: ");
        } while (true);
    }

    private static List<int> GetCoordinates()
    {
        do
        {
            var input = Console.ReadLine()!;
            if (input.Contains(";"))
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
                    return coordinates;
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid input");
                } 
            }
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
                return coordinates;
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid input");
            }

        } while (true);
    }

    private static void MakeMove(TicTacTwoBrain gameInstance, string userChoice)
    {
        var madeMove = false;
        do
        {
            var coordinates = GetCoordinates();
            if ((userChoice == "" || userChoice == "1") && gameInstance.MakeAMove(coordinates[0], coordinates[1]))
            {
                madeMove = true;
            } else if (userChoice == "2" &&
                      gameInstance.ChangePieceLocation(coordinates[0], coordinates[1], coordinates[2],
                          coordinates[3]))
            {
                madeMove = true;
            } else if (userChoice == "3" && gameInstance.MoveGrid(coordinates[0], coordinates[1]))
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