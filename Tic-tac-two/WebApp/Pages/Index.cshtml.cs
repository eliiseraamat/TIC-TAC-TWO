using DAL;
using Domain;
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
                Error = "Choose piece";
                return Page();
            }
            if (ConfigName == "Customize")
            {
                return RedirectToPage("./Customize", new { ConfigName = ConfigName, GameType = GameType, Piece = Piece });
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
            string gameName;
            if (GameType == EGameType.TwoPlayer)
            {
                gameName = _gameRepository.SaveGame(gameInstance.GameState, ConfigName, EGamePiece.Empty);
            } else if (GameType == EGameType.OnePlayer && Piece == EGamePiece.O)
            {
                gameName = _gameRepository.SaveGame(gameInstance.GameState, ConfigName, EGamePiece.O);
            }
            else
            {
                gameName = _gameRepository.SaveGame(gameInstance.GameState, ConfigName, EGamePiece.X);
            }
            var passwords = _gameRepository.GetPasswords(gameName);
            if (GameType == EGameType.TwoPlayer)
            {
                return RedirectToPage("./PlayGame", new { GameName = gameName, Piece = Piece, PasswordX = passwords[0], PasswordO = passwords[1]}); 
            }
            if (GameType == EGameType.OnePlayer)
            {
                if (Piece == EGamePiece.O)
                {
                    return RedirectToPage("./AIGame", new { GameName = gameName, Piece = Piece, AImove = true, PasswordO = passwords[1] });
                }
                return RedirectToPage("./AIGame", new { GameName = gameName, Piece = Piece, PasswordX = passwords[0] });
            }
            return RedirectToPage("./AILoop", new { GameName = gameName, Password = passwords[0] });
        } 
        if (Piece == null)
        {
            Error = "Choose piece for the game";
            return Page();
        }
        
        var realPassword = _gameRepository.GetPasswords(GameName);
        if (string.IsNullOrWhiteSpace(Password))
        {
            Error = "Enter the password";
            return Page();
        }
        
        if (Piece == EGamePiece.X && Password != realPassword[0] || 
            Piece == EGamePiece.O && Password != realPassword[1] || Piece == EGamePiece.Empty && Password != realPassword[0])
        {
            Error = "Wrong password";
            return Page();
        }
        var game = _gameRepository.LoadGame(GameName);
        var instance = new TicTacTwoBrain(game);
        if (instance.GameType == EGameType.TwoPlayer)
        {
            return RedirectToPage("./PlayGame", new { GameName = GameName, Piece = Piece, PasswordX = realPassword[0], PasswordO = realPassword[1] });
        }
        if (instance.GameType == EGameType.OnePlayer)
        {
            if (Piece == EGamePiece.O)
            {
                return RedirectToPage("./AIGame", new { GameName = GameName, Piece = Piece, PasswordO = realPassword[1] });
            }
            return RedirectToPage("./AIGame", new { GameName = GameName, Piece = Piece, PasswordX = realPassword[0] });
        }
        return RedirectToPage("./AILoop", new { GameName = GameName, Password = realPassword[0] });
    }
}