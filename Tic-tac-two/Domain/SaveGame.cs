using System.ComponentModel.DataAnnotations;

namespace Domain;

public class SaveGame
{
    public int Id { get; set; }

    [MaxLength(128)] 
    public string Name { get; set; } = default!;

    [MaxLength(10240)]
    public string Game { get; set; } = default!;
    
    public int ConfigurationId { get; set; }
    public Configuration? Configuration { get; set; }
}