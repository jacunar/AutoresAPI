namespace AutoresAPI.DTOs; 
public class RespuestaAutenticacion {
    public string Token { get; set; } = string.Empty;
    public DateTime Expiracion { get; set; }
}