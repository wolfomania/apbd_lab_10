using Lab_11.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lab_11.Context;

public class DatabaseContext : DbContext
{
    protected DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("lab_11");
        
        modelBuilder.Entity<Doctor>().HasData(new Doctor
        {
            IdDoctor = 1,
            FirstName = "a",
            LastName = "a",
            Email = "a@gmail.com"
        });
        
        modelBuilder.Entity<Medicament>().HasData(new Medicament
        {
            IdMedicament = 1,
            Name = "Medicament1",
            Description = "Description1",
            Type = "Type1"
        });
        
    }
}