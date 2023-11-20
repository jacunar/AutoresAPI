using AutoresAPI.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace AutoresAPI.Entidades; 
public class Libro {
    public int Id { get; set; }
    [Required]
    [PrimeraLetraMayuscula]
    [StringLength(maximumLength: 250)]
    public string Titulo { get; set; } = string.Empty;
    public List<Comentario> Comentarios { get; set; } = null!;
    public List<AutorLibro> AutoresLibros { get; set; } = null!;
}