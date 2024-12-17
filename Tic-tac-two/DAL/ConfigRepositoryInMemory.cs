using GameBrain;

namespace DAL;

public class ConfigRepositoryInMemory : IConfigRepository
{
    private readonly List<GameConfiguration> _gameConfigurations =
    [
        new GameConfiguration()
        {
            Name = "Classical"
        },

        new GameConfiguration()
        {
            Name = "Customize"
        }
    ];
    
    public List<string> GetConfigurationNames()
    {
        return _gameConfigurations
            .Select(config => config.Name).ToList();
    }

    public GameConfiguration GetConfigurationByName(string name)
    {
        return _gameConfigurations.Single(c => c.Name == name);
    }
}