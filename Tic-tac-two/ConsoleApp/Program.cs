using GameBrain;
using MenuSystem;

var gameInstance= new TicTacTwoBrain();

var customizeMenu = new Menu(
    EMenuLevel.Deep,
    "TIC-TAC-TWO - customize", [
        new MenuItem()
        {
            ShortCut = "1",
            Title = "Choose the side size of the game board and grid, choose number of pieces (x, y, z)",
            MenuItemAction = DummyMethod
        }
    ]);

var newGameMenu = new Menu(
    EMenuLevel.Secondary,
    "TIC-TAC-TWO - choose game config", [
        new MenuItem()
        {
            ShortCut = "1",
            Title = "Classical",
            MenuItemAction = NewGame
        },
        new MenuItem()
        {
            ShortCut = "2",
            Title = "Customize",
            MenuItemAction = customizeMenu.Run
        }
    ]);

var optionsMenu = new Menu(
    EMenuLevel.Secondary,
    "TIC-TAC-TWO Options", [
    new MenuItem()
    {
        ShortCut = "X",
        Title = "X starts",
        MenuItemAction = DummyMethod
    },
    new MenuItem()
    {
        ShortCut = "O",
        Title = "O starts",
        MenuItemAction = DummyMethod
    }
]);

var mainMenu = new Menu(
    EMenuLevel.Main, 
    "TIC-TAC-TWO", [
    new MenuItem()
    {
        ShortCut = "O",
        Title = "Options",
        MenuItemAction = optionsMenu.Run
    },

    new MenuItem()
    {
        ShortCut = "N",
        Title = "New game",
        MenuItemAction = newGameMenu.Run
    }
]);

mainMenu.Run();

string DummyMethod()
{
    Console.Write("Choose something");
    Console.ReadKey();
    return "foobar";
}

string NewGame()
{
    ConsoleUI.Visualizer.DrawBoard(gameInstance);
    
    Console.WriteLine("Give me coordinates <x, y>: ");
    
    var input = Console.ReadLine()!;
    var inoutSplit = input.Split(",");
    var inputX = int.Parse(inoutSplit[0]);
    var inputY = int.Parse(inoutSplit[1]);

    gameInstance.MakeAMove(inputX, inputY);

    return "===";
}

