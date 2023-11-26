using AutoresAPI.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace AutoresAPI.DTOs; 
public class LibroPatchDTO {
    [PrimeraLetraMayuscula]
    [StringLength(maximumLength: 250)]
    [Required]
    public string Titulo { get; set; } = string.Empty;
    public DateTime FechaPublicacion { get; set; }
}