using System.Text.Json;
using GameBrain;

namespace DAL;

public class GameRepositoryJson : IGameRepository
{
    public int SaveGame(string jsonStateString, string gameConfigName)
    {
        var fileName = FileHelper.BasePath + gameConfigName + " " + DateTime.Now.ToString("yy-MM-dd_HH-mm-ss") + FileHelper.GameExtension;
        File.WriteAllText(fileName, jsonStateString);
        return -1;
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
    
    public GameState LoadGame(int id)
    {
        throw new NotImplementedException();
    }

    public List<string> GetGameNames()
    {
        return Directory
            .GetFiles(FileHelper.BasePath, FileHelper.SearchPattern + FileHelper.GameExtension)
            .Select(fileNameParts => Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(fileNameParts)))
            .ToList();
    }
}