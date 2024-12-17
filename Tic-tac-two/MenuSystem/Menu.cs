namespace MenuSystem;

public class Menu
{
    private string Header { get; set; }

    private const string MenuDivider = "===========";

    private List<MenuItem> MenuItems { get; set; }
    
    private EMenuLevel MenuLevel { get; set; }
    
    private bool IsCustomMenu { get; set; }

    private readonly MenuItem _menuItemExit = new MenuItem()
    {
        Shortcut = "E",
        Title = "Exit",
    };

    private readonly MenuItem _menuItemReturn = new MenuItem()
    {
        Shortcut = "R",
        Title = "Return",
    };

    private readonly MenuItem _menuItemReturnMain = new MenuItem()
    {
        Shortcut = "M",
        Title = "return to Main menu",
    };

    public Menu(EMenuLevel menuLevel, string header, List<MenuItem> menuItems, bool isCustomMenu = false)
    {
        MenuLevel = menuLevel;
        
        if (string.IsNullOrWhiteSpace(header))
        {
            throw new ApplicationException("Menu header cannot be empty or null.");
        }
        Header = header;
        
        if (menuItems == null || menuItems.Count == 0)
        {
            throw new ApplicationException("Menu items cannot be null or empty.");
        }
        
        var shortcuts = new HashSet<string>(menuItems.Select(item => item.Shortcut));
        
        if (shortcuts.Count < menuItems.Count || shortcuts.Contains(_menuItemExit.Shortcut) || shortcuts.Contains(_menuItemReturn.Shortcut) ||
            shortcuts.Contains(_menuItemReturnMain.Shortcut))
        {
            throw new ApplicationException("Incorrect menu shortcuts.");
        }
        MenuItems = menuItems;
        IsCustomMenu = isCustomMenu;

        if (MenuLevel != EMenuLevel.Main)
        {
            menuItems.Add(_menuItemReturn);
        }
        
        if (MenuLevel == EMenuLevel.Deep)
        {
            menuItems.Add(_menuItemReturnMain);
        }
        
        menuItems.Add(_menuItemExit);
    }

    public string Run()
    {
        do
        {
            Console.Clear();

            var menuItem = DisplayMenuGetUserChoice();
            var menuReturnValue = "";

            if (menuItem.MenuItemAction != null)
            {
                menuReturnValue = menuItem.MenuItemAction();
                if (IsCustomMenu) return menuReturnValue;
            }

            if (menuItem.Shortcut == _menuItemReturn.Shortcut)
            {
                return menuItem.Shortcut;
            }
            
            if (menuItem.Shortcut == _menuItemExit.Shortcut || menuReturnValue == _menuItemExit.Shortcut)
            {
                return _menuItemExit.Shortcut;
            }
            
            if ((menuItem.Shortcut == _menuItemReturnMain.Shortcut || menuReturnValue == _menuItemReturnMain.Shortcut) && MenuLevel != EMenuLevel.Main)
            {
                return menuItem.Shortcut;
            }
            
        } while (true);
    }

    private MenuItem DisplayMenuGetUserChoice()
    {

        do
        {
            DrawMenu();
            
            var userInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userInput))
            {
                Console.WriteLine("Please choose one of the following options:");
                Console.WriteLine();
            }
            else
            {
                userInput = userInput.ToUpper();
                
                foreach (var menuItem in MenuItems)
                {
                    if (!menuItem.Shortcut.Equals(userInput, StringComparison.CurrentCultureIgnoreCase)) continue;
                    return menuItem;
                }
                
                Console.WriteLine("Please choose one of the following options:");
                Console.WriteLine();
            }
        } while (true);
    }

    private void DrawMenu()
    {
        Console.WriteLine(Header);
        Console.WriteLine(MenuDivider);
        
        foreach (var t in MenuItems)
        {
            Console.WriteLine(t);
        }
        
        Console.WriteLine();
        Console.Write(">");
    }
}