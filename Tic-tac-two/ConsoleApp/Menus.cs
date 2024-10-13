using MenuSystem;

namespace Tic_tac_two2;

public static class Menus
{
    public static readonly Menu OptionsMenu = new Menu(
        EMenuLevel.Secondary,
        "TIC-TAC-TWO Options", [
            new MenuItem()
            {
                Shortcut = "C",
                Title = "Choose starting piece",
                MenuItemAction = OptionsController.SetStartingPiece
            }
        ]);

    public static readonly Menu MainMenu = new Menu(
        EMenuLevel.Main, 
        "TIC-TAC-TWO", [
            new MenuItem()
            {
                Shortcut = "O",
                Title = "Options",
                MenuItemAction = OptionsMenu.Run
            },

            new MenuItem()
            {
                Shortcut = "N",
                Title = "New game",
                MenuItemAction = GameController.MainLoop
            }
        ]);
}