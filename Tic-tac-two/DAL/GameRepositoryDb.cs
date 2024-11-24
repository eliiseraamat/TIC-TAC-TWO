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
    
    public int SaveGame(string jsonStateString, string gameConfigName)
    {
        var configuration = _ctx.Configurations.FirstOrDefault(c => c.Name == gameConfigName);
        
        if (configuration == null)
        {
            throw new Exception($"Configuration with name '{gameConfigName}' not found.");
        }
        
        var gameState = new SaveGame()
        { 
            Name = gameConfigName + " " + DateTime.Now.ToString("yy-MM-dd_HH-mm-ss"), 
            Game = jsonStateString,
            ConfigurationId = configuration.Id,
            Configuration = configuration
        };

        _ctx.GameStates.Add(gameState);
        _ctx.SaveChanges();
        return gameState.Id;
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
}