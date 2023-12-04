using System.ComponentModel.DataAnnotations;

namespace AutoresAPI.DTOs; 
public class EditarAdminDTO {
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}