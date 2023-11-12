namespace AutoresAPI.Entidades; 
public class Libro {
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public int AutorId { get; set; }
    public Autor Autor { get; set; } = null!;
}