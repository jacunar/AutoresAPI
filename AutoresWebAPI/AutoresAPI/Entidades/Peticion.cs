namespace AutoresAPI.Entidades; 
public class Peticion {
    public int Id { get; set; }
    public int LlaveAPIId { get; set; }
    public DateTime FechaPeticion { get; set; }
    public LlaveAPI LlaveAPI { get; set; } = null!;
}