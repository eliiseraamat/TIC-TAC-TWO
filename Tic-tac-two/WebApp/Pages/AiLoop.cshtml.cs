using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tic_tac_two2;

namespace WebApp.Pages;

public class AiLoop : PageModel
{
    private readonly IGameRepository _gameRepository;
    
    [BindProperty(SupportsGet = true)] 
    public string GameName { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)] 
    public EGamePiece Piece { get; set; } = default!;
    
    [BindProperty]
    public List<string> Choices { get; set; } = [];
    
    [BindProperty(SupportsGet = true)] 
    public EGamePiece? Winner { get; set; }
    
    public TicTacTwoBrain TicTacTwoBrain { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)] 
    public string Password { get; set; } = default!;
    
    
    public AiLoop(IGameRepository gameRepository)
    {
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

        if (Choices.Count != 0) return;
        Winner = EGamePiece.Empty;
        RedirectToPage(new { GameName, Piece, Winner, Password });
    }

    public IActionResult OnPost(string action)
    {
        var game = _gameRepository.LoadGame(GameName);
        TicTacTwoBrain = new TicTacTwoBrain(game);
        
        switch (action)
        {
            case "reset":
                TicTacTwoBrain.ResetGame();
                GameName = _gameRepository.UpdateGame(TicTacTwoBrain.GetGameStateJson(), GameName);
                return RedirectToPage(new { GameName, Piece, Password });
            case "exit":
            {
                if (Winner != null)
                {
                    _gameRepository.DeleteGame(GameName);
                }
                return RedirectToPage("Index");
            }
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

            return RedirectToPage(new { GameName, Winner, Password });
        }

        return Page();
    }
}