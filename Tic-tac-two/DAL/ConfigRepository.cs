using GameBrain;

namespace DAL;

public class ConfigRepository
{
    private List<GameConfiguration> _gameConfigurations = new List<GameConfiguration>()
    {
        new GameConfiguration()
        {
            Name = "Classical"
        },
    };
    
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