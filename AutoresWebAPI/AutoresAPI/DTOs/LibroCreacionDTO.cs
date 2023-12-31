﻿using AutoresAPI.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace AutoresAPI.DTOs; 
public class LibroCreacionDTO {
    [PrimeraLetraMayuscula]
    [StringLength(maximumLength: 250)]
    [Required]
    public string Titulo { get; set; } = string.Empty;
    public DateTime FechaPublicacion { get; set; }
    public List<int> AutoresIds { get; set; } = null!;
}