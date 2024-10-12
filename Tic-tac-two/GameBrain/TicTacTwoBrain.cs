namespace GameBrain;

public class TicTacTwoBrain
{
    private EGamePiece[,] _gameBoard;
    private EGamePiece _nextMoveBy { get; set; }
    
    private GameConfiguration _gameConfiguration;
    
    private int _playerXPieces { get; set; }
    
    private int _playerYPieces { get; set; }

    private List<int> _gridCoordinates;

    private string _playerX;
    private string _playerY;
    

    public TicTacTwoBrain(GameConfiguration gameConfiguration)
    {
        _gameConfiguration = gameConfiguration;
        _gameBoard = new EGamePiece[gameConfiguration.BoardSize, gameConfiguration.BoardSize];
        _playerXPieces = gameConfiguration.Pieces;
        _playerYPieces = gameConfiguration.Pieces;
        _gridCoordinates = gameConfiguration.GridCoordinates;
        _playerX = gameConfiguration.PlayerX;
        _playerY = gameConfiguration.PlayerY;
        _nextMoveBy = gameConfiguration.StartingPiece;
        if (_nextMoveBy != EGamePiece.O)
        {
            _nextMoveBy = gameConfiguration.StartingPiece;
        }
    }
    
    public string PlayerX => _playerX;
    
    public string PlayerY => _playerY;
    
    //public void SetNextMoveBy(EGamePiece piece) => _nextMoveBy = piece;
    
    public int DimX => _gameBoard.GetLength(0);
    
    public int DimY => _gameBoard.GetLength(1);

    public EGamePiece NextMoveBy => _nextMoveBy;

    public List<int> GridCoordinates => _gameConfiguration.GridCoordinates;

    public int GridSize => _gameConfiguration.GridSize;

    public EGamePiece[,] GameBoard
    {
        get => GetBoard();
        private set => _gameBoard = value;
    }
    
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
        var playerPieces = _nextMoveBy == EGamePiece.X ? _playerXPieces : _playerYPieces;
        if (_gameBoard[x, y] != EGamePiece.Empty || !(x >= _gridCoordinates[0] && x <= _gridCoordinates[0] + GridSize - 1) 
            || !(y >= _gridCoordinates[1] && y <= _gridCoordinates[1] + GridSize - 1) || playerPieces == 0)
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
            _playerYPieces--;
        }
        
        _nextMoveBy = _nextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
        return true;
    }

    public bool ChangePieceLocation(int oldX, int oldY, int newX, int newY)
    {
        if (_gameBoard[oldX, oldY] != _nextMoveBy || _gameBoard[newX, newY] != EGamePiece.Empty
                                                  || !(newX >= _gridCoordinates[0] && newX <= _gridCoordinates[1] + 2)
                                                  || !(newY >= _gridCoordinates[0] && newY <= _gridCoordinates[1] + 2) 
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
        if (Math.Abs(x - _gridCoordinates[0]) != 1 && Math.Abs(y - _gridCoordinates[1]) != 1 || 
            x + _gameConfiguration.GridSize > DimX || y + _gameConfiguration.GridSize > DimY || x < 0 || y < 0 || 
            x == _gridCoordinates[0]  && y == _gridCoordinates[1])
        {
            return false;
        }
        _gridCoordinates[0] = x;
        _gridCoordinates[1] = y;
        _nextMoveBy = _nextMoveBy == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
        return true;
    }

    public void ResetGame()
    {
        _gameBoard = new EGamePiece[_gameBoard.GetLength(0), _gameBoard.GetLength(1)];
        _nextMoveBy = EGamePiece.X;
    }

    public bool EnoughMovesForMoreOptions()
    {
        return _playerXPieces <= _gameConfiguration.Pieces - _gameConfiguration.MovePieceAfterMoves &&
               _playerYPieces <= _gameConfiguration.Pieces - _gameConfiguration.MovePieceAfterMoves;
    }

    public bool WinningCondition()
    {
        return CheckWinConditionHorizontally() || CheckWinConditionVertically() 
                                               || CheckWinConditionDiagonally1() || CheckWinConditionDiagonally2();
    }

    private bool CheckWinConditionHorizontally()
    {
        for (var y = _gridCoordinates[1]; y < _gridCoordinates[1] + GridSize; y++)
        {
            var count = 0;
            var piece = EGamePiece.Empty;
            for (var x = _gridCoordinates[0]; x < _gridCoordinates[0] + GridSize; x++)
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
        for (var x = _gridCoordinates[0]; x < _gridCoordinates[0] + GridSize; x++)
        {
            var count = 0;
            var piece = EGamePiece.Empty;
            for (var y = _gridCoordinates[0]; y < _gridCoordinates[0] + GridSize; y++)
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
        for (var x = _gridCoordinates[0]; x < _gridCoordinates[0] + GridSize; x++)
        {
            var count = 0;
            var piece = EGamePiece.Empty;
            var y = _gridCoordinates[1];
            for (var x2 = x; x2 > _gridCoordinates[0] - 1; x2--)
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
            for (var x = _gridCoordinates[0] + 1; x < _gridCoordinates[0] + GridSize - 1; x++)
            {
                var count = 0;
                var piece = EGamePiece.Empty;
                var y = _gridCoordinates[1] + GridSize - 1;
                for (var i = 0; i < GridSize - 1; i++)
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
        for (var x = _gridCoordinates[0] + GridSize - 1; x >= _gridCoordinates[0]; x--)
        {
            var count = 0;
            var piece = EGamePiece.Empty;
            var y = _gridCoordinates[1];
            for (var x2 = x; x2 < _gridCoordinates[0] + GridSize; x2++)
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
            for (var x = _gridCoordinates[0] + GridSize - 2; x >= _gridCoordinates[0]; x--)
            {
                var count = 0;
                var piece = EGamePiece.Empty;
                var y = _gridCoordinates[1] + GridSize - 1;
                for (var i = 0; i < GridSize - 1; i++)
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
