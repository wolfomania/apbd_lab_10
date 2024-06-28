namespace Lab_11.Models;

public class AddPrescriptionRequest
{
    public PatientInfo Patient { get; set; }

    public ICollection<MedicamentInfo> Medicaments { get; set; }

    public DoctorInfo Doctor { get; set; }

    public DateTime Date { get; set; }

    public DateTime DueDate { get; set; }
}
