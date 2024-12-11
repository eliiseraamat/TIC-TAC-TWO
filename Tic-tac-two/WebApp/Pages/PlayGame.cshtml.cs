﻿using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

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
    
    public void OnGet(int? x, int? y)
    {
        var game = _gameRepository.LoadGame(GameName);

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
        }
    }

    public IActionResult OnPost(string action)
    {
        var game = _gameRepository.LoadGame(GameName);
        TicTacTwoBrain = new TicTacTwoBrain(game);
        
        var parts = action.Split('-');
        if (parts[0] == "reset")
        {
            TicTacTwoBrain.ResetGame();
            GameName = _gameRepository.UpdateGame(TicTacTwoBrain.GetGameStateJson(), GameName);
            return RedirectToPage(new { GameName = GameName, Piece = Piece, PasswordX = PasswordX, PasswordO = PasswordO });
        }

        if (parts[0] == "exit")
        {
            return RedirectToPage("Index");
        }

        var isMoveValid = false;

        if (TicTacTwoBrain.NextMoveBy != Piece)
        {
            /*Error = "Not your turn";
            return Page();*/
            TempData["Error"] = "Not your turn";
            return RedirectToPage(new { GameName = GameName, Piece = Piece, PasswordX = PasswordX, PasswordO = PasswordO });
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
            RedirectToPage(new { GameName = GameName, Piece = Piece, Winner = Winner, PasswordX = PasswordX, PasswordO = PasswordO });
        }
        
        if (parts[0] == "move_grid")
        {
            if (!TicTacTwoBrain.EnoughMovesForMoreOptions())
            {
                Error = "Invalid move!";
                return Page();
            }
            var direction = parts[1];
            switch (direction)
            {
                case "up":
                    isMoveValid = TicTacTwoBrain.MoveGrid(
                        TicTacTwoBrain.GridCoordinates[0], TicTacTwoBrain.GridCoordinates[1] - 1);
                    break;
                case "down":
                    isMoveValid = TicTacTwoBrain.MoveGrid(
                        TicTacTwoBrain.GridCoordinates[0], TicTacTwoBrain.GridCoordinates[1] + 1);
                    break;
                case "right":
                    isMoveValid = TicTacTwoBrain.MoveGrid(
                        TicTacTwoBrain.GridCoordinates[0] + 1, TicTacTwoBrain.GridCoordinates[1]);
                    break;
                case "left":
                    isMoveValid = TicTacTwoBrain.MoveGrid(
                        TicTacTwoBrain.GridCoordinates[0] - 1, TicTacTwoBrain.GridCoordinates[1]);
                    break;
                case "diagonalUpL":
                    isMoveValid = TicTacTwoBrain.MoveGrid(
                        TicTacTwoBrain.GridCoordinates[0] - 1, TicTacTwoBrain.GridCoordinates[1] - 1);
                    break;
                case "diagonalUpR":
                    isMoveValid = TicTacTwoBrain.MoveGrid(
                        TicTacTwoBrain.GridCoordinates[0] + 1, TicTacTwoBrain.GridCoordinates[1] - 1);
                    break;
                case "diagonalDownL":
                    isMoveValid = TicTacTwoBrain.MoveGrid(
                        TicTacTwoBrain.GridCoordinates[0] - 1, TicTacTwoBrain.GridCoordinates[1] + 1);
                    break;
                case "diagonalDownR":
                    isMoveValid = TicTacTwoBrain.MoveGrid(
                        TicTacTwoBrain.GridCoordinates[0] + 1, TicTacTwoBrain.GridCoordinates[1] + 1);
                    break;
            }

            if (!isMoveValid)
            {
                Error = "You cannot move the grid in this direction!";
                return Page();
            }
        }
        else if (parts[0] == "place_piece")
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
        }
        else if (parts[0] == "select_piece")
        {
            SelectedX = int.Parse(parts[1]);
            SelectedO = int.Parse(parts[2]);
            return Page();
        }

        if (isMoveValid)
        {
            GameName = _gameRepository.UpdateGame(TicTacTwoBrain.GetGameStateJson(), GameName);

            var win = TicTacTwoBrain.WinningCondition();
            if (win != EGamePiece.Empty)
            {
                Winner = win;
            }

            return RedirectToPage(new { GameName = GameName, Piece = Piece, Winner = Winner, PasswordX = PasswordX, PasswordO = PasswordO});
        }

        return Page();
    }
}