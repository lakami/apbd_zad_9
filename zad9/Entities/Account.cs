using System.ComponentModel.DataAnnotations;

namespace zad8;

public class Account
{
    [Key]
    public int IdUser { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Login { get; set; } = null!;
    
    [Required]
    [MaxLength(100)]
    public string Password { get; set; } = null!;
    
    [Required]
    public string Salt { get; set; } = null!;
    public string? refreshToken { get; set; }
    public DateTime? refreshTokenExp { get; set; }
}