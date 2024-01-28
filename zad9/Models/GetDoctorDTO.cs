namespace zad8.Models;

public class GetDoctorDTO
{
    
    public int IdDoctor { get; set; }
    
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;
    
    
}