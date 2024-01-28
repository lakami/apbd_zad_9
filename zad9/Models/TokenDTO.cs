using System.ComponentModel.DataAnnotations;

namespace zad8.Models;

public class TokenDTO
{
    [Required]
    public string Token { get; set; } = null!;
    [Required]
    public string RefreshToken { get; set; } = null!;
}