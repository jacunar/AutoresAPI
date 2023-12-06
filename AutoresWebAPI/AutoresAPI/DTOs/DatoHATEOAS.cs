namespace AutoresAPI.DTOs; 
public class DatoHATEOAS {
    public string Enlace { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public string Metodo { get; private set; } = string.Empty;

    public DatoHATEOAS(string enlace, string descripcion, string metodo) {
        Enlace = enlace;
        Descripcion = descripcion;
        Metodo = metodo;
    }
}