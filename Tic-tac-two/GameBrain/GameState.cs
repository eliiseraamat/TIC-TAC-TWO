namespace GameBrain;

public class GameState
{
    public EGamePiece[][] GameBoard { get; set; }
    public EGamePiece NextMoveBy { get; set; }
    
    public GameConfiguration GameConfiguration { get; set; }

    public List<int> GridCoordinates { get; set; }
    
    public int PlayerXPieces { get; set; }
    
    public int PlayerOPieces { get; set; }
    
    public string PlayerX { get; }

    public string PlayerO { get; }

    public GameState(GameConfiguration gameConfiguration, EGamePiece[][] gameBoard, int playerXPieces, int playerOPieces, List<int> gridCoordinates, string playerX, string playerO, EGamePiece startingPiece)
    {
        GameConfiguration = gameConfiguration;
        GameBoard = gameBoard;
        PlayerXPieces = playerXPieces;
        PlayerOPieces = playerOPieces;
        GridCoordinates = gridCoordinates;
        PlayerX = playerX;
        PlayerO = playerO;
        NextMoveBy = startingPiece != EGamePiece.X ? EGamePiece.O : gameConfiguration.StartingPiece;
    }

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}