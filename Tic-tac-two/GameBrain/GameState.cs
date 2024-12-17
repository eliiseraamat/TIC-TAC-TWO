namespace GameBrain;

public class GameState
{
    public EGamePiece[][] GameBoard { get; set; }
    public EGamePiece NextMoveBy { get; set; }
    
    public GameConfiguration GameConfiguration { get; init; }

    public List<int> GridCoordinates { get; set; } = [1, 1];
    
    public int PlayerXPieces { get; set; }
    
    public int PlayerOPieces { get; set; }

    public string PlayerX { get; init; } = "X";

    public string PlayerO { get; init; } = "O";
    
    public EGamePiece StartingPiece { get; init; }
    
    public EGameType GameType { get; init; }

    public GameState() 
    {
        GameBoard = new EGamePiece[5][];
        for (var i = 0; i < GameBoard.Length; i++)
        {
            GameBoard[i] = new EGamePiece[5];
        }

        GameConfiguration = default!;
    }

    public GameState(GameConfiguration gameConfiguration, EGamePiece[][] gameBoard, int playerXPieces, int playerOPieces, 
        List<int> gridCoordinates, string playerX, string playerO, EGamePiece startingPiece, EGameType gameType)
    {
        GameConfiguration = gameConfiguration;
        GameBoard = gameBoard;
        PlayerXPieces = playerXPieces;
        PlayerOPieces = playerOPieces;
        GridCoordinates = gridCoordinates;
        PlayerX = playerX;
        PlayerO = playerO;
        NextMoveBy = startingPiece;
        GameType = gameType;
    }

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}