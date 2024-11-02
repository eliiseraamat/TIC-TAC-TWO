using System.Text.Json;
using Domain;
using GameBrain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class ConfigRepositoryDb : IConfigRepository
{
    private readonly string _connectionString = $"Data Source={FileHelper.BasePath}app.db";
    private AppDbContext _ctx;
    
    public ConfigRepositoryDb()
    {
        var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connectionString)
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .Options;

        //using var ctx = new AppDbContext(contextOptions);
        _ctx = new AppDbContext(contextOptions);
        
        //_ctx.Database.Migrate();
        
        CheckAndCreateInitialConfig(_ctx);
    }
    
    public List<string> GetConfigurationNames()
    {
       var configs = _ctx.Configurations.ToList();
       
       List<string> names = new List<string>();

       foreach (var configJson in configs)
       {
           var config = JsonSerializer.Deserialize<GameConfiguration>(configJson.GameConfig);
           names.Add(config?.Name);
       }
       return names;
    }

    public GameConfiguration GetConfigurationByName(string name)
    {
        var configs = _ctx.Configurations.ToList();

        foreach (var configJson in configs)
        {
            var config = JsonSerializer.Deserialize<GameConfiguration>(configJson.GameConfig);
            if (config == null) throw new Exception($"Configuration with name '{name}' not found.");
            if (config.Name == name)
            {
                return config;
            }
        }
        throw new Exception($"Configuration with name '{name}' not found.");
    }
    
    private void CheckAndCreateInitialConfig(AppDbContext ctx)
    {
        if (!ctx.Configurations.Any())
        {
            var hardCodedRepo = new ConfigRepositoryInMemory();
            var names = hardCodedRepo.GetConfigurationNames();

            foreach (var name in names)
            {
                var gameOption = hardCodedRepo.GetConfigurationByName(name);

                
                var config = new Configuration
                {
                    GameConfig = JsonSerializer.Serialize(gameOption),
                };

                ctx.Configurations.Add(config);
            }

            ctx.SaveChanges();
        }
    }
}