namespace GameBrain;

public class GameData(string passwordX, string passwordO, GameState gameState)
{
    public string PasswordX { get; init; } = passwordX;
    public string PasswordO { get; init; } = passwordO;
    public GameState GameState { get; set; } = gameState;
    
    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}