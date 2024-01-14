using System.ComponentModel.DataAnnotations;

namespace AutoresAPI.DTOs; 
public class RestriccionDominioDTO {
    public int Id { get; set; }
    public string Dominio { get; set; } = string.Empty;
}

public class CrearRestriccionesDominioDTO { 
    public int LlaveId { get; set; }
    [Required]
    public string Dominio { get; set; } = string.Empty;
}

public class ActualizarRestriccionDominioDTO {
    [Required]
    public string Dominio { get; set; } = string.Empty;
}