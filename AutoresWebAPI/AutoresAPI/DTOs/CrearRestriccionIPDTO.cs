using System.ComponentModel.DataAnnotations;

namespace AutoresAPI.DTOs;
public class RestriccionIPDTO {
    public int Id { get; set; }
    public string IP { get; set; } = string.Empty;
}

public class CrearRestriccionIPDTO {
    public int LlaveId { get; set; }
    [Required]
    public string IP { get; set; } = string.Empty;
}

public class ActualizarRestriccionDTO {
    [Required]
    public string IP { get; set; } = string.Empty;
}