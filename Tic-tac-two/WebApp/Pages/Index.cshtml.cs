using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly IConfigRepository _configRepository;


    public IndexModel(IConfigRepository configRepository)
    {
        _configRepository = configRepository;
    }
    public SelectList ConfigSelectList { get; set; } = default!;
    [BindProperty]
    public int ConfigId { get; set; }

    public void OnGet()
    {
        var selectListData = _configRepository.GetConfigurationNames()
            .Select(name => new {id = name, value = name})
            .ToList();
        ConfigSelectList = new SelectList(selectListData, "id", "value");
    }
}