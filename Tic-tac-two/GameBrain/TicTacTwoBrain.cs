namespace GameBrain;

public class TicTacTwoBrain
{
    private readonly GameState _gameState;

    private static EGamePiece _startingPiece { get; set; } = EGamePiece.X;


    public TicTacTwoBrain(GameConfiguration gameConfiguration)
    {
        var gameBoard = new EGamePiece[gameConfiguration.BoardSize][];
        for (var x = 0; x < gameBoard.Length; x++)
        {
            gameBoard[x] = new EGamePiece[gameConfiguration.BoardSize];
        }
        
        _gameState = new GameState(
            gameConfiguration, 
            gameBoard, 
            gameConfiguration.Pieces, 
            gameConfiguration.Pieces, 
            new List<int>(gameConfiguration.GridCoordinates), 
            gameConfiguration.PlayerX, 
            gameConfiguration.PlayerO, 
            _startingPiece);
    }
    
    public TicTacTwoBrain(GameState gameState)
    {
        _gameState = gameState;
    }
    
    
    public int PlayerXPieces => _gameState.PlayerXPieces;
    
    public int PlayerOPieces => _gameState.PlayerOPieces;
    
    public string PlayerX => _gameState.PlayerX;
    
    public string PlayerO => _gameState.PlayerO;

    public static void SetStartingPiece(EGamePiece piece) => _startingPiece = piece;
    
    public int DimX => _gameState.GameBoard.Length;
    
    public int DimY => _gameState.GameBoard[0].Length;

    public EGamePiece NextMoveBy => _gameState.NextMoveBy;

    public List<int> GridCoordinates => _gameState.GridCoordinates;

    public int GridSize => _gameState.GameConfiguration.GridSize;

    public string GetGameStateJson()
    {
        return _gameState.ToString();
    }
    
    public string GetGameConfigName() => _gameState.GameConfiguration.Name;

    public EGamePiece[][] GameBoard => GetBoard();

    private EGamePiece[][] GetBoard()
    {
        var copyOfBoard = new EGamePiece[_gameState.GameBoard.GetLength(0)][];
        for (var x = 0; x < _gameState.GameBoard.Length; x++)
        {
            copyOfBoard[x] = new EGamePiece[_gameState.GameBoard[x].Length];
            for (var y = 0; y < _gameState.GameBoard[x].Length; y++)
            {
                copyOfBoard[x][y] = _gameState.GameBoard[x][y];
            }
        }
        return copyOfBoard;
    }

    public bool MakeAMove(int x, int y)
    {
        var playerPieces = _gameState.NextMoveBy == EGamePiece.X ? _gameState.PlayerXPieces : _gameState.PlayerOPieces;
        
        if (x >= _gameState.GameBoard.Length || y >= _gameState.GameBoard[0].Length || _gameState.GameBoard[x][y] != EGamePiece.Empty || !(x >= GridCoordinates[0] && x <= GridCoordinates[0] + GridSize - 1) 
                                                           || !(y >= GridCoordinates[1] && y <= GridCoordinates[1] + GridSize - 1) || playerPieces == 0)
        {
            return false;
        }
        
        _gameState.GameBoard[x][y] = _gameState.NextMoveBy;

        if (_gameState.NextMoveBy == EGamePiece.X)
        {
            _gameState.PlayerXPieces--;
        }
        else
        {
            _gameState.PlayerOPieces--;
        }
        
        _gameState.NextMoveBy = _gameState.NextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
        return true;
    }

    public bool ChangePieceLocation(int oldX, int oldY, int newX, int newY)
    {
        if (_gameState.GameBoard[oldX][oldY] != _gameState.NextMoveBy || _gameState.GameBoard[newX][newY] != EGamePiece.Empty
                                                                       || !(newX >= GridCoordinates[0] && newX <= GridCoordinates[0] + 2)
                                                                       || !(newY >= GridCoordinates[1] && newY <= GridCoordinates[1] + 2) 
                                                                       || !(oldX <= DimX && oldX >= 0) || !(oldY <= DimY && oldY >= 0))
        {
            return false;
        }
        _gameState.GameBoard[oldX][oldY] = EGamePiece.Empty;
        _gameState.GameBoard[newX][newY] = _gameState.NextMoveBy;
        _gameState.NextMoveBy = _gameState.NextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
        return true;
    }

    public bool MoveGrid(int x, int y)
    {
        if (Math.Abs(x - GridCoordinates[0]) != 1 && Math.Abs(y - GridCoordinates[1]) != 1 || 
            x + _gameState.GameConfiguration.GridSize > DimX || y + _gameState.GameConfiguration.GridSize > DimY || x < 0 || y < 0 || 
            x == GridCoordinates[0]  && y == GridCoordinates[1])
        {
            return false;
        }
        GridCoordinates[0] = x;
        GridCoordinates[1] = y;
        _gameState.NextMoveBy = _gameState.NextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
        return true;
    }

    public bool IsGridFull()
    {
        for (var y = GridCoordinates[1]; y < GridCoordinates[1] + GridSize; y++)
        {
            for (var x = GridCoordinates[0]; x < GridCoordinates[0] + GridSize; x++)
            {
                if (_gameState.GameBoard[x][y] == EGamePiece.Empty)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void ResetGame()
    {
        var gameBoard = new EGamePiece[_gameState.GameConfiguration.BoardSize][];
        for (var x = 0; x < gameBoard.Length; x++)
        {
            gameBoard[x] = new EGamePiece[_gameState.GameConfiguration.BoardSize];
        }
        _gameState.GameBoard = gameBoard;
        _gameState.NextMoveBy = _startingPiece;
        _gameState.PlayerXPieces = _gameState.GameConfiguration.Pieces;
        _gameState.PlayerOPieces = _gameState.GameConfiguration.Pieces;
        _gameState.GridCoordinates = new List<int>(_gameState.GameConfiguration.GridCoordinates);
    }

    public bool EnoughMovesForMoreOptions()
    {
        return _gameState.PlayerXPieces <= _gameState.GameConfiguration.Pieces - _gameState.GameConfiguration.MovePieceAfterMoves &&
               _gameState.PlayerOPieces <= _gameState.GameConfiguration.Pieces - _gameState.GameConfiguration.MovePieceAfterMoves;
    }

    public EGamePiece WinningCondition()
    {
        if (CheckWinConditionHorizontally() == EGamePiece.O || CheckWinConditionVertically() == EGamePiece.O ||
            CheckWinConditionDiagonally1() == EGamePiece.O || CheckWinConditionDiagonally2() == EGamePiece.O)
        {
            return EGamePiece.O;
        }

        if (CheckWinConditionHorizontally() == EGamePiece.X || CheckWinConditionVertically() == EGamePiece.X ||
            CheckWinConditionDiagonally1() == EGamePiece.X || CheckWinConditionDiagonally2() == EGamePiece.X)
        {
            return EGamePiece.X;
        }
        return EGamePiece.Empty;
    }

    private EGamePiece CheckWinConditionHorizontally()
    {
        for (var y = GridCoordinates[1]; y < GridCoordinates[1] + GridSize; y++)
        {
            var count = 0;
            var piece = EGamePiece.Empty;
            for (var x = GridCoordinates[0]; x < GridCoordinates[0] + GridSize; x++)
            {
                if (piece == EGamePiece.Empty && _gameState.GameBoard[x][y] != EGamePiece.Empty)
                {
                    piece = _gameState.GameBoard[x][y];
                    count++;
                } else if (piece == _gameState.GameBoard[x][y] && _gameState.GameBoard[x][y] != EGamePiece.Empty)
                {
                    count++;
                }
                else
                {
                    count = 0;
                    piece = _gameState.GameBoard[x][y];
                }

                if (count == _gameState.GameConfiguration.WinCondition)
                {
                    return piece;
                }
            }
        }
        return EGamePiece.Empty;
    }

    private EGamePiece CheckWinConditionVertically()
    {
        for (var x = GridCoordinates[0]; x < GridCoordinates[0] + GridSize; x++)
        {
            var count = 0;
            var piece = EGamePiece.Empty;
            for (var y = GridCoordinates[1]; y < GridCoordinates[1] + GridSize; y++)
            {
                if (piece == EGamePiece.Empty && _gameState.GameBoard[x][y] != EGamePiece.Empty)
                {
                    piece = _gameState.GameBoard[x][y];
                    count++;
                } else if (piece == _gameState.GameBoard[x][y] && _gameState.GameBoard[x][y] != EGamePiece.Empty)
                {
                    count++;
                }
                else
                {
                    count = 0;
                    piece = _gameState.GameBoard[x][y];
                }

                if (count == _gameState.GameConfiguration.WinCondition)
                {
                    return piece;
                }
            }
        }
        return EGamePiece.Empty;
    }

    private EGamePiece CheckWinConditionDiagonally1()
    {
        for (var x = GridCoordinates[0]; x < GridCoordinates[0] + GridSize; x++)
        {
            var count = 0;
            var piece = EGamePiece.Empty;
            var y = GridCoordinates[1];
            for (var x2 = x; x2 > GridCoordinates[0] - 1; x2--)
            {
                if (piece == EGamePiece.Empty && _gameState.GameBoard[x2][y] != EGamePiece.Empty)
                {
                    piece = _gameState.GameBoard[x2][y];
                    count++;
                }
                else if (piece == _gameState.GameBoard[x2][y] && _gameState.GameBoard[x2][y] != EGamePiece.Empty)
                {
                    count++;
                }
                else
                {
                    count = 0;
                    piece = _gameState.GameBoard[x2][y];
                }

                if (count == _gameState.GameConfiguration.WinCondition)
                {
                    return piece;
                }

                y++;
            }
        }

        if (_gameState.GameConfiguration.WinCondition < GridSize)
        {
            for (var x = GridCoordinates[0] + 1; x < GridCoordinates[0] + GridSize - 1; x++)
            {
                var count = 0;
                var piece = EGamePiece.Empty;
                var y = GridCoordinates[1] + GridSize - 1;
                for (var i = 0; y - i <= 0; i++)
                {
                    if (piece == EGamePiece.Empty && _gameState.GameBoard[x + i][y - i] != EGamePiece.Empty)
                    {
                        piece = _gameState.GameBoard[x + i][y - i];
                        count++;
                    }
                    else if (piece == _gameState.GameBoard[x + i][y - i] && _gameState.GameBoard[x + i][y - i] != EGamePiece.Empty)
                    {
                        count++;
                    }
                    else
                    {
                        count = 0;
                        piece = _gameState.GameBoard[x + i][y - i];
                    }

                    if (count == _gameState.GameConfiguration.WinCondition)
                    {
                        return piece;
                    }
                }
            }
        }
        return EGamePiece.Empty;
    }

    private EGamePiece CheckWinConditionDiagonally2()
    {
        for (var x = GridCoordinates[0] + GridSize - 1; x >= GridCoordinates[0]; x--)
        {
            var count = 0;
            var piece = EGamePiece.Empty;
            var y = GridCoordinates[1];
            for (var x2 = x; x2 < GridCoordinates[0] + GridSize; x2++)
            {
                if (piece == EGamePiece.Empty && _gameState.GameBoard[x2][y] != EGamePiece.Empty)
                {
                    piece = _gameState.GameBoard[x2][y];
                    count++;
                }
                else if (piece == _gameState.GameBoard[x2][y] && _gameState.GameBoard[x2][y] != EGamePiece.Empty)
                {
                    count++;
                }
                else
                {
                    count = 0;
                    piece = _gameState.GameBoard[x2][y];
                }

                if (count == _gameState.GameConfiguration.WinCondition)
                {
                    return piece;
                }

                y++;
            }
        }
        if (_gameState.GameConfiguration.WinCondition < GridSize)
        {
            for (var x = GridCoordinates[0] + GridSize - 2; x >= GridCoordinates[0]; x--)
            {
                var count = 0;
                var piece = EGamePiece.Empty;
                var y = GridCoordinates[1] + GridSize - 1;
                for (var i = 0; y - i <= 0; i++)
                {
                    if (piece == EGamePiece.Empty && _gameState.GameBoard[x - i][y - i] != EGamePiece.Empty)
                    {
                        piece = _gameState.GameBoard[x- i][y - i];
                        count++;
                    }
                    else if (piece == _gameState.GameBoard[x - i][y - i] && _gameState.GameBoard[x - i][y - i] != EGamePiece.Empty)
                    {
                        count++;
                    }
                    else
                    {
                        count = 0;
                        piece = _gameState.GameBoard[x - i][y - i];
                    }

                    if (count == _gameState.GameConfiguration.WinCondition)
                    {
                        return piece;
                    }
                }
            }
        }
        return EGamePiece.Empty;
    }
}
