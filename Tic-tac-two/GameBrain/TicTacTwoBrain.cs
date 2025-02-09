﻿namespace GameBrain;

public class TicTacTwoBrain
{
    private readonly GameState _gameState;

    private static EGamePiece StartingPiece { get; set; } = EGamePiece.X;


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
            [..gameConfiguration.GridCoordinates], 
            gameConfiguration.PlayerX, 
            gameConfiguration.PlayerO, 
            StartingPiece,
            gameConfiguration.GameType);
    }
    
    public TicTacTwoBrain(GameState gameState)
    {
        _gameState = gameState;
    }
    
    public GameState GameState => _gameState;
    
    
    public int PlayerXPieces => _gameState.PlayerXPieces;
    
    public int PlayerOPieces => _gameState.PlayerOPieces;
    
    public string PlayerX => _gameState.PlayerX;
    
    public string PlayerO => _gameState.PlayerO;
    
    public int DimX => _gameState.GameBoard.Length;
    
    public int DimY => _gameState.GameBoard[0].Length;

    public EGamePiece NextMoveBy => _gameState.NextMoveBy;

    public List<int> GridCoordinates => _gameState.GridCoordinates;

    public int GridSize => _gameState.GameConfiguration.GridSize;
    
    public EGameType GameType => _gameState.GameConfiguration.GameType;

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
                                                                       || !(newX >= GridCoordinates[0] && newX <= GridCoordinates[0] + GridSize)
                                                                       || !(newY >= GridCoordinates[1] && newY <= GridCoordinates[1] + GridSize) 
                                                                       || !(oldX <= DimX && oldX >= 0) || !(oldY <= DimY && oldY >= 0)
                                                                       || !EnoughMovesForMoreOptions())
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
            x == GridCoordinates[0]  && y == GridCoordinates[1] || !EnoughMovesForMoreOptions())
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
        _gameState.NextMoveBy = StartingPiece;
        _gameState.PlayerXPieces = _gameState.GameConfiguration.Pieces;
        _gameState.PlayerOPieces = _gameState.GameConfiguration.Pieces;
        _gameState.GridCoordinates = [.._gameState.GameConfiguration.GridCoordinates];
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
                else if (_gameState.GameBoard[x][y] == EGamePiece.Empty)
                {
                    count = 0;
                    piece = _gameState.GameBoard[x][y];
                }
                else
                {
                    count = 1;
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
                } else if (_gameState.GameBoard[x][y] == EGamePiece.Empty)
                {
                    count = 0;
                    piece = _gameState.GameBoard[x][y];
                }
                else
                {
                    count = 1;
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
                } else if (_gameState.GameBoard[x2][y] == EGamePiece.Empty)
                {
                    count = 0;
                    piece = _gameState.GameBoard[x2][y];
                }
                else
                {
                    count = 1;
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
            var counter = GridSize;
            for (var x = GridCoordinates[0] + 1; x < GridCoordinates[0] + GridSize - 1; x++)
            {
                var count = 0;
                var piece = EGamePiece.Empty;
                var y = GridCoordinates[1] + GridSize - 1;
                counter--;
                for (var i = 0; i < counter; i++)
                {
                    if (piece == EGamePiece.Empty && _gameState.GameBoard[x + i][y - i] != EGamePiece.Empty)
                    {
                        piece = _gameState.GameBoard[x + i][y - i];
                        count++;
                    }
                    else if (piece == _gameState.GameBoard[x + i][y - i] && _gameState.GameBoard[x + i][y - i] != EGamePiece.Empty)
                    {
                        count++;
                    } else if (_gameState.GameBoard[x + i][y - i] == EGamePiece.Empty)
                    {
                        count = 0;
                        piece = _gameState.GameBoard[x + i][y - i];
                    }
                    else
                    {
                        count = 1;
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
                } else if (_gameState.GameBoard[x2][y] == EGamePiece.Empty)
                {
                    count = 0;
                    piece = _gameState.GameBoard[x2][y];
                }
                else
                {
                    count = 1;
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
            var counter = GridSize;
            for (var x = GridCoordinates[0] + GridSize - 2; x >= GridCoordinates[0]; x--)
            {
                var count = 0;
                var piece = EGamePiece.Empty;
                var y = GridCoordinates[1] + GridSize - 1;
                counter--;
                for (var i = 0; i < counter; i++)
                {
                    if (piece == EGamePiece.Empty && _gameState.GameBoard[x - i][y - i] != EGamePiece.Empty)
                    {
                        piece = _gameState.GameBoard[x- i][y - i];
                        count++;
                    }
                    else if (piece == _gameState.GameBoard[x - i][y - i] && _gameState.GameBoard[x - i][y - i] != EGamePiece.Empty)
                    {
                        count++;
                    } else if (_gameState.GameBoard[x - i][y - i] == EGamePiece.Empty)
                    {
                        count = 0;
                        piece = _gameState.GameBoard[x - i][y - i];
                    }
                    else
                    {
                        count = 1;
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

    public List<int> AiNewPiece(EGamePiece piece)
    {
        var horizontally = CheckAiHorizontally(piece);
        if (horizontally.Count > 0)
        {
            return horizontally;
        }
        var vertically = CheckAiVertically(piece);
        if (vertically.Count > 0)
        {
            return vertically;
        }
        var diagonally1 = CheckAiDiagonally1(piece);
        if (diagonally1.Count > 0)
        {
            return diagonally1;
        }
        var diagonally2 = CheckAiDiagonally2(piece);
        if (diagonally2.Count > 0)
        {
            return diagonally2;
        }
        var opponentPiece = piece == EGamePiece.X ? EGamePiece.O : EGamePiece.X;
        var opponentHorizontally = CheckAiHorizontally(opponentPiece);
        if (opponentHorizontally.Count > 0)
        {
            return opponentHorizontally;
        }
        var opponentVertically = CheckAiVertically(opponentPiece);
        if (opponentVertically.Count > 0)
        {
            return opponentVertically;
        }
        var opponentDiagonally1 = CheckAiDiagonally1(opponentPiece);
        if (opponentDiagonally1.Count > 0)
        {
            return opponentDiagonally1;
        }
        var opponentDiagonally2 = CheckAiDiagonally2(opponentPiece);
        if (opponentDiagonally2.Count > 0)
        {
            return opponentDiagonally2;
        }

        return !IsGridFull() ? RandomAiMove() : [];
    }
    
    private List<int> CheckAiHorizontally(EGamePiece piece)
    {
        for (var y = GridCoordinates[1]; y < GridCoordinates[1] + GridSize; y++)
        {
            var emptyCount = 0;
            var coordinates = new List<int>();
            var pieceCount = 0;
            for (var x = GridCoordinates[0]; x < GridCoordinates[0] + GridSize; x++)
            {
                if (_gameState.GameBoard[x][y] == piece)
                {
                    emptyCount = 0;
                    pieceCount++;
                } else if (_gameState.GameBoard[x][y] == EGamePiece.Empty && coordinates.Count > 0)
                {
                    coordinates.RemoveRange(0, coordinates.Count);
                    coordinates.Add(x);
                    coordinates.Add(y);
                    emptyCount++;
                } else if (_gameState.GameBoard[x][y] == EGamePiece.Empty && coordinates.Count == 0)
                {
                    coordinates.Add(x);
                    coordinates.Add(y);
                    emptyCount++;
                }
                else if (_gameState.GameBoard[x][y] != piece && coordinates.Count > 0)
                {
                    emptyCount = 0;
                    coordinates.RemoveRange(0, coordinates.Count);
                }

                if (pieceCount == _gameState.GameConfiguration.WinCondition - 1 && coordinates.Count != 0)
                {
                    return coordinates;
                }

                if (emptyCount > 1)
                {
                    pieceCount = 0;
                    emptyCount = 0;
                }
            }
        }
        return [];
    }
    
    private List<int> CheckAiVertically(EGamePiece piece)
    {
        for (var x = GridCoordinates[0]; x < GridCoordinates[0] + GridSize; x++)
        {
            var emptyCount = 0;
            var coordinates = new List<int>();
            var pieceCount = 0;
            for (var y = GridCoordinates[1]; y < GridCoordinates[1] + GridSize; y++)
            {
                if (_gameState.GameBoard[x][y] == piece)
                {
                    emptyCount = 0;
                    pieceCount++;
                } else if (_gameState.GameBoard[x][y] == EGamePiece.Empty && coordinates.Count > 0)
                {
                    coordinates.RemoveRange(0, coordinates.Count);
                    coordinates.Add(x);
                    coordinates.Add(y);
                    emptyCount++;
                } else if (_gameState.GameBoard[x][y] == EGamePiece.Empty && coordinates.Count == 0)
                {
                    coordinates.Add(x);
                    coordinates.Add(y);
                    emptyCount++;
                }
                else if (_gameState.GameBoard[x][y] != piece && coordinates.Count > 0)
                {
                    emptyCount = 0;
                    coordinates.RemoveRange(0, coordinates.Count);
                }

                if (pieceCount == _gameState.GameConfiguration.WinCondition - 1 && coordinates.Count != 0)
                {
                    return coordinates;
                }

                if (emptyCount > 1)
                {
                    pieceCount = 0;
                    emptyCount = 0;
                }
            }
        }
        return [];
    }

    private List<int> RandomAiMove()
    {
        do
        {
            var limitX = GridCoordinates[0] + GridSize;
            var limitY = GridCoordinates[1] + GridSize;
            var r = new Random();
            var randomX = r.Next(GridCoordinates[0], limitX);
            var randomY = r.Next(GridCoordinates[1], limitY);
            if (GameBoard[randomX][randomY] == EGamePiece.Empty)
            {
                return [randomX, randomY];
            }
        } while (true);
    }
    
    private List<int> CheckAiDiagonally1(EGamePiece piece)
    {
        for (var x = GridCoordinates[0]; x < GridCoordinates[0] + GridSize; x++)
        {
            var emptyCount = 0;
            var coordinates = new List<int>();
            var pieceCount = 0;
            var y = GridCoordinates[1];
            for (var x2 = x; x2 > GridCoordinates[0] - 1; x2--)
            {
                if (_gameState.GameBoard[x2][y] == piece)
                {
                    emptyCount = 0;
                    pieceCount++;
                } else if (_gameState.GameBoard[x2][y] == EGamePiece.Empty && coordinates.Count > 0)
                {
                    coordinates.RemoveRange(0, coordinates.Count);
                    coordinates.Add(x2);
                    coordinates.Add(y);
                    emptyCount++;
                } else if (_gameState.GameBoard[x2][y] == EGamePiece.Empty && coordinates.Count == 0)
                {
                    coordinates.Add(x2);
                    coordinates.Add(y);
                    emptyCount++;
                }
                else if (_gameState.GameBoard[x2][y] != piece && coordinates.Count > 0)
                {
                    emptyCount = 0;
                    coordinates.RemoveRange(0, coordinates.Count);
                }

                if (pieceCount == _gameState.GameConfiguration.WinCondition - 1 && coordinates.Count != 0)
                {
                    return coordinates;
                }

                y++;

                if (emptyCount > 1)
                {
                    pieceCount = 0;
                    emptyCount = 0;
                }
            }
        }

        if (_gameState.GameConfiguration.WinCondition < GridSize)
        {
            var counter = GridSize;
            for (var x = GridCoordinates[0] + 1; x < GridCoordinates[0] + GridSize - 1; x++)
            {
                var emptyCount = 0;
                var coordinates = new List<int>();
                var pieceCount = 0;
                var y = GridCoordinates[1] + GridSize - 1;
                counter--;
                for (var i = 0; i < counter; i++)
                {
                    if (_gameState.GameBoard[x + i][y - i] == piece)
                    {
                        emptyCount = 0;
                        pieceCount++;
                    } else if (_gameState.GameBoard[x + i][y - i] == EGamePiece.Empty && coordinates.Count > 0)
                    {
                        coordinates.RemoveRange(0, coordinates.Count);
                        coordinates.Add(x + i);
                        coordinates.Add(y - i);
                        emptyCount++;
                    } else if (_gameState.GameBoard[x + i][y - i] == EGamePiece.Empty && coordinates.Count == 0)
                    {
                        coordinates.Add(x + i);
                        coordinates.Add(y - i);
                        emptyCount++;
                    }
                    else if (_gameState.GameBoard[x + i][y - i] != piece && coordinates.Count > 0)
                    {
                        emptyCount = 0;
                        coordinates.RemoveRange(0, coordinates.Count);
                    }

                    if (pieceCount == _gameState.GameConfiguration.WinCondition - 1 && coordinates.Count != 0)
                    {
                        return coordinates;
                    }

                    if (emptyCount > 1)
                    {
                        pieceCount = 0;
                        emptyCount = 0;
                    }
                }
            }
        }

        return [];
    }
    
    private List<int> CheckAiDiagonally2(EGamePiece piece)
    {
        for (var x = GridCoordinates[0] + GridSize - 1; x >= GridCoordinates[0]; x--)
        {
            var emptyCount =0;
            var coordinates = new List<int>();
            var pieceCount = 0;
            var y = GridCoordinates[1];
            for (var x2 = x; x2 < GridCoordinates[0] + GridSize; x2++)
            {
                if (_gameState.GameBoard[x2][y] == piece)
                {
                    emptyCount = 0;
                    pieceCount++;
                } else if (_gameState.GameBoard[x2][y] == EGamePiece.Empty && coordinates.Count > 0)
                {
                    coordinates.RemoveRange(0, coordinates.Count);
                    coordinates.Add(x2);
                    coordinates.Add(y);
                    emptyCount++;
                } else if (_gameState.GameBoard[x2][y] == EGamePiece.Empty && coordinates.Count == 0)
                {
                    coordinates.Add(x2);
                    coordinates.Add(y);
                    emptyCount++;
                }
                else if (_gameState.GameBoard[x2][y] != piece && coordinates.Count > 0)
                {
                    emptyCount = 0;
                    coordinates.RemoveRange(0, coordinates.Count);
                }

                if (pieceCount == _gameState.GameConfiguration.WinCondition - 1 && coordinates.Count != 0)
                {
                    return coordinates;
                }

                y++;

                if (emptyCount > 1)
                {
                    pieceCount = 0;
                    emptyCount = 0;
                }
            }
        }
        if (_gameState.GameConfiguration.WinCondition < GridSize)
        {
            var counter = GridSize;
            for (var x = GridCoordinates[0] + GridSize - 2; x >= GridCoordinates[0]; x--)
            {
                var emptyCount = 0;
                var coordinates = new List<int>();
                var pieceCount = 0;
                var y = GridCoordinates[1] + GridSize - 1;
                counter--;
                for (var i = 0; i < counter; i++)
                {
                    if (_gameState.GameBoard[x - i][y - i] == piece)
                    {
                        emptyCount = 0;
                        pieceCount++;
                    } else if (_gameState.GameBoard[x - i][y - i] == EGamePiece.Empty && coordinates.Count > 0)
                    {
                        coordinates.RemoveRange(0, coordinates.Count);
                        coordinates.Add(x - i);
                        coordinates.Add(y - i);
                        emptyCount++;
                    } else if (_gameState.GameBoard[x - i][y - i] == EGamePiece.Empty && coordinates.Count == 0)
                    {
                        coordinates.Add(x - i);
                        coordinates.Add(y - i);
                        emptyCount++;
                    }
                    else if (_gameState.GameBoard[x - i][y - i] != piece && coordinates.Count > 0)
                    {
                        emptyCount = 0;
                        coordinates.RemoveRange(0, coordinates.Count);
                    }

                    if (pieceCount == _gameState.GameConfiguration.WinCondition - 1 && coordinates.Count != 0)
                    {
                        return coordinates;
                    }

                    if (emptyCount > 1)
                    {
                        pieceCount = 0;
                        emptyCount = 0;
                    }
                }
            }
        }
        return [];
    }

    public List<int> AiMoveGrid()
    {
        if (GridSize < DimX)
        {
            var r = new Random();
            var directions = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 };

            while (directions.Count > 0)
            {
                var randomIndex = r.Next(0, directions.Count);
                var random = directions[randomIndex];
                directions.RemoveAt(randomIndex);

                switch (random)
                {
                    case 0 when GridCoordinates[0] > 0:
                        return [GridCoordinates[0] - 1, GridCoordinates[1]];
                    case 1 when GridCoordinates[1] > 0:
                        return [GridCoordinates[0], GridCoordinates[1] - 1];
                    case 2 when GridCoordinates[0] + GridSize < DimX:
                        return [GridCoordinates[0] + 1, GridCoordinates[1]];
                    case 3 when GridCoordinates[1] + GridSize < DimY:
                        return [GridCoordinates[0], GridCoordinates[1] + 1];
                    case 4 when GridCoordinates[0] > 0 && GridCoordinates[1] > 0:
                        return [GridCoordinates[0] - 1, GridCoordinates[1] - 1];
                    case 5 when GridCoordinates[0] + GridSize < DimX && GridCoordinates[1] > 0:
                        return [GridCoordinates[0] + 1, GridCoordinates[1] - 1];
                    case 6 when GridCoordinates[0] > 0 && GridCoordinates[1] + GridSize < DimY:
                        return [GridCoordinates[0] - 1, GridCoordinates[1] + 1];
                    case 7 when GridCoordinates[0] + GridSize < DimX && GridCoordinates[1] + GridSize < DimY:
                        return [GridCoordinates[0] + 1, GridCoordinates[1] + 1];
                }
            }
        }
        return [];
    }
    
    public List<int> AiMovePiece(EGamePiece piece)
    {
        if (!CheckPieceInBoard(piece) || IsGridFull())
        {
            return [];
        }

        var outOfGrid = GetPieceOutOfGrid(piece);

        if (outOfGrid.Count > 0)
        {
            List<int> coordinates = [outOfGrid[0], outOfGrid[1]];
            var winCoordinates = AiNewPiece(piece);
            if (winCoordinates.Count > 0)
            {
                coordinates.Add(winCoordinates[0]);
                coordinates.Add(winCoordinates[1]);
                return coordinates;
            }
            do
            {
                var r = new Random();
                var randomX2 = r.Next(GridCoordinates[0], GridCoordinates[0] + GridSize);
                var randomY2 = r.Next(GridCoordinates[1], GridCoordinates[1] + GridSize);
                if (GameBoard[randomX2][randomY2] != EGamePiece.Empty) continue;
                coordinates.Add(randomX2);
                coordinates.Add(randomY2);
                return coordinates;
            } while (true);
        }
        do
        {
            var r = new Random();
            var randomX = r.Next(0, DimX);
            var randomY = r.Next(0, DimY);
            if (GameBoard[randomX][randomY] == piece)
            {
                List<int> coordinates = [randomX, randomY];
                var winCoordinates = AiNewPiece(piece);
                if (winCoordinates.Count > 0)
                {
                    coordinates.Add(winCoordinates[0]);
                    coordinates.Add(winCoordinates[1]);
                    return coordinates;
                }
                do
                {
                    var r2 = new Random();
                    var randomX2 = r2.Next(GridCoordinates[0], GridCoordinates[0] + GridSize);
                    var randomY2 = r2.Next(GridCoordinates[1], GridCoordinates[1] + GridSize);
                    if (GameBoard[randomX2][randomY2] != EGamePiece.Empty) continue;
                    coordinates.Add(randomX2);
                    coordinates.Add(randomY2);
                    return coordinates;
                } while (true);
            }
        } while (true);
    }

    public bool CheckPieceInBoard(EGamePiece piece)
    {
        for (var y = 0; y < DimY; y++)
        {
            for (var x = 0; x < DimX; x++)
            {
                if (GameBoard[x][y] == piece)
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    public List<int> GetPieceOutOfGrid(EGamePiece piece)
    {
        for (var y = 0; y < DimY; y++)
        {
            for (var x = 0; x < DimX; x++)
            {
                if ((x < GridCoordinates[0] || x >= GridCoordinates[0] + GridSize || y < GridCoordinates[1] || 
                     y >= GridCoordinates[1] + GridSize) && GameBoard[x][y] == piece)
                {
                    return [x, y];
                }
            }
        }
        return [];
    }
}
