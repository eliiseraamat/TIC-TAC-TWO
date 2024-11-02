using System.ComponentModel.DataAnnotations;

namespace Domain;

public class SaveGame
{
    public int Id { get; set; }  // Primary key

    [MaxLength(128)] public string Name { get; set; } = default!;

    [MaxLength(10240)]
    public string Game { get; set; } = default!; // JSON representation of the game state
}