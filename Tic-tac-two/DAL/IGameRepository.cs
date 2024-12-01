using GameBrain;

namespace DAL;

public interface IGameRepository
{
    public string SaveGame(string jsonStateString, string gameConfigName);
    
    public GameState LoadGame(string fileName);
    
    public List<string> GetGameNames();
    
    public string UpdateGame(string jsonStateString, string gameName);
}