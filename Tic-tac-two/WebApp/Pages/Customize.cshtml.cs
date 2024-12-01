using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class Customize : PageModel
{
    private readonly IConfigRepository _configRepository;
    private readonly IGameRepository _gameRepository;


    public Customize(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
    }
    
    [BindProperty(SupportsGet = true)] 
    public string ConfigName { get; set; } = default!;
    
    [BindProperty]
    public int BoardSize { get; set; }
    
    [BindProperty]
    public int GridSize { get; set; }
    
    [BindProperty]
    public int PieceNr { get; set; }

    [BindProperty] 
    public int WinCondition { get; set; }
    
    [BindProperty] 
    public int MovePieceAfterMoves { get; set; }
    
    [BindProperty] 
    public int x { get; set; }
    
    [BindProperty] 
    public int y { get; set; }

    public void OnGet()
    {
        
    }
}