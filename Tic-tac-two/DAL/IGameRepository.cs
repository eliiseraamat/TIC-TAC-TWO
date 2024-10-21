using GameBrain;

namespace DAL;

public interface IGameRepository
{
    public void SaveGame(string jsonStateString, string gameConfigName);

    public GameState LoadGame(string fileName);
    
    public List<string> GetGameNames();
}