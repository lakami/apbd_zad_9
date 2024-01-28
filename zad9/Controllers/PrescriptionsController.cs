using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using zad8.Models;
using zad8.Repo;

namespace zad8.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class PrescriptionsController :ControllerBase
{
    private readonly ILogger<PrescriptionsController> _logger;
    private readonly DatabaseContext _context;
    
    public PrescriptionsController(ILogger<PrescriptionsController> logger, DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }
    
    [HttpGet("{idPrescription}")]
    public async Task<IActionResult> getPrescription(int idPrescription)
    {
        var prescription = await _context.Prescription
            .Where(p => p.IdPrescription == idPrescription)
            //join tabeli Prescription z tabelą Doctor
            .Include(p => p.Doctor)
            //join tabeli Prescription z tabelą Patient
            .Include(p => p.Patient)
            //join tabeli Prescription z tabelą Prescription_Medicament
            .Include(p => p.Prescription_Medicaments)
            
            //join do tabeli Medicament za pomocą tabeli pośredniczącej Prescription_Medicament
            .ThenInclude(pm => pm.Medicament)
            .Select(p => new GetPrescriptionDTO
            {
                IdPrescription = p.IdPrescription,
                Date = p.Date,
                DueDate = p.DueDate,
                Patient = new PatientDTO()
                {
                    Birthdate = p.Patient.Birthdate,
                    FirstName = p.Patient.FirstName,
                    LastName = p.Patient.LastName,
                    IdPatient = p.Patient.IdPatient
                },
                
                Doctor = new GetDoctorDTO()
                {
                    FirstName = p.Doctor.FirstName,
                    LastName = p.Doctor.LastName,
                    Email = p.Doctor.Email,
                    IdDoctor = p.Doctor.IdDoctor
                },
                
                Medicaments = p.Prescription_Medicaments.Select(pm => new MedicamentDTO
                {
                    IdMedicament = pm.IdMedicament,
                    Name = pm.Medicament.Name,
                    Description = pm.Medicament.Description,
                    Type = pm.Medicament.Type
                })
            })
            .FirstOrDefaultAsync();

        if (prescription == null)
        {
            return NotFound("Nie znaleziono recepty");
        }

        return Ok(prescription);
    }
    
}