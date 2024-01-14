using Microsoft.AspNetCore.Identity;

namespace AutoresAPI.Entidades; 
public class LlaveAPI {
    public int Id { get; set; }
    public string Llave { get; set; } = string.Empty;
    public TipoLlave TipoLlave { get; set; }
    public bool Activa { get; set; }
    public string? UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public List<RestriccionDominio> RestriccionesDominio { get; set; } = null!;
    public List<RestriccionIP> RestriccionesIP { get; set; } = null!;
}

public enum TipoLlave {
    Gratuita = 1,
    Profesional = 2
}