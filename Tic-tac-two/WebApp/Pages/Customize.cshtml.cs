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
    
    [BindProperty]
    public int BoardSize { get; set; }
    
    [BindProperty]
    public int GridSize { get; set; }
    
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

    public void OnGet()
    {
        
    }

    public IActionResult OnPost()
    {
        var config = new GameConfiguration()
        {
            BoardSize = BoardSize,
            GridSize = GridSize,
            Pieces = PieceNr,
            WinCondition = WinCondition,
            MovePieceAfterMoves = MovePieceAfterMoves,
            PlayerX = x,
            PlayerO = o
        };
        var brain = new TicTacTwoBrain(config);
        var gameName = _gameRepository.SaveGame(brain.GetGameStateJson(), ConfigName);
        return RedirectToPage("/PlayGame", new { GameName = gameName, Piece = EGamePiece.X });
    }
}