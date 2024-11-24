using GameBrain;

namespace DAL;

public interface IGameRepository
{
    public int SaveGame(string jsonStateString, string gameConfigName);

    public GameState LoadGame(int id);
    
    public GameState LoadGame(string fileName);
    
    public List<string> GetGameNames();
}