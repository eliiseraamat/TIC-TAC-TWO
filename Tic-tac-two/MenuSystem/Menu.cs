namespace MenuSystem;

public class Menu
{
    private string Header { get; set; }

    private static string _menuDivider = "===========";
    
    private List<MenuItem> MenuItems { get; set; }
    
    private EMenuLevel MenuLevel { get; set; }

    private MenuItem _menuItemExit = new MenuItem()
    {
        ShortCut = "E",
        Title = "Exit",
    };
    
    private MenuItem _menuItemReturn = new MenuItem()
    {
        ShortCut = "R",
        Title = "Return",
    };
    
    private MenuItem _menuItemReturnMain = new MenuItem()
    {
        ShortCut = "M",
        Title = "return to Main menu",
    };

    public Menu(EMenuLevel menuLevel, string header, List<MenuItem> menuItems)
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
        
        var shortcuts = new HashSet<string>(menuItems.Select(item => item.ShortCut));
        
        if (shortcuts.Count < menuItems.Count || shortcuts.Contains("E") || shortcuts.Contains("R") ||
            shortcuts.Contains("M"))
        {
            throw new ApplicationException("Incorrect menu shortcuts.");
        }
        MenuItems = menuItems;
        
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
            }

            if (menuItem.ShortCut == _menuItemReturn.ShortCut)
            {
                return menuItem.ShortCut;
            }
            
            if (menuItem.ShortCut == _menuItemExit.ShortCut || menuReturnValue == _menuItemExit.ShortCut)
            {
                return "E";
            }
            
            if ((menuItem.ShortCut == _menuItemReturnMain.ShortCut || menuReturnValue == _menuItemReturnMain.ShortCut) && MenuLevel != EMenuLevel.Main)
            {
                return menuItem.ShortCut;
            }
            
            if (!string.IsNullOrWhiteSpace(menuReturnValue))
            {
                return menuReturnValue;
            }

        } while (true);
    }

    private MenuItem DisplayMenuGetUserChoice()
    {
        var userInput = "";

        do
        {
            DrawMenu();
            
            userInput = Console.ReadLine();

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
                    if (menuItem.ShortCut.ToUpper() != userInput) continue;
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
        Console.WriteLine(_menuDivider);
        
        foreach (var t in MenuItems)
        {
            Console.WriteLine(t);
        }
        
        Console.WriteLine();
        Console.Write(">");
    }
}