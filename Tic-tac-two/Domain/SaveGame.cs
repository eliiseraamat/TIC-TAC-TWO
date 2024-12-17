using System.ComponentModel.DataAnnotations;

namespace Domain;

public class SaveGame
{
    public int Id { get; init; }

    [MaxLength(128)] 
    public string Name { get; init; } = default!;

    [MaxLength(10240)]
    public string Game { get; set; } = default!;
    
    [MaxLength(128)]
    public string PasswordX { get; init; } = default!;
    
    [MaxLength(128)]
    public string PasswordO { get; init; } = default!;
    
    public int ConfigurationId { get; init; }
    public Configuration? Configuration { get; init; }
}