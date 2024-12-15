using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tic_tac_two2;

namespace WebApp.Pages;

public class AILoop : PageModel
{
    private readonly IConfigRepository _configRepository;
    private readonly IGameRepository _gameRepository;
    
    [BindProperty(SupportsGet = true)] 
    public string GameName { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)] 
    public EGamePiece Piece { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)] 
    public string Error { get; set; } = "";
    
    [BindProperty]
    public List<string> Choices { get; set; } = [];
    
    [BindProperty(SupportsGet = true)] 
    public EGamePiece? Winner { get; set; }
    
    public TicTacTwoBrain TicTacTwoBrain { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)] 
    public string Password { get; set; } = default!;
    
    
    public AILoop(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
    }
    
    public void OnGet()
    {
        var game = _gameRepository.LoadGame(GameName);

        TicTacTwoBrain = new TicTacTwoBrain(game);
        
        var amount = TicTacTwoBrain.NextMoveBy == EGamePiece.X ? TicTacTwoBrain.PlayerXPieces : TicTacTwoBrain.PlayerOPieces;
        if (amount > 0 && !TicTacTwoBrain.IsGridFull())
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

        if (Choices.Count == 0)
        {
            Winner = EGamePiece.Empty;
            RedirectToPage(new { GameName = GameName, Piece = Piece, Winner = Winner, Password = Password });
        }
    }

    public IActionResult OnPost(string action)
    {
        var game = _gameRepository.LoadGame(GameName);
        TicTacTwoBrain = new TicTacTwoBrain(game);
        
        if (action == "reset")
        {
            TicTacTwoBrain.ResetGame();
            GameName = _gameRepository.UpdateGame(TicTacTwoBrain.GetGameStateJson(), GameName);
            return RedirectToPage(new { GameName = GameName, Piece = Piece, Password = Password });
        }

        if (action == "exit")
        {
            if (Winner != null)
            {
                _gameRepository.DeleteGame(GameName);
            }
            return RedirectToPage("Index");
        }
        
        var amount = TicTacTwoBrain.NextMoveBy == EGamePiece.X ? TicTacTwoBrain.PlayerXPieces : TicTacTwoBrain.PlayerOPieces;
        if (amount > 0 && !TicTacTwoBrain.IsGridFull())
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
        
        if (Choices.Count == 0)
        {
            Winner = EGamePiece.Empty;
            RedirectToPage(new { GameName = GameName, Piece = Piece, Winner = Winner, Password = Password });
        }
        
        var move = GameControllerAi.AIMove(TicTacTwoBrain, TicTacTwoBrain.NextMoveBy);
        if (!move)
        {
            Winner = EGamePiece.Empty;
        }
        else
        {
            GameName = _gameRepository.UpdateGame(TicTacTwoBrain.GetGameStateJson(), GameName);
            var win = TicTacTwoBrain.WinningCondition();
            if (win != EGamePiece.Empty)
            {
                Winner = win;
            }

            return RedirectToPage(new { GameName = GameName, Winner = Winner, Password = Password });
        }

        return Page();
    }
}