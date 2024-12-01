using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class PlayGame : PageModel
{
    private readonly IConfigRepository _configRepository;
    private readonly IGameRepository _gameRepository;
    private readonly ILogger<IndexModel> _logger;


    public PlayGame(IConfigRepository configRepository, IGameRepository gameRepository, ILogger<IndexModel> logger)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
        _logger = logger;
    }
    
    [BindProperty(SupportsGet = true)] 
    public string GameName { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)] 
    public EGamePiece Piece { get; set; } = default!;
    
    [BindProperty] 
    public string Error { get; set; } = "";
    
    [BindProperty]
    public List<String> Choices { get; set; } = new List<string>();
    
    [BindProperty] 
    public EGamePiece Winner { get; set; }

    [BindProperty(SupportsGet = true)] 
    public EGamePiece NextMoveBy { get; set; } = default!;
    
    public TicTacTwoBrain TicTacTwoBrain { get; set; } = default!;
    
    [BindProperty]
    public int? SelectedPieceX { get; set; }
    [BindProperty]
    public int? SelectedPieceY { get; set; }
    
    public void OnGet(int? x, int? y)
    {
        var game = _gameRepository.LoadGame(GameName);

        TicTacTwoBrain = new TicTacTwoBrain(game);
        
        if (Piece > 0 && !TicTacTwoBrain.IsGridFull())
        {
            Choices.Add("Put a new piece on the grid");
        } 

        if (TicTacTwoBrain.EnoughMovesForMoreOptions())
        {
            if (!TicTacTwoBrain.IsGridFull())
            {
                Choices.Add("Move one of your pieces to another spot in the grid");
            }

            if (TicTacTwoBrain.GridSize < TicTacTwoBrain.DimX)
            {
                Choices.Add("Move grid one spot horizontally, vertically or diagonally");
            }
        }

        if (TicTacTwoBrain.NextMoveBy != Piece)
        {
            Error = "Not your turn";
        } 
        else if (x != null && y != null && TicTacTwoBrain.NextMoveBy == Piece) 
        {
            var move = TicTacTwoBrain.MakeAMove(x.Value, y.Value);
            if (!move)
            {
                Error = "Wrong move";
            }
            else
            {
                GameName = _gameRepository.UpdateGame(TicTacTwoBrain.GetGameStateJson(), GameName);
                var win = TicTacTwoBrain.WinningCondition();
                if (win != EGamePiece.Empty)
                {
                    Winner = win;
                }
            }
        }
    }
}