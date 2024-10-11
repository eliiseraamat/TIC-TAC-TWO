using GameBrain;
using MenuSystem;

namespace Tic_tac_two2;

public static class Menus
{
    
/*var customizeMenu = new Menu(
    EMenuLevel.Deep,
    "TIC-TAC-TWO - customize", [
        new MenuItem()
        {
            ShortCut = "1",
            Title = "Choose the side size of the game board and grid, choose number of pieces (x, y, z)",
            MenuItemAction = DummyMethod
        }
    ]);*/

/*var newGameMenu = new Menu(
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
    ]);*/

    public static readonly Menu OptionsMenu = new Menu(
        EMenuLevel.Secondary,
        "TIC-TAC-TWO Options", [
            new MenuItem()
            {
                ShortCut = "C",
                Title = "Choose starting piece",
                MenuItemAction = DummyMethod
            },
            // new MenuItem()
            // {
            //     ShortCut = "O",
            //     Title = "O starts",
            //     MenuItemAction = DummyMethod
            // }
        ]);

    public static readonly Menu MainMenu = new Menu(
        EMenuLevel.Main, 
        "TIC-TAC-TWO", [
            new MenuItem()
            {
                ShortCut = "O",
                Title = "Options",
                MenuItemAction = OptionsMenu.Run
            },

            new MenuItem()
            {
                ShortCut = "N",
                Title = "New game",
                MenuItemAction = GameController.MainLoop
            }
        ]);
    
    // public static readonly Menu ChooseMenu = new Menu(
    //     EMenuLevel.Main, 
    //     "Choose one of the following", [
    //         new MenuItem()
    //         {
    //             ShortCut = "P",
    //             Title = "Put the piece",
    //             MenuItemAction = DummyMethod
    //         },
    //         new MenuItem()
    //         {
    //             ShortCut = "1",
    //             Title = "Move your piece",
    //             MenuItemAction = DummyMethod
    //         },
    //         new MenuItem()
    //         {
    //             ShortCut = "G",
    //             Title = "Move the grid",
    //             MenuItemAction = DummyMethod
    //         }
    //     ]);
    
    private static string DummyMethod()
    {
        Console.Write("Choose something");
        Console.ReadKey();
        return "foobar";
    }
}