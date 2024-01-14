namespace AutoresAPI.Entidades; 
public class RestriccionIP {
    public int Id { get; set; }
    public int LlaveId { get; set; }
    public string IP { get; set; } = string.Empty;
    public LlaveAPI Llave { get; set; } = null!;
}