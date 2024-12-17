using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Configuration
{ 
    public int Id { get; init; }
    
    [MaxLength(128)] 
    public string Name { get; init; } = default!;
    
    [MaxLength(10240)]
    public string GameConfig { get; init; } = default!;
    
    public ICollection<SaveGame>? SaveGames { get; init; }
}