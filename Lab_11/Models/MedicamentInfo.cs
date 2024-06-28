using System.ComponentModel.DataAnnotations;

namespace Lab_11.Models;

public class MedicamentInfo
{
    public int IdMedicament { get; set; }
        
    [Range(1, 100)]
    public int? Dose { get; set; }
        
    [MaxLength(100)]
    public string Details { get; set; }
}