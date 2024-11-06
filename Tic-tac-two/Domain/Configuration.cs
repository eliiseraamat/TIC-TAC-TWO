using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Configuration
{ 
    public int Id { get; set; }
    
    [MaxLength(128)] 
    public string Name { get; set; } = default!;
    
    [MaxLength(10240)]
    public string GameConfig { get; set; } = default!;
    
    public ICollection<SaveGame>? SaveGames { get; set; }
}