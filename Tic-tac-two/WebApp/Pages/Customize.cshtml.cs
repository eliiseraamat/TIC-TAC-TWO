using System.ComponentModel.DataAnnotations;
using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class Customize : PageModel
{
    private readonly IConfigRepository _configRepository;
    private readonly IGameRepository _gameRepository;


    public Customize(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
    }
    
    [BindProperty(SupportsGet = true)] 
    public string ConfigName { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)] 
    public EGameType GameType { get; set; }
    
    [BindProperty(SupportsGet = true)] 
    public EGamePiece Piece { get; set; }
    
    [BindProperty]
    public int BoardSize { get; set; }
    
    [BindProperty]
    public int GridSize { get; set; }
    
    [BindProperty] 
    public int GridX { get; set; }
    [BindProperty] 
    public int GridY { get; set; }
    
    [BindProperty]
    public int PieceNr { get; set; }

    [BindProperty] 
    public int WinCondition { get; set; }
    
    [BindProperty] 
    public int MovePieceAfterMoves { get; set; }

    [BindProperty] 
    public string x { get; set; } = default!;
    
    [BindProperty] 
    public string o { get; set; } = default!;
    
    [BindProperty]
    public string Error  { get; set; } = default!;

    public void OnGet()
    {
        
    }

    public IActionResult OnPost()
    {
        if (string.IsNullOrWhiteSpace(x) || string.IsNullOrWhiteSpace(o))
        {
            Error = "Fill all fields";
            return Page();
        }

        if (BoardSize < 3 || BoardSize > 20)
        {
            Error = "Game board size must be between 3 and 20";
            return Page();
        }
        if (GridSize < 3 || GridSize > BoardSize)
        {
            Error = "Grid size must be between 3 and BoardSize";
            return Page();
        }
        if (GridX < 0 || GridX + GridSize > BoardSize || GridY < 0 || GridY + GridSize > BoardSize)
        {
            Error = "Grid must fit in the board";
            return Page();
        }
        if (PieceNr < WinCondition)
        {
            Error = "Piece number must be greater than winning condition";
            return Page();
        }
        if (WinCondition < 2 || WinCondition > GridSize)
        {
            Error = "Winning condition must fit in the grid";
            return Page();
        }

        if (x == o)
        {
            Error = "Player's names can't be the same";
            return Page();
        }
        var config = new GameConfiguration()
        {
            BoardSize = BoardSize,
            GridSize = GridSize,
            Pieces = PieceNr,
            WinCondition = WinCondition,
            MovePieceAfterMoves = MovePieceAfterMoves,
            PlayerX = x,
            PlayerO = o,
            GridCoordinates = [GridX,GridY],
            GameType = GameType
        };
        var brain = new TicTacTwoBrain(config);
        string gameName;
        if (GameType == EGameType.TwoPlayer)
        {
            gameName = _gameRepository.SaveGame(brain.GameState, ConfigName, EGamePiece.Empty);
        } else if (GameType == EGameType.OnePlayer && Piece == EGamePiece.O)
        {
            gameName = _gameRepository.SaveGame(brain.GameState, ConfigName, EGamePiece.O);
        }
        else
        {
            gameName = _gameRepository.SaveGame(brain.GameState, ConfigName, EGamePiece.X);
        }
        var passwords = _gameRepository.GetPasswords(gameName);
        if (GameType == EGameType.OnePlayer)
        {
            if (Piece == EGamePiece.O)
            {
                return RedirectToPage("/AIGame", new { GameName = gameName, Piece =  Piece, AImove = true, passwordO = passwords[1] });
            }
            return RedirectToPage("/AIGame", new { GameName = gameName, Piece =  Piece, PasswordX = passwords[0] });
        }

        if (GameType == EGameType.AI)
        {
            return RedirectToPage("/AILoop", new { GameName = gameName, Password = passwords[0] });
        }
        
        return RedirectToPage("/PlayGame", new { GameName = gameName, Piece = Piece, PasswordX = passwords[0], passwordO = passwords[1] });
    }
}