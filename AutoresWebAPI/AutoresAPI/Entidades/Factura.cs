namespace AutoresAPI.Entidades; 
public class Factura {
    public int Id { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public Usuario Usuario { get; set; } = null!;
    public bool Pagada { get; set; }
    public decimal Monto { get; set; }
    public DateTime FechaEmision { get; set; }
    public DateTime FechaLimiteDePago { get; set; }
}