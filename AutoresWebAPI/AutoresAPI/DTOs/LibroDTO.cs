namespace AutoresAPI.DTOs; 
public class LibroDTO {
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    //public List<ComentarioDTO> Comentarios { get; set; } = null!;
    public List<AutorDTO> Autores { get; set; } = null!;
}