using System.Transactions;
using Lab_10.Context;
using Lab_10.Models;
using Lab_10.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab_10.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private readonly DatabaseContext _dbContext;
        
        public PrescriptionController(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        [HttpPost]
        public async Task<IActionResult> AddPrescription([FromBody] AddPrescriptionRequest request)
        {
            var patient = await _dbContext.Patients
                .FirstOrDefaultAsync(p => p.IdPatient == request.Patient.IdPatient);

            
            if (patient == null)
            {
                patient = new Patient
                {
                    FirstName = request.Patient.FirstName,
                    LastName = request.Patient.LastName,
                    Birthdate = request.Patient.Birthdate
                };

                _dbContext.Patients.Add(patient);
            }
            
            var doctor = await _dbContext.Doctors
                .FirstOrDefaultAsync(d => d.IdDoctor == request.Doctor.IdDoctor);
            
            if (doctor == null)
            {
                return BadRequest("Doctor with id " + request.Doctor.IdDoctor + " does not exist");
            }

            switch (request.Medicaments.Count)
            {
                case 0:
                    return BadRequest("Prescription must contain at least one medicament");
                case > 10:
                    return BadRequest("Prescription can contain at most 10 medicaments");
            }
            
            if (request.Date > request.DueDate)
            {
                return BadRequest("Due date must be later than date or equal to date");
            }

            var prescription = new Prescription
            {
                Date = request.Date,
                DueDate = request.DueDate,
                IdPatient = patient.IdPatient,
                Patient = patient,
                IdDoctor = doctor.IdDoctor,
                Doctor = doctor
            };
            
            _dbContext.Prescriptions.Add(prescription);
            
            foreach (var medicamentInfo in request.Medicaments)
            {
                var medicament = await _dbContext.Medicaments
                    .FirstOrDefaultAsync(m => m.IdMedicament == medicamentInfo.IdMedicament);

                if (medicament == null)
                {
                    return BadRequest("Medicament with id " + medicamentInfo.IdMedicament + " does not exist");
                }
                
                var prescriptionMedicament = new PrescriptionMedicament
                {
                    IdMedicament = medicament.IdMedicament,
                    Medicament = medicament,
                    IdPrescription = prescription.IdPrescription,
                    Prescription = prescription,
                    Dose = medicamentInfo.Dose,
                    Details = medicamentInfo.Details
                };
                
                _dbContext.PrescriptionMedicaments.Add(prescriptionMedicament);
            }
            
            await _dbContext.SaveChangesAsync();
            
            return Ok();
        }
    }
}
