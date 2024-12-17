using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class PlayGame : BaseGamePageModel
{
    private readonly IGameRepository _gameRepository;


    public PlayGame(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }
    
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
    public int SelectedX { get; set; } = -1;

    [BindProperty(SupportsGet = true)] 
    public int SelectedO { get; set; } = -1;

    [BindProperty(SupportsGet = true)] 
    public string PasswordX { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)] 
    public string PasswordO { get; set; } = default!;
    
    public IActionResult OnGet()
    {
        var game = _gameRepository.LoadGame(GameName);

        if (game == null)
        {
            return RedirectToPage("/Index", new { Error = "Game not found!" });
        }

        TicTacTwoBrain = new TicTacTwoBrain(game);
        
        if (TempData.ContainsKey("Error"))
        {
            Error = TempData["Error"] as string ?? "";
        }
        
        var win = TicTacTwoBrain.WinningCondition();
        if (win != EGamePiece.Empty)
        {
            Winner = win;
        }
        
        else
        {
            Choices = GetChoices(TicTacTwoBrain, Piece);

            if (Choices.Count == 0)
            {
                Winner = EGamePiece.Empty;
            }
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
        if (parts[0] == "reset")
        {
            TicTacTwoBrain.ResetGame();
            GameName = _gameRepository.UpdateGame(TicTacTwoBrain.GetGameStateJson(), GameName);
            return RedirectToPage(new { GameName, Piece, PasswordX, PasswordO });
        }

        if (parts[0] == "exit")
        {
            return Exit(_gameRepository, GameName, Winner);
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

        Winner = GetWinner(TicTacTwoBrain);

        return RedirectToPage(new { GameName, Piece, Winner, PasswordX, PasswordO});

    }
}