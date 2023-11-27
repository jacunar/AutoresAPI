using System.ComponentModel.DataAnnotations;

namespace AutoresAPI.DTOs; 
public class CredencialUsuario {
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}