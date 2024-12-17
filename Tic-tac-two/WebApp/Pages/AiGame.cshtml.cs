using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tic_tac_two2;

namespace WebApp.Pages;

public class AiGame : BaseGamePageModel
{
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

    [BindProperty] 
    public int SelectedX { get; set; } = -1;

    [BindProperty] 
    public int SelectedO { get; set; } = -1;

    [BindProperty(SupportsGet = true)] 
    public bool AImove { get; set; }
    
    [BindProperty(SupportsGet = true)] 
    public string? PasswordX { get; set; }
    
    [BindProperty(SupportsGet = true)] 
    public string? PasswordO { get; set; }

    public AiGame(IGameRepository gameRepository)
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
        
        var win = TicTacTwoBrain.WinningCondition();
        
        if (win != EGamePiece.Empty)
        {
            Winner = win;
            AImove = false;
        }
        
        if (TempData.ContainsKey("Error"))
        {
            Error = TempData["Error"] as string ?? "";
        }
        
        if (TempData.ContainsKey("AImove"))
        {
            AImove = (bool)(TempData["AImove"] ?? false);
        }

        if (AImove)
        {
            var pieceAi = Piece == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
            var moveAi = GameControllerAi.AiMove(TicTacTwoBrain, pieceAi);

            if (!moveAi)
            {
                Winner = EGamePiece.Empty;
            }
            else
            {
                GameName = _gameRepository.UpdateGame(TicTacTwoBrain.GetGameStateJson(), GameName);

                var winCon = TicTacTwoBrain.WinningCondition();
                if (winCon != EGamePiece.Empty)
                {
                    Winner = winCon;
                }
            }
            AImove = false;
        }
        
        Choices = GetChoices(TicTacTwoBrain, Piece);

        if (Choices.Count == 0)
        {
            Winner = EGamePiece.Empty;
        }
        return Page();
    }

    public IActionResult OnPost(string action)
    {
        var game = _gameRepository.LoadGame(GameName);
        
        if (game == null)
        {
            return RedirectToPage("/Index", new { Error = "Game not found!" });
        }
        
        TicTacTwoBrain = new TicTacTwoBrain(game);
        var parts = action.Split('-');

        switch (parts[0])
        {
            case "reset":
            {
                TicTacTwoBrain.ResetGame();
                GameName = _gameRepository.UpdateGame(TicTacTwoBrain.GetGameStateJson(), GameName);
                if (Piece != EGamePiece.O) return RedirectToPage(new { GameName, Piece, PasswordX, PasswordO });
                TempData["AImove"] = true;
                return RedirectToPage(new { GameName, Piece, PasswordX, PasswordO });

            }
            case "exit":
            {
                return Exit(_gameRepository, GameName, Winner);
            }
        }
        
        Choices = GetChoices(TicTacTwoBrain, Piece);

        var isMoveValid = false;

        if (TicTacTwoBrain.NextMoveBy != Piece)
        {
            TempData["Error"] = "Not your turn!";
            return RedirectToPage(new { GameName, Piece, PasswordX, PasswordO });
        }
        
        switch (parts[0])
        {
            case "move_grid" when !TicTacTwoBrain.EnoughMovesForMoreOptions():
                Error = "Invalid move!";
                return Page();
            case "move_grid":
            {
                isMoveValid = MoveGrid(TicTacTwoBrain, parts[1]);

                if (!isMoveValid)
                {
                    Error = "You can't move the grid in this direction!";
                    return Page();
                }

                break;
            }
            case "place_piece":
            {
                var x = int.Parse(parts[1]);
                var y = int.Parse(parts[2]);
                if (SelectedX != -1 && SelectedO != -1)
                {
                    if (!TicTacTwoBrain.EnoughMovesForMoreOptions())
                    {
                        Error = "Invalid move!";
                        SelectedX = -1;
                        SelectedO = -1;
                        return Page();
                    }

                    isMoveValid = TicTacTwoBrain.ChangePieceLocation(SelectedX, SelectedO, x, y);
                    if (!isMoveValid)
                    {
                        Error = "You can only move your own piece on the grid!";
                        SelectedX = -1;
                        SelectedO = -1;
                        return Page();
                    }

                    SelectedX = -1;
                    SelectedO = -1;
                }
                else
                {
                    isMoveValid = TicTacTwoBrain.MakeAMove(x, y);
                    if (!isMoveValid)
                    {
                        Error = "Invalid move!";
                        return Page();
                    }
                }

                break;
            }
            case "select_piece":
            {
                SelectedX = int.Parse(parts[1]);
                SelectedO = int.Parse(parts[2]);
                return Page();
            }
        }

        if (!isMoveValid) return Page();
        GameName = _gameRepository.UpdateGame(TicTacTwoBrain.GetGameStateJson(), GameName);

        var win = TicTacTwoBrain.WinningCondition();
        if (win != EGamePiece.Empty)
        {
            Winner = win;
            return RedirectToPage(new { GameName, Piece, PasswordX, PasswordO, Winner });
        }

        TempData["AImove"] = true;
        return RedirectToPage(new { GameName, Piece, PasswordX, PasswordO});
    }
}