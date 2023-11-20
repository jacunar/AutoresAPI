using AutoresAPI.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace AutoresAPI.DTOs; 
public class LibroCreacionDTO {
    [PrimeraLetraMayuscula]
    [StringLength(maximumLength: 250)]
    public string Titulo { get; set; } = string.Empty;
}