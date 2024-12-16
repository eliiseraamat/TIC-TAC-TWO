using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly IConfigRepository _configRepository;
    private readonly IGameRepository _gameRepository;


    public IndexModel(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
    }

    public SelectList ConfigSelectList { get; set; } = default!;
    
    [BindProperty]
    public string ConfigName { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)]
    public string? Error { get; set; }
    
    [BindProperty]
    public string GameName { get; set; } = default!;

    public SelectList GameSelectList { get; set; } = default!;

    [BindProperty] public EGamePiece? Piece { get; set; }

    [BindProperty] 
    public string Password { get; set; } = default!;
    
    [BindProperty] 
    public EGameType? GameType { get; set; }
    

    public void OnGet()
    {
        var selectListData = _configRepository.GetConfigurationNames()
            .Select(name => new {id = name, value = name})
            .ToList();
        ConfigSelectList = new SelectList(selectListData, "id", "value");
        
        var selectListGames = _gameRepository.GetGameNames()
            .Select(name => new {id = name, value = name})
            .ToList();
        GameSelectList = new SelectList(selectListGames, "id", "value");
    }
    
    public IActionResult OnPost(string action)
    {
        var selectListData = _configRepository.GetConfigurationNames()
            .Select(name => new { id = name, value = name })
            .ToList();
        ConfigSelectList = new SelectList(selectListData, "id", "value");
    
        var selectListGames = _gameRepository.GetGameNames()
            .Select(name => new { id = name, value = name })
            .ToList();
        GameSelectList = new SelectList(selectListGames, "id", "value");
        
        if (action == "new")
        {
            if (GameType == null)
            {
                Error = "Choose gameplay type";
                return Page();
            }

            if (GameType != EGameType.AI && Piece == null)
            {
                Error = "Choose piece!";
                return Page();
            }
            if (ConfigName == "Customize")
            {
                return RedirectToPage("./Customize", new { ConfigName, GameType, Piece });
            }
            var chosenConfig = _configRepository.GetConfigurationByName(ConfigName);
            if (GameType == EGameType.OnePlayer)
            {
                chosenConfig.GameType = EGameType.OnePlayer;
            } else if (GameType == EGameType.AI)
            {
                chosenConfig.GameType = EGameType.AI;
            }
            var gameInstance = new TicTacTwoBrain(chosenConfig);
            var gameName = GameType switch
            {
                EGameType.TwoPlayer => _gameRepository.SaveGame(gameInstance.GameState, ConfigName, EGamePiece.Empty),
                EGameType.OnePlayer when Piece == EGamePiece.O => _gameRepository.SaveGame(gameInstance.GameState,
                    ConfigName, EGamePiece.O),
                _ => _gameRepository.SaveGame(gameInstance.GameState, ConfigName, EGamePiece.X)
            };
            var passwords = _gameRepository.GetPasswords(gameName);
            return GameType switch
            {
                EGameType.TwoPlayer => RedirectToPage("./PlayGame",
                    new { GameName = gameName, Piece, PasswordX = passwords[0], PasswordO = passwords[1] }),
                EGameType.OnePlayer when Piece == EGamePiece.O => RedirectToPage("./AIGame",
                    new { GameName = gameName, Piece, AImove = true, PasswordO = passwords[1] }),
                EGameType.OnePlayer => RedirectToPage("./AIGame",
                    new { GameName = gameName, Piece, PasswordX = passwords[0] }),
                _ => RedirectToPage("./AILoop", new { GameName = gameName, Password = passwords[0] })
            };
        } 
        if (Piece == null)
        {
            Error = "Choose piece for the game!";
            return Page();
        }
        
        var realPassword = _gameRepository.GetPasswords(GameName);
        var game = _gameRepository.LoadGame(GameName);
        var instance = new TicTacTwoBrain(game);
        
        if (string.IsNullOrWhiteSpace(Password))
        {
            Error = "Enter the password!";
            return Page();
        }

        if ((instance.GameType == EGameType.OnePlayer || instance.GameType == EGameType.TwoPlayer) &&
            Piece == EGamePiece.Empty || Piece != EGamePiece.Empty && GameType == EGameType.AI)
        {
            Error = "You can be observer only in AI vs AI game!";
            return Page();
        }
        
        if (instance.GameType == EGameType.TwoPlayer)
        {
            if ((Piece != EGamePiece.X || Password == realPassword[0]) &&
                (Piece != EGamePiece.O || Password == realPassword[1]))
                return RedirectToPage("./PlayGame",
                    new
                    {
                        GameName, Piece, PasswordX = realPassword[0], PasswordO = realPassword[1]
                    });
            Error = "Wrong password!";
            return Page();
        }
        if (instance.GameType == EGameType.OnePlayer)
        {
            if (Piece == EGamePiece.O)
            {
                if (Password == realPassword[1])
                    return RedirectToPage("./AIGame",
                        new { GameName, Piece, PasswordO = realPassword[1] });
                Error = "Wrong password!";
                return Page();
            }

            if (Password == realPassword[0])
                return RedirectToPage("./AIGame",
                    new { GameName, Piece, PasswordX = realPassword[0] });
            Error = "Wrong password!";
            return Page();
        }

        if (Password == realPassword[0])
            return RedirectToPage("./AILoop", new { GameName, Password = realPassword[0] });
        Error = "Wrong password!";
        return Page();
    }
}