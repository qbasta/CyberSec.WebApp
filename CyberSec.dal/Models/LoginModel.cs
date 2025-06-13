using System.ComponentModel.DataAnnotations;

namespace CyberSec.dal.Models;

public class LoginModel
{
    [Required(ErrorMessage = "Login jest wymagany")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Login musi mieć od 3 do 50 znaków")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Hasło jest wymagane")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Hasło musi mieć od 3 do 100 znaków")]
    public string Password { get; set; } = string.Empty;
}