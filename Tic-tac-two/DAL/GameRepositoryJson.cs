using System.Text.Json;
using GameBrain;

namespace DAL;

public class GameRepositoryJson : IGameRepository
{
    public string SaveGame(string jsonStateString, string gameConfigName)
    {
        var gameName = gameConfigName + " " + DateTime.Now.ToString("yy-MM-dd_HH-mm-ss");
        var fileName = FileHelper.BasePath + gameName + FileHelper.GameExtension;
        File.WriteAllText(fileName, jsonStateString);
        return gameName;
    }

    public GameState LoadGame(string fileName)
    {
        var configJsonStr = File.ReadAllText(FileHelper.BasePath + fileName + FileHelper.GameExtension);
        var config = JsonSerializer.Deserialize<GameState>(configJsonStr);
        if (config == null) 
        {
            throw new Exception("Failed to load the game: deserialization returned null.");
        }
    
        return config;
    }

    public List<string> GetGameNames()
    {
        return Directory
            .GetFiles(FileHelper.BasePath, FileHelper.SearchPattern + FileHelper.GameExtension)
            .Select(fileNameParts => Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(fileNameParts)))
            .ToList();
    }

    public string UpdateGame(string jsonStateString, string gameName)
    {
        var fileName = FileHelper.BasePath + gameName + FileHelper.GameExtension;
        File.WriteAllText(fileName, jsonStateString);
        return gameName;
    }
}