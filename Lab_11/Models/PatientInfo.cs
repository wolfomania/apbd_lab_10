using System.ComponentModel.DataAnnotations;

namespace Lab_11.Models;

public class PatientInfo
{
    [Range(1, int.MaxValue)]
    public int IdPatient { get; set; }
    
    [MaxLength(100)]
    public string FirstName { get; set; }
    
    [MaxLength(100)]
    public string LastName { get; set; }
    
    public DateTime Birthdate { get; set; }
}