namespace AutoresAPI.DTOs; 
public class LlaveDTO {
    public int Id { get; set; }
    public string Llave { get; set; } = string.Empty;
    public bool Activa { get; set; }
    public string TipoLlave { get; set; } = string.Empty;
    public List<RestriccionDominioDTO> RestriccionesDominio { get; set; } = null!;
    public List<RestriccionIP> RestriccionesIP { get; set; } = null!;
}

public class CrearLlaveDTO {
    public TipoLlave TipoLlave { get; set; }
}

public class ActualizarLlaveDTO {
    public int LlaveId { get; set; }
    public bool ActualizarLlave { get; set; }
    public bool Activa { get; set; }
}