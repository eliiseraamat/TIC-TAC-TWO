using System.Diagnostics;
using System.Text.Json;
using Domain;
using GameBrain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class GameRepositoryDb : IGameRepository
{
    private readonly AppDbContext _ctx;
    
    public GameRepositoryDb(AppDbContext context)
    {
        _ctx = context;
    }
    
    public string SaveGame(GameState state, string gameConfigName, EGamePiece piece)
    {
        var configuration = _ctx.Configurations.FirstOrDefault(c => c.Name == gameConfigName);
        
        if (configuration == null)
        {
            throw new Exception($"Configuration with name '{gameConfigName}' not found.");
        }
        
        var random = new Random();
        var max = (int)Math.Pow(10, 6) - 1;
        string passwordX;
        string passwordO;
        if (piece == EGamePiece.Empty)
        {
            passwordX = random.Next(0, max + 1).ToString($"D{6}");
            passwordO = random.Next(0, max + 1).ToString($"D{6}");
        }
        else if (piece == EGamePiece.X)
        {
            passwordX = random.Next(0, max + 1).ToString($"D{6}");
            passwordO = "-";
        }
        else
        {
            passwordX = "-";
            passwordO = random.Next(0, max + 1).ToString($"D{6}");
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

        _ctx.GameStates.Add(gameState);
        _ctx.SaveChanges();
        return gameState.Name;
    }

    public GameState LoadGame(string gameName)
    {
        var games = _ctx.GameStates;
        foreach (var game in games)
        {
            if (game.Name == gameName)
            {
                var gameState = JsonSerializer.Deserialize<GameState>(game.Game);
                if (gameState != null) return gameState;
            }
        }
        throw new Exception("Failed to load the game.");
    }
    
    public GameState LoadGame(int id)
    {
        var games = _ctx.GameStates;
        foreach (var game in games)
        {
            if (game.Id == id)
            {
                var gameState = JsonSerializer.Deserialize<GameState>(game.Game);
                if (gameState != null) return gameState;
            }
        }
        throw new Exception("Failed to load the game.");
    }

    public List<string> GetGameNames()
    {
        var games = _ctx.GameStates;

        return games.Select(game => game.Name).ToList();
    }

    public string UpdateGame(string jsonStateString, string gameName)
    {
        var gameState = _ctx.GameStates.FirstOrDefault(g => g.Name == gameName);

        if (gameState == null)
        {
            throw new Exception($"Game with name '{gameName}' not found.");
        }
        
        gameState.Game = jsonStateString;
        
        _ctx.SaveChanges();
        
        return gameName;
    }

    public List<string> GetPasswords(string gameName)
    {
        var gameState = _ctx.GameStates.FirstOrDefault(g => g.Name == gameName);
        if (gameState == null)
        {
            throw new Exception($"Game with name '{gameName}' not found.");
        }
        return [gameState.PasswordX, gameState.PasswordO];
    }

    public void DeleteGame(string gameName)
    {
        var gameState = _ctx.GameStates.FirstOrDefault(g => g.Name == gameName);
        _ctx.GameStates.Remove(gameState);
        _ctx.SaveChanges();
    }
}