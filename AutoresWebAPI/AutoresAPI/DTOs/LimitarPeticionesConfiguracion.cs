namespace AutoresAPI.DTOs; 
public class LimitarPeticionesConfiguracion {
    public int PeticionesPorDiaGratuito { get; set; }
    public string[]? ListaBlancaRutas { get; set; }
}