namespace zad8.Models;

public class GetPrescriptionDTO
{
    public PatientDTO Patient { get; set; }

    public IEnumerable<MedicamentDTO> Medicaments { get; set; }
    
    public int IdPrescription { get; set; }
    
    public DateTime Date { get; set; }
    
    public DateTime DueDate { get; set; }
    
    public GetDoctorDTO Doctor { get; set; } = null!;
    
}

public class PatientDTO
{
    public int IdPatient { get; set; }
    
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime Birthdate { get; set; }
    
}

public class MedicamentDTO
{
    public int IdMedicament { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string Description { get; set; } = null!;
    
    public string Type { get; set; } = null!;
    
    public int? Dose { get; set; }
    
    public string Details { get; set; } = null!;
    
}