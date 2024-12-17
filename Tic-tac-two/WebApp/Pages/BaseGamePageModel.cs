using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public abstract class BaseGamePageModel : PageModel
{
    public List<string> GetChoices(TicTacTwoBrain instance, EGamePiece piece)
    {
        var choices = new List<string>();
        var amount = piece == EGamePiece.X ? instance.PlayerXPieces : instance.PlayerOPieces;
        if (amount > 0 && !instance.IsGridFull())
        {
            choices.Add("Put a new piece on the grid");
        }

        if (!instance.EnoughMovesForMoreOptions()) return choices;
        if (!instance.IsGridFull() && instance.CheckPieceInBoard(piece))
        {
            choices.Add("Move one of your pieces to another spot in the grid");
        }

        if (instance.GridSize < instance.DimX)
        {
            choices.Add("Move grid one spot horizontally, vertically or diagonally");
        }
        return choices;
    }

    public bool MoveGrid(TicTacTwoBrain instance, string direction)
    {
        var isMoveValid = false;
        isMoveValid = direction switch
        {
            "up" => instance.MoveGrid(instance.GridCoordinates[0],
                instance.GridCoordinates[1] - 1),
            "down" => instance.MoveGrid(instance.GridCoordinates[0],
                instance.GridCoordinates[1] + 1),
            "right" => instance.MoveGrid(instance.GridCoordinates[0] + 1,
                instance.GridCoordinates[1]),
            "left" => instance.MoveGrid(instance.GridCoordinates[0] - 1,
                instance.GridCoordinates[1]),
            "diagonalUpL" => instance.MoveGrid(instance.GridCoordinates[0] - 1,
                instance.GridCoordinates[1] - 1),
            "diagonalUpR" => instance.MoveGrid(instance.GridCoordinates[0] + 1,
                instance.GridCoordinates[1] - 1),
            "diagonalDownL" => instance.MoveGrid(instance.GridCoordinates[0] - 1,
                instance.GridCoordinates[1] + 1),
            "diagonalDownR" => instance.MoveGrid(instance.GridCoordinates[0] + 1,
                instance.GridCoordinates[1] + 1),
            _ => isMoveValid
        };
        return isMoveValid;
    }

    public EGamePiece? GetWinner(TicTacTwoBrain instance)
    {
        var win = instance.WinningCondition();
        if (win != EGamePiece.Empty)
        {
            return win;
        }

        return null;
    }

    public IActionResult Exit(IGameRepository repository, string name, EGamePiece? winner)
    {
        if (winner != null)
        {
            repository.DeleteGame(name);
        }
        return RedirectToPage("Index");
    }
}