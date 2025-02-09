﻿using System.Text.Json;
using GameBrain;

namespace DAL;

public class GameRepositoryJson : IGameRepository
{
    public string SaveGame(GameState state, string gameConfigName, EGamePiece piece)
    {
        var random = new Random();
        var max = (int)Math.Pow(10, 6) - 1;
        string passwordX;
        string passwordO;
        switch (piece)
        {
            case EGamePiece.Empty:
                passwordX = random.Next(0, max + 1).ToString($"D{6}");
                passwordO = random.Next(0, max + 1).ToString($"D{6}");
                break;
            case EGamePiece.X:
                passwordX = random.Next(0, max + 1).ToString($"D{6}");
                passwordO = "-";
                break;
            case EGamePiece.O:
            default:
                passwordX = "-";
                passwordO = random.Next(0, max + 1).ToString($"D{6}");
                break;
        }
        
        var gameName = gameConfigName + " " + DateTime.Now.ToString("yy-MM-dd_HH-mm-ss");
        var fileName = FileHelper.BasePath + gameName + FileHelper.GameExtension;
        var gameData = new GameData(passwordX, passwordO, state);
        File.WriteAllText(fileName, gameData.ToString());
        return gameName;
    }

    public GameState? LoadGame(string fileName)
    {
        var fullPath = FileHelper.BasePath + fileName + FileHelper.GameExtension;

        if (!File.Exists(fullPath))
        {
            return null;
        }
        try
        {
            var gameDataJson = File.ReadAllText(fullPath);
            var gameData = JsonSerializer.Deserialize<GameData>(gameDataJson);
            return gameData?.GameState;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public List<string> GetGameNames()
    {
        return Directory
            .GetFiles(FileHelper.BasePath, FileHelper.SearchPattern + FileHelper.GameExtension)
            .Select(fileNameParts => Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(fileNameParts)))
            .ToList();
    }

    public string UpdateGame(string jsonStateString, string gameName)
    {
        var fileName = FileHelper.BasePath + gameName + FileHelper.GameExtension;

        if (!File.Exists(fileName))
        {
            return "Error";
        }
        
        var gameDataJson = File.ReadAllText(fileName);
        var gameData = JsonSerializer.Deserialize<GameData>(gameDataJson);

        if (gameData == null)
        {
            return "Error";
        }

        gameData.GameState = JsonSerializer.Deserialize<GameState>(jsonStateString) ?? new GameState();

        File.WriteAllText(fileName, gameData.ToString());
        
        return gameName;
    }
    
    public List<string> GetPasswords(string gameName)
    {
        var name = Directory
            .GetFiles(FileHelper.BasePath, FileHelper.SearchPattern + FileHelper.GameExtension)
            .FirstOrDefault(fileNameParts => Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(fileNameParts)) == gameName);
        if (name == null)
        {
            return [];
        }
        var gameDataJson = File.ReadAllText(name);
        var gameData = JsonSerializer.Deserialize<GameData>(gameDataJson);
        if (gameData == null)
        {
            throw new Exception("Failed to load the game");
        }
        return [gameData.PasswordX, gameData.PasswordO];
    }

    public void DeleteGame(string gameName)
    {
        var fileName = FileHelper.BasePath + gameName + FileHelper.GameExtension;
        File.Delete(fileName);
    }
}