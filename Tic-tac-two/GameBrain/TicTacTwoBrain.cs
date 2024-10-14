namespace GameBrain;

public class TicTacTwoBrain
{
    private EGamePiece[,] _gameBoard;
    private EGamePiece _nextMoveBy { get; set; }

    private static EGamePiece _startingPiece { get; set; } = EGamePiece.X;
    
    private readonly GameConfiguration _gameConfiguration;
    
    private int _playerXPieces { get; set; }
    
    private int _playerOPieces { get; set; }


    public TicTacTwoBrain(GameConfiguration gameConfiguration)
    {
        _gameConfiguration = gameConfiguration;
        _gameBoard = new EGamePiece[gameConfiguration.BoardSize, gameConfiguration.BoardSize];
        _playerXPieces = gameConfiguration.Pieces;
        _playerOPieces = gameConfiguration.Pieces;
        GridCoordinates = new List<int>(gameConfiguration.GridCoordinates);
        PlayerX = gameConfiguration.PlayerX;
        PlayerO = gameConfiguration.PlayerY;
        _nextMoveBy = _startingPiece != EGamePiece.X ? EGamePiece.O : gameConfiguration.StartingPiece;
    }
    
    public string PlayerX { get; }

    public string PlayerO { get; }
    
    public int PlayerXPieces => _playerXPieces;
    
    public int PlayerOPieces => _playerOPieces;

    public static void SetStartingPiece(EGamePiece piece) => _startingPiece = piece;
    
    public int DimX => _gameBoard.GetLength(0);
    
    public int DimY => _gameBoard.GetLength(1);

    public EGamePiece NextMoveBy => _nextMoveBy;

    public List<int> GridCoordinates { get; private set; }

    public int GridSize => _gameConfiguration.GridSize;

    public EGamePiece[,] GameBoard => GetBoard();

    private EGamePiece[,] GetBoard()
    {
        var copyOfBoard = new EGamePiece[_gameBoard.GetLength(0), _gameBoard.GetLength(1)];
        for (var x = 0; x < _gameBoard.GetLength(0); x++)
        {
            for (var y = 0; y < _gameBoard.GetLength(1); y++)
            {
                copyOfBoard[x, y] = _gameBoard[x, y];
            }
        }
        return copyOfBoard;
    }

    public bool MakeAMove(int x, int y)
    {
        var playerPieces = _nextMoveBy == EGamePiece.X ? _playerXPieces : _playerOPieces;
        
        if (_gameBoard[x, y] != EGamePiece.Empty || !(x >= GridCoordinates[0] && x <= GridCoordinates[0] + GridSize - 1) 
            || !(y >= GridCoordinates[1] && y <= GridCoordinates[1] + GridSize - 1) || playerPieces == 0)
        {
            return false;
        }
        
        _gameBoard[x, y] = _nextMoveBy;

        if (_nextMoveBy == EGamePiece.X)
        {
            _playerXPieces--;
        }
        else
        {
            _playerOPieces--;
        }
        
        _nextMoveBy = _nextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
        return true;
    }

    public bool ChangePieceLocation(int oldX, int oldY, int newX, int newY)
    {
        if (_gameBoard[oldX, oldY] != _nextMoveBy || _gameBoard[newX, newY] != EGamePiece.Empty
                                                  || !(newX >= GridCoordinates[0] && newX <= GridCoordinates[0] + 2)
                                                  || !(newY >= GridCoordinates[1] && newY <= GridCoordinates[1] + 2) 
                                                  || !(oldX <= DimX && oldX >= 0) || !(oldY <= DimY && oldY >= 0))
        {
            return false;
        }
        _gameBoard[oldX, oldY] = EGamePiece.Empty;
        _gameBoard[newX, newY] = _nextMoveBy;
        _nextMoveBy = _nextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
        return true;
    }

    public bool MoveGrid(int x, int y)
    {
        if (Math.Abs(x - GridCoordinates[0]) != 1 && Math.Abs(y - GridCoordinates[1]) != 1 || 
            x + _gameConfiguration.GridSize > DimX || y + _gameConfiguration.GridSize > DimY || x < 0 || y < 0 || 
            x == GridCoordinates[0]  && y == GridCoordinates[1])
        {
            return false;
        }
        GridCoordinates[0] = x;
        GridCoordinates[1] = y;
        _nextMoveBy = _nextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
        return true;
    }

    public bool IsGridFull()
    {
        for (var y = GridCoordinates[1]; y < GridCoordinates[1] + GridSize; y++)
        {
            for (var x = GridCoordinates[0]; x < GridCoordinates[0] + GridSize; x++)
            {
                if (_gameBoard[x, y] == EGamePiece.Empty)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void ResetGame()
    {
        _gameBoard = new EGamePiece[_gameConfiguration.BoardSize, _gameConfiguration.BoardSize];
        _nextMoveBy = _startingPiece;
        _playerXPieces = _gameConfiguration.Pieces;
        _playerOPieces = _gameConfiguration.Pieces;
        GridCoordinates = new List<int>(_gameConfiguration.GridCoordinates);
    }

    public bool EnoughMovesForMoreOptions()
    {
        return _playerXPieces <= _gameConfiguration.Pieces - _gameConfiguration.MovePieceAfterMoves &&
               _playerOPieces <= _gameConfiguration.Pieces - _gameConfiguration.MovePieceAfterMoves;
    }

    public bool WinningCondition()
    {
        return CheckWinConditionHorizontally() || CheckWinConditionVertically() 
                                               || CheckWinConditionDiagonally1() || CheckWinConditionDiagonally2();
    }

    private bool CheckWinConditionHorizontally()
    {
        for (var y = GridCoordinates[1]; y < GridCoordinates[1] + GridSize; y++)
        {
            var count = 0;
            var piece = EGamePiece.Empty;
            for (var x = GridCoordinates[0]; x < GridCoordinates[0] + GridSize; x++)
            {
                if (piece == EGamePiece.Empty && _gameBoard[x, y] != EGamePiece.Empty)
                {
                    piece = _gameBoard[x, y];
                    count++;
                } else if (piece == _gameBoard[x, y] && _gameBoard[x, y] != EGamePiece.Empty)
                {
                    count++;
                }
                else
                {
                    count = 0;
                    piece = _gameBoard[x, y];
                }

                if (count == _gameConfiguration.WinCondition)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool CheckWinConditionVertically()
    {
        for (var x = GridCoordinates[0]; x < GridCoordinates[0] + GridSize; x++)
        {
            var count = 0;
            var piece = EGamePiece.Empty;
            for (var y = GridCoordinates[1]; y < GridCoordinates[1] + GridSize; y++)
            {
                if (piece == EGamePiece.Empty && _gameBoard[x, y] != EGamePiece.Empty)
                {
                    piece = _gameBoard[x, y];
                    count++;
                } else if (piece == _gameBoard[x, y] && _gameBoard[x, y] != EGamePiece.Empty)
                {
                    count++;
                }
                else
                {
                    count = 0;
                    piece = _gameBoard[x, y];
                }

                if (count == _gameConfiguration.WinCondition)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool CheckWinConditionDiagonally1()
    {
        for (var x = GridCoordinates[0]; x < GridCoordinates[0] + GridSize; x++)
        {
            var count = 0;
            var piece = EGamePiece.Empty;
            var y = GridCoordinates[1];
            for (var x2 = x; x2 > GridCoordinates[0] - 1; x2--)
            {
                if (piece == EGamePiece.Empty && _gameBoard[x2, y] != EGamePiece.Empty)
                {
                    piece = _gameBoard[x2, y];
                    count++;
                }
                else if (piece == _gameBoard[x2, y] && _gameBoard[x2, y] != EGamePiece.Empty)
                {
                    count++;
                }
                else
                {
                    count = 0;
                    piece = _gameBoard[x2, y];
                }

                if (count == _gameConfiguration.WinCondition)
                {
                    return true;
                }

                y++;
            }
        }

        if (_gameConfiguration.WinCondition < GridSize)
        {
            for (var x = GridCoordinates[0] + 1; x < GridCoordinates[0] + GridSize - 1; x++)
            {
                var count = 0;
                var piece = EGamePiece.Empty;
                var y = GridCoordinates[1] + GridSize - 1;
                for (var i = 0; y - i <= 0; i++)
                {
                    if (piece == EGamePiece.Empty && _gameBoard[x + i, y - i] != EGamePiece.Empty)
                    {
                        piece = _gameBoard[x + i, y - i];
                        count++;
                    }
                    else if (piece == _gameBoard[x + i, y - i] && _gameBoard[x + i, y - i] != EGamePiece.Empty)
                    {
                        count++;
                    }
                    else
                    {
                        count = 0;
                        piece = _gameBoard[x + i, y - i];
                    }

                    if (count == _gameConfiguration.WinCondition)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool CheckWinConditionDiagonally2()
    {
        for (var x = GridCoordinates[0] + GridSize - 1; x >= GridCoordinates[0]; x--)
        {
            var count = 0;
            var piece = EGamePiece.Empty;
            var y = GridCoordinates[1];
            for (var x2 = x; x2 < GridCoordinates[0] + GridSize; x2++)
            {
                if (piece == EGamePiece.Empty && _gameBoard[x2, y] != EGamePiece.Empty)
                {
                    piece = _gameBoard[x2, y];
                    count++;
                }
                else if (piece == _gameBoard[x2, y] && _gameBoard[x2, y] != EGamePiece.Empty)
                {
                    count++;
                }
                else
                {
                    count = 0;
                    piece = _gameBoard[x2, y];
                }

                if (count == _gameConfiguration.WinCondition)
                {
                    return true;
                }

                y++;
            }
        }
        if (_gameConfiguration.WinCondition < GridSize)
        {
            for (var x = GridCoordinates[0] + GridSize - 2; x >= GridCoordinates[0]; x--)
            {
                var count = 0;
                var piece = EGamePiece.Empty;
                var y = GridCoordinates[1] + GridSize - 1;
                for (var i = 0; y - i <= 0; i++)
                {
                    if (piece == EGamePiece.Empty && _gameBoard[x - i, y - i] != EGamePiece.Empty)
                    {
                        piece = _gameBoard[x- i, y - i];
                        count++;
                    }
                    else if (piece == _gameBoard[x - i, y - i] && _gameBoard[x - i, y - i] != EGamePiece.Empty)
                    {
                        count++;
                    }
                    else
                    {
                        count = 0;
                        piece = _gameBoard[x - i, y - i];
                    }

                    if (count == _gameConfiguration.WinCondition)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
