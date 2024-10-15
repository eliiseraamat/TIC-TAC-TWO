namespace DAL;

public class GameRepositoryJson : IGameRepository
{
    public void SaveGame(string jsonStateString, string gameConfigName)
    {
        var fileName = FileHelper.BasePath + gameConfigName + " " + DateTime.Now.ToString("yy-MM-dd") + FileHelper.GameExtension;
        File.WriteAllText(fileName, jsonStateString);
    }
}