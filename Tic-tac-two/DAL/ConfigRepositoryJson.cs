using System.Text.Json;
using GameBrain;

namespace DAL;

public class ConfigRepositoryJson : IConfigRepository
{
    public List<string> GetConfigurationNames()
    {
        CheckAndCreateInitialConfig();

        return Directory
            .GetFiles(FileHelper.BasePath, FileHelper.SearchPattern + FileHelper.ConfigExtension)
            .Select(fileNameParts => Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(fileNameParts)))
            .ToList();
    }

    public GameConfiguration GetConfigurationByName(string name)
    {
        var configJsonStr = File.ReadAllText(FileHelper.BasePath + name + FileHelper.ConfigExtension);
        var config = JsonSerializer.Deserialize<GameConfiguration>(configJsonStr);
        return config;
    }

    private void CheckAndCreateInitialConfig()
    {
        if (!Directory.Exists(FileHelper.BasePath))
        {
            Directory.CreateDirectory(FileHelper.BasePath);
        }
        var data = Directory.GetFiles(FileHelper.BasePath, FileHelper.SearchPattern + FileHelper.ConfigExtension).ToList();
        if (data.Count == 0)
        {
            var hardCodedRepo = new ConfigRepositoryInMemory();
            var optionNames = hardCodedRepo.GetConfigurationNames();
            foreach (var optionName in optionNames)
            {
                var gameOption = hardCodedRepo.GetConfigurationByName(optionName);
                var optionJsonStr = JsonSerializer.Serialize(gameOption);
                File.WriteAllText(FileHelper.BasePath + gameOption.Name + FileHelper.ConfigExtension, optionJsonStr);
            }
        }
    }
}