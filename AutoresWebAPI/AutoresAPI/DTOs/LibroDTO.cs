namespace AutoresAPI.DTOs; 
public class LibroDTO {
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public DateTime FechaPublicacion { get; set; }
}