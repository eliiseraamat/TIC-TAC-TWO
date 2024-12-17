using DAL;
using MenuSystem;

namespace Tic_tac_two2;

public static class Menus
{
    private static IConfigRepository _configRepository = default!;
    private static IGameRepository _gameRepository = default!;

    public static void Init(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
    }
    
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
    
    public static readonly Menu GameMenu = new Menu(
        EMenuLevel.Secondary,
        "TIC-TAC-TWO Game type", [
            new MenuItem()
            {
                Shortcut = "T",
                Title = "Two players gameplay",
                MenuItemAction = () => GameController.MainLoop(_configRepository, _gameRepository)
            },
            new MenuItem()
            {
                Shortcut = "P",
                Title = "Player vs AI gameplay",
                MenuItemAction = () => GameControllerAi.MainLoop(_configRepository, _gameRepository)
            },
            new MenuItem()
            {
                Shortcut = "A",
                Title = "AI vs AI gameplay",
                MenuItemAction = () => GameControllerAi.AiLoop(_configRepository, _gameRepository)
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
                MenuItemAction = GameMenu.Run
            },
            new MenuItem()
            {
                Shortcut = "L",
                Title = "Load game",
                MenuItemAction = () => GameController.LoadGame(_configRepository, _gameRepository)
            }
        ]);
}