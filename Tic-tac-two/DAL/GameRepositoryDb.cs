using System.Text.Json;
using Domain;
using GameBrain;

namespace DAL;

public class GameRepositoryDb(AppDbContext context) : IGameRepository
{
    public string SaveGame(GameState state, string gameConfigName, EGamePiece piece)
    {
        var configuration = context.Configurations.FirstOrDefault(c => c.Name == gameConfigName);
        
        if (configuration == null)
        {
            throw new Exception($"Configuration with name '{gameConfigName}' not found.");
        }
        
        var random = new Random();
        var max = (int)Math.Pow(10, 6) - 1;
        string passwordX;
        string passwordO;
        switch (piece)
        {
            case EGamePiece.Empty:
                passwordX = random.Next(0, max + 1).ToString($"D{6}");
                passwordO = random.Next(0, max + 1).ToString($"D{6}");
                break;
            case EGamePiece.X:
                passwordX = random.Next(0, max + 1).ToString($"D{6}");
                passwordO = "-";
                break;
            case EGamePiece.O:
            default:
                passwordX = "-";
                passwordO = random.Next(0, max + 1).ToString($"D{6}");
                break;
        }
        
        var gameState = new SaveGame()
        { 
            Name = gameConfigName + " " + DateTime.Now.ToString("yy-MM-dd_HH-mm-ss"), 
            Game = state.ToString(),
            PasswordX = passwordX,
            PasswordO = passwordO,
            ConfigurationId = configuration.Id,
            Configuration = configuration
        };

        context.GameStates.Add(gameState);
        context.SaveChanges();
        return gameState.Name;
    }

    public GameState? LoadGame(string gameName)
    {
        var games = context.GameStates;
        foreach (var game in games)
        {
            if (game.Name != gameName) continue;
            var gameState = JsonSerializer.Deserialize<GameState>(game.Game);
            if (gameState != null) return gameState;
        }
        return null;
    }

    public List<string> GetGameNames()
    {
        var games = context.GameStates;

        return games.Select(game => game.Name).ToList();
    }

    public string UpdateGame(string jsonStateString, string gameName)
    {
        var gameState = context.GameStates.FirstOrDefault(g => g.Name == gameName);

        if (gameState == null)
        {
            return "Error";
        }
        
        gameState.Game = jsonStateString;
        
        context.SaveChanges();
        
        return gameName;
    }

    public List<string> GetPasswords(string gameName)
    {
        var gameState = context.GameStates.FirstOrDefault(g => g.Name == gameName);
        if (gameState == null)
        {
            return [];
        }
        return [gameState.PasswordX, gameState.PasswordO];
    }

    public void DeleteGame(string gameName)
    {
        var gameState = context.GameStates.FirstOrDefault(g => g.Name == gameName);
        if (gameState != null) context.GameStates.Remove(gameState);
        context.SaveChanges();
    }
}