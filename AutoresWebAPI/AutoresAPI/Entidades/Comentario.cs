using Microsoft.AspNetCore.Identity;

namespace AutoresAPI.Entidades; 
public class Comentario {
    public int Id { get; set; }
    public string Contenido { get; set; } = string.Empty;
    public int LibroId { get; set; }
    public Libro Libro { get; set; } = null!;
    public string UsuarioId { get; set; } = string.Empty;
    public IdentityUser Usuario { get; set; } = null!;
}