using Lab_11.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab_11.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly DatabaseContext _dbContext;

        public PatientController(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatient(int id)
        {
            var patient = await _dbContext.Patients
                .Include(p => p.Prescriptions)
                .ThenInclude(p => p.Doctor)
                .Include(p => p.Prescriptions)
                .ThenInclude(p => p.Medicaments)
                .ThenInclude(p => p.Medicament)
                .FirstOrDefaultAsync(p => p.IdPatient == id);
            
            if (patient == null)
            {
                return NotFound("Patient not found");
            }

            var fullPatientData = new
            {   
                patient.IdPatient,
                patient.FirstName,
                patient.LastName,
                patient.Birthdate,
                Prescriptions = patient.Prescriptions.Select(p => new
                {
                    p.IdPrescription,
                    p.Date,
                    p.DueDate,
                    Doctor = new
                    {
                        p.Doctor.IdDoctor,
                        p.Doctor.FirstName
                    },
                    Medicaments = p.Medicaments.Select(pm => new
                    {
                        pm.IdMedicament,
                        pm.Medicament.Name,
                        pm.Dose,
                        pm.Medicament.Description
                    }).ToList()
                }).ToList()
            };
            
            return Ok(fullPatientData);
        }
    }
}
