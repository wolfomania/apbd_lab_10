using System.ComponentModel.DataAnnotations;

namespace Lab_10.Models.Domain;

public class Medicament
{
    
    [Key]
    public int IdMedicament { get; set; }
    
    [MaxLength(100)]
    public string Name { get; set; } = null!;
    
    [MaxLength(100)]
    public string Description { get; set; } = null!;
    
    [MaxLength(100)]
    public string Type { get; set; } = null!;
    
    public ICollection<PrescriptionMedicament> Prescriptions { get; set; } = null!;
}