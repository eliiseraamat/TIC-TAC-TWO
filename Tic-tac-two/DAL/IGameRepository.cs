using GameBrain;

namespace DAL;

public interface IGameRepository
{
    //public string SaveGame(string jsonStateString, string gameConfigName);
    public string SaveGame(GameState gameState, string gameConfigName, EGamePiece piece);
    
    public GameState LoadGame(string fileName);
    
    public List<string> GetGameNames();
    
    public string UpdateGame(string jsonStateString, string gameName);

    public List<string> GetPasswords(string gameName);
}