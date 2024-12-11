namespace GameBrain;

public class GameData(string passwordX, string passwordO, GameState gameState)
{
    public string PasswordX { get; set; } = passwordX;
    public string PasswordO { get; set; } = passwordO;
    public GameState GameState { get; set; } = gameState;
    
    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}