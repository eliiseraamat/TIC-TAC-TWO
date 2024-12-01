using DAL;
using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly IConfigRepository _configRepository;
    private readonly IGameRepository _gameRepository;


    public IndexModel(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
    }
    public SelectList ConfigSelectList { get; set; } = default!;
    
    [BindProperty]
    public string ConfigName { get; set; } = default!;
    

    public void OnGet()
    {
        var selectListData = _configRepository.GetConfigurationNames()
            .Select(name => new {id = name, value = name})
            .ToList();
        ConfigSelectList = new SelectList(selectListData, "id", "value");
    }
    
    public IActionResult OnPost()
    {
        if (ConfigName == "Customize")
        {
            return RedirectToPage("./Customize", new { ConfigName = ConfigName });
        }
        var chosenConfig = _configRepository.GetConfigurationByName(ConfigName);
        var gameInstance = new TicTacTwoBrain(chosenConfig);
        var gameName = _gameRepository.SaveGame(gameInstance.GetGameStateJson(), ConfigName);
        return RedirectToPage("./PlayGame", new { GameName = gameName, Piece = EGamePiece.X});
    }
}