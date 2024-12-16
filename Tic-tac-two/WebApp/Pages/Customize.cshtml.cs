using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class Customize : PageModel
{
    private readonly IGameRepository _gameRepository;
    
    public Customize(IGameRepository gameRepository)
    {
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
    public string X { get; set; } = default!;
    
    [BindProperty] 
    public string O { get; set; } = default!;
    
    [BindProperty]
    public string Error  { get; set; } = default!;

    public void OnGet()
    {
        
    }

    public IActionResult OnPost()
    {
        if (string.IsNullOrWhiteSpace(X) || string.IsNullOrWhiteSpace(O))
        {
            Error = "Fill all fields!";
            return Page();
        }

        if (BoardSize < 3 || BoardSize > 20)
        {
            Error = "Game board size must be between 3 and 20!";
            return Page();
        }
        if (GridSize < 3 || GridSize > BoardSize)
        {
            Error = "Grid size must be between 3 and board size!";
            return Page();
        }
        if (GridX < 0 || GridX + GridSize > BoardSize || GridY < 0 || GridY + GridSize > BoardSize)
        {
            Error = "Select the grid coordinates so that the grid fits on the board!";
            return Page();
        }
        if (PieceNr < WinCondition)
        {
            Error = "Piece number must be greater than winning condition!";
            return Page();
        }
        if (WinCondition < 2 || WinCondition > GridSize)
        {
            Error = "Winning condition must fit in the grid!";
            return Page();
        }

        if (X == O)
        {
            Error = "Player's names can't be the same!";
            return Page();
        }
        var config = new GameConfiguration()
        {
            BoardSize = BoardSize,
            GridSize = GridSize,
            Pieces = PieceNr,
            WinCondition = WinCondition,
            MovePieceAfterMoves = MovePieceAfterMoves,
            PlayerX = X,
            PlayerO = O,
            GridCoordinates = [GridX,GridY],
            GameType = GameType
        };
        var brain = new TicTacTwoBrain(config);
        var gameName = GameType switch
        {
            EGameType.TwoPlayer => _gameRepository.SaveGame(brain.GameState, ConfigName, EGamePiece.Empty),
            EGameType.OnePlayer when Piece == EGamePiece.O => _gameRepository.SaveGame(brain.GameState, ConfigName,
                EGamePiece.O),
            _ => _gameRepository.SaveGame(brain.GameState, ConfigName, EGamePiece.X)
        };
        var passwords = _gameRepository.GetPasswords(gameName);
        return GameType switch
        {
            EGameType.OnePlayer => Piece == EGamePiece.O
                ? RedirectToPage("/AIGame", new { GameName = gameName, Piece, AImove = true, passwordO = passwords[1] })
                : RedirectToPage("/AIGame", new { GameName = gameName, Piece, PasswordX = passwords[0] }),
            EGameType.AI => RedirectToPage("/AILoop", new { GameName = gameName, Password = passwords[0] }),
            _ => RedirectToPage("/PlayGame",
                new { GameName = gameName, Piece, PasswordX = passwords[0], passwordO = passwords[1] })
        };
    }
}