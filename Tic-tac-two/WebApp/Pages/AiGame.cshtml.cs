using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tic_tac_two2;

namespace WebApp.Pages;

public class AiGame : PageModel
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
            var moveAi = GameControllerAi.AIMove(TicTacTwoBrain, pieceAi);
            Console.WriteLine(moveAi);
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
        
        var amount = Piece == EGamePiece.X ? TicTacTwoBrain.PlayerXPieces : TicTacTwoBrain.PlayerOPieces;
        if (amount > 0 && !TicTacTwoBrain.IsGridFull())
        {
            Choices.Add("Put a new piece on the grid");
        } 

        if (TicTacTwoBrain.EnoughMovesForMoreOptions())
        {
            if (!TicTacTwoBrain.IsGridFull() && TicTacTwoBrain.CheckPieceInBoard(Piece))
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
        }
        return Page();
    }

    public IActionResult OnPost(string action)
    {
        var game = _gameRepository.LoadGame(GameName);
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
                if (Winner != null)
                {
                    _gameRepository.DeleteGame(GameName);
                }

                return RedirectToPage("Index");
            }
        }
        
        var amount = Piece == EGamePiece.X ? TicTacTwoBrain.PlayerXPieces : TicTacTwoBrain.PlayerOPieces;
        if (amount > 0 && !TicTacTwoBrain.IsGridFull())
        {
            Choices.Add("Put a new piece on the grid");
        }

        if (TicTacTwoBrain.EnoughMovesForMoreOptions())
        {
            if (!TicTacTwoBrain.IsGridFull() && TicTacTwoBrain.CheckPieceInBoard(Piece))
            {
                Choices.Add("Move one of your pieces to another spot in the grid");
            }

            if (TicTacTwoBrain.GridSize < TicTacTwoBrain.DimX)
            {
                Choices.Add("Move grid one spot horizontally, vertically or diagonally");
            }
        }

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
                var direction = parts[1];
                isMoveValid = direction switch
                {
                    "up" => TicTacTwoBrain.MoveGrid(TicTacTwoBrain.GridCoordinates[0],
                        TicTacTwoBrain.GridCoordinates[1] - 1),
                    "down" => TicTacTwoBrain.MoveGrid(TicTacTwoBrain.GridCoordinates[0],
                        TicTacTwoBrain.GridCoordinates[1] + 1),
                    "right" => TicTacTwoBrain.MoveGrid(TicTacTwoBrain.GridCoordinates[0] + 1,
                        TicTacTwoBrain.GridCoordinates[1]),
                    "left" => TicTacTwoBrain.MoveGrid(TicTacTwoBrain.GridCoordinates[0] - 1,
                        TicTacTwoBrain.GridCoordinates[1]),
                    "diagonalUpL" => TicTacTwoBrain.MoveGrid(TicTacTwoBrain.GridCoordinates[0] - 1,
                        TicTacTwoBrain.GridCoordinates[1] - 1),
                    "diagonalUpR" => TicTacTwoBrain.MoveGrid(TicTacTwoBrain.GridCoordinates[0] + 1,
                        TicTacTwoBrain.GridCoordinates[1] - 1),
                    "diagonalDownL" => TicTacTwoBrain.MoveGrid(TicTacTwoBrain.GridCoordinates[0] - 1,
                        TicTacTwoBrain.GridCoordinates[1] + 1),
                    "diagonalDownR" => TicTacTwoBrain.MoveGrid(TicTacTwoBrain.GridCoordinates[0] + 1,
                        TicTacTwoBrain.GridCoordinates[1] + 1),
                    _ => isMoveValid
                };

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