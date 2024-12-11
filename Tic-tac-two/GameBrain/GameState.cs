namespace GameBrain;

public class GameState
{
    public EGamePiece[][] GameBoard { get; set; }
    public EGamePiece NextMoveBy { get; set; }
    
    public GameConfiguration GameConfiguration { get; set; }

    public List<int> GridCoordinates { get; set; } = [1, 1];
    
    public int PlayerXPieces { get; set; }
    
    public int PlayerOPieces { get; set; }

    public string PlayerX { get; set; } = "X";

    public string PlayerO { get; set; } = "O";
    
    public EGamePiece StartingPiece { get; set; }
    
    public EGameType GameType { get; set; }

    public GameState() 
    {
        GameBoard = new EGamePiece[5][];
        for (int i = 0; i < GameBoard.Length; i++)
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