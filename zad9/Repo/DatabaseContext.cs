using Microsoft.EntityFrameworkCore;

namespace zad8.Repo;

public class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }
    
    //umożliwienie pracy na tablicy Doctors
    public virtual DbSet<Doctor> Doctor { get; set; }
    
    //umożliwienie pracy na tablicy Patient
    public virtual DbSet<Patient> Patient { get; set; }
    
    //umożliwienie pracy na tablicy Medicament
    public virtual DbSet<Medicament> Medicament { get; set; }
    
    //umożliwienie pracy na tablicy Prescription
    public virtual DbSet<Prescription> Prescription { get; set; }
    
    //umożliwienie pracy na tablicy Prescription_Medicament
    public virtual DbSet<Prescription_Medicament> Prescription_Medicament { get; set; }
    
    //umożliwienie pracy na tablicy Account
    public virtual DbSet<Account> Account { get; set; }
}