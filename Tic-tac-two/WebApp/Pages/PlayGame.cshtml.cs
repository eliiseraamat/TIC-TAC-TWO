using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class PlayGame : PageModel
{
    private readonly IConfigRepository _configRepository;
    private readonly IGameRepository _gameRepository;


    public PlayGame(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
    }
    
    [BindProperty(SupportsGet = true)] 
    public int GameId { get; set; } = default!;

    [BindProperty(SupportsGet = true)] 
    public EGamePiece NextMoveBy { get; set; } = default!;
    
    public TicTacTwoBrain TicTacTwoBrain { get; set; } = default!;
    
    public void OnGet(int? x, int? y)
    {
        var game = _gameRepository.LoadGame(GameId);

        TicTacTwoBrain = new TicTacTwoBrain(game);

        if (x != null && y != null)
        {
            TicTacTwoBrain.MakeAMove(x.Value, y.Value);
            GameId = _gameRepository.SaveGame(TicTacTwoBrain.GetGameStateJson(), TicTacTwoBrain.GetGameConfigName());
        }
    }
}