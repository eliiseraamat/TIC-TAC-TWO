namespace DAL;

public static class FileHelper
{
    public static readonly string BasePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
                                             + Path.DirectorySeparatorChar + "tic-tac-two" + Path.DirectorySeparatorChar;

    public const string ConfigExtension = ".config.json";

    public const string GameExtension = ".game.json";

    public const string SearchPattern = "*";
}