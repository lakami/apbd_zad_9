using System.ComponentModel.DataAnnotations;

namespace zad8.Models;

public class AddUserDTO
{
    
    [Required]
    [MaxLength(100)]
    public string Login { get; set; } = null!;
    
    [Required]
    [MaxLength(100)]
    public string Password { get; set; } = null!;
    
}