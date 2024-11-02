using System.Text.Json;
using Domain;
using GameBrain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class GameRepositoryDb : IGameRepository
{
    private readonly string _connectionString = $"Data Source={FileHelper.BasePath}app.db";
    private AppDbContext _ctx;
    
    public GameRepositoryDb()
    {
        var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connectionString)
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .Options;
        
        _ctx = new AppDbContext(contextOptions);
    }
    
    public void SaveGame(string jsonStateString, string gameConfigName)
    {
        var gameState = new SaveGame()
        { 
            Name = gameConfigName + " " + DateTime.Now.ToString("yy-MM-dd_HH-mm-ss"), 
            Game = jsonStateString,
        };

        _ctx.GameStates.Add(gameState);
        _ctx.SaveChanges();
    }

    public GameState LoadGame(string gameName)
    {
        var games = _ctx.GameStates;
        foreach (var gameJson in games)
        {
            if (gameJson.Name == gameName)
            {
                JsonSerializer.Deserialize<GameState>(gameJson.Game);
            }
        }
        throw new Exception("Failed to load the game.");
    }

    public List<string> GetGameNames()
    {
        var names = new List<string>();
        var games = _ctx.GameStates;
        foreach (var game in games)
        {
            names.Add(game.Name);
        }

        return names;
    }
}