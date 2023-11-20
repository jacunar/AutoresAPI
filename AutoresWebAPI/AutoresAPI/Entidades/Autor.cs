using AutoresAPI.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace AutoresAPI.Entidades; 
public class Autor {
    public int Id { get; set; }
    [Required(ErrorMessage = "El campo {0} es requerido")]
    [StringLength(maximumLength: 120, ErrorMessage = "El campo {0} no debe tener más de {1} carácteres")]
    [PrimeraLetraMayuscula]
    public string Nombre { get; set; } = string.Empty;
    public List<AutorLibro> AutoresLibros { get; set; } = null!;
}