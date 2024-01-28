
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using zad8.Models;
using zad8.Repo;

namespace zad8.Controllers;

[ApiController]
[Route("[controller]")]
public class DoctorsController : ControllerBase
{
   
    private readonly ILogger<DoctorsController> _logger;
    private readonly DatabaseContext _context;

    public DoctorsController(ILogger<DoctorsController> logger, DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public async Task<IEnumerable<GetDoctorDTO>> getDocotor()
    {
        return await _context.Doctor
            .Select(d => new GetDoctorDTO
            {
                IdDoctor = d.IdDoctor,
                FirstName = d.FirstName,
                LastName = d.LastName,
                Email = d.Email
            })
            .ToListAsync();
        
    }
    
    [HttpPost]
    public async Task<IActionResult> AddDoctor(AddDoctorDTO addDoctorDTO)
    {
        var doctor = new Doctor
        {
            FirstName = addDoctorDTO.FirstName,
            LastName = addDoctorDTO.LastName,
            Email = addDoctorDTO.Email
        };
        await _context.Doctor.AddAsync(doctor);
        await _context.SaveChangesAsync();
        return Ok("Dodano lekarza");
    }
    
    [HttpPut("{idDoctor}")] 
    public async Task<IActionResult> UpdateDoctor(int idDoctor, AddDoctorDTO addDoctorDTO)
    {
        var doctor = await _context.Doctor.FindAsync(idDoctor);
        if (doctor == null)
        {
            return NotFound("Nie znaleziono lekarza");
        }
        doctor.FirstName = addDoctorDTO.FirstName;
        doctor.LastName = addDoctorDTO.LastName;
        doctor.Email = addDoctorDTO.Email;
        await _context.SaveChangesAsync();
        return Ok("Zaktualizowano dane lekarza");
    }
    
    [HttpDelete("{idDoctor}")]
    public async Task<IActionResult> DeleteDoctor(int idDoctor)
    {
        var doctor = await _context.Doctor.FindAsync(idDoctor);
        if (doctor == null)
        {
            return NotFound("Nie znaleziono lekarza");
        }
        _context.Doctor.Remove(doctor);
        await _context.SaveChangesAsync();
        return Ok("Usunięto lekarza");
    }
    
}