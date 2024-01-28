using System.ComponentModel.DataAnnotations;

namespace zad8.Models;

public class UpdateDoctorDTO
{
    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
}