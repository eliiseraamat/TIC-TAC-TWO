﻿namespace GameBrain;

public record GameConfiguration
{
    public string Name { get; init; } = default!;
    
    public int BoardSize { get; set; } = 5;

    public int GridSize { get; set; } = 3;
    
    public int Pieces { get; set; } = 4;

    public int WinCondition { get; set; } = 3;
    
    public int MovePieceAfterMoves { get; set; } = 2;

    public EGamePiece StartingPiece { get; init; } = EGamePiece.X;
    
    public List<int> GridCoordinates { get; set; } = [1, 1];

    public string PlayerX { get; set; } = "X";
    
    public string PlayerO { get; set; } = "O";

    public EGameType GameType { get; set; } = EGameType.TwoPlayer;
    
    public override string ToString() => $"Board {BoardSize}x{BoardSize}, grid {GridSize}x{GridSize}, to win: {WinCondition}, can move piece after {MovePieceAfterMoves} moves";
}