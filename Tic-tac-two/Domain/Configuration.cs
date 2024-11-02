using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Configuration
{ 
    public int Id { get; set; }  // Primary key
    
    [MaxLength(8192)] // Adjust size as needed
    public string GameConfig { get; set; } = default!; // JSON representation of the configuration
}