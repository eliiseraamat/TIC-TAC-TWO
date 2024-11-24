using System.Text.Json;
using Domain;
using GameBrain;

namespace DAL;

public class ConfigRepositoryDb : IConfigRepository
{
    private readonly AppDbContext _ctx;
    
    public ConfigRepositoryDb(AppDbContext context)
    {
        _ctx = context;
        CheckAndCreateInitialConfig(_ctx);
    }
    
    public List<string> GetConfigurationNames()
    {
       var configs = _ctx.Configurations.ToList();

       return configs.Select(config => config.Name).ToList();
    }

    public GameConfiguration GetConfigurationByName(string name)
    {
        var configs = _ctx.Configurations.ToList();

        foreach (var config in configs)
        {
            if (config.GameConfig == null) throw new Exception($"Configuration with name '{name}' not found.");
            if (config.Name != name) continue;
            var gameConfig = JsonSerializer.Deserialize<GameConfiguration>(config.GameConfig);
            if (gameConfig == null) throw new Exception($"Configuration with name '{name}' not found.");
            return gameConfig;
        }
        throw new Exception($"Configuration with name '{name}' not found.");
    }
    
    private void CheckAndCreateInitialConfig(AppDbContext ctx)
    {
        if (ctx.Configurations.Any()) return;
        var hardCodedRepo = new ConfigRepositoryInMemory();
        var names = hardCodedRepo.GetConfigurationNames();

        foreach (var name in names)
        {
            var gameOption = hardCodedRepo.GetConfigurationByName(name);
            
            var config = new Configuration
            {
                Name = name,
                GameConfig = JsonSerializer.Serialize(gameOption),
            };

            ctx.Configurations.Add(config);
        }

        ctx.SaveChanges();
    }
}