using System.ComponentModel.DataAnnotations;

namespace Lab_11.Models.Domain;

public class AppUser
{
    [Key]
    public int IdUser { get; set; }

    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Salt { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime? RefreshTokenExp { get; set; }
}