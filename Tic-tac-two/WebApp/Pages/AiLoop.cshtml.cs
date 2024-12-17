using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tic_tac_two2;

namespace WebApp.Pages;

public class AiLoop : BaseGamePageModel
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
    
    public IActionResult OnGet()
    {
        var game = _gameRepository.LoadGame(GameName);
        
        if (game == null)
        {
            return RedirectToPage("/Index", new { Error = "Game not found!" });
        }

        TicTacTwoBrain = new TicTacTwoBrain(game);
        
        Choices = GetChoices(TicTacTwoBrain, Piece);

        if (Choices.Count != 0) return Page();
        Winner = EGamePiece.Empty;
        return RedirectToPage(new { GameName, Piece, Winner, Password });
    }

    public IActionResult OnPost(string action)
    {
        var game = _gameRepository.LoadGame(GameName);
        
        if (game == null)
        {
            return RedirectToPage("/Index", new { Error = "Game not found!" });
        }
        
        TicTacTwoBrain = new TicTacTwoBrain(game);
        
        switch (action)
        {
            case "reset":
                TicTacTwoBrain.ResetGame();
                GameName = _gameRepository.UpdateGame(TicTacTwoBrain.GetGameStateJson(), GameName);
                return RedirectToPage(new { GameName, Piece, Password });
            case "exit":
            {
                return Exit(_gameRepository, GameName, Winner);
            }
        }
        
        var move = GameControllerAi.AiMove(TicTacTwoBrain, TicTacTwoBrain.NextMoveBy);
        if (!move)
        {
            Winner = EGamePiece.Empty;
        }
        else
        {
            GameName = _gameRepository.UpdateGame(TicTacTwoBrain.GetGameStateJson(), GameName);
            Winner = GetWinner(TicTacTwoBrain);

            return RedirectToPage(new { GameName, Winner, Password });
        }

        return Page();
    }
}