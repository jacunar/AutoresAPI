namespace AutoresAPI.Controllers.V1;
[Route("api/facturas")]
[ApiController]
public class FacturasController : ControllerBase {
    private readonly ApplicationDbContext context;

    public FacturasController(ApplicationDbContext context) {
        this.context = context;
    }

    [HttpPost]
    public async Task<ActionResult> Pagar(PagarFacturaDTO pagarFacturaDTO) {
        var facturaDB = await context.Facturas.Include(x => x.Usuario)
                .FirstOrDefaultAsync(x => x.Id == pagarFacturaDTO.FacturaId);

        if (facturaDB is null) return NotFound();
        if (facturaDB.Pagada) return BadRequest("La factura ya fue saldada");

        //Logica de pago de la factura
        facturaDB.Pagada = true;
        await context.SaveChangesAsync();

        var facturasPendientesVencidas = 
            await context.Facturas.AnyAsync(x => x.UsuarioId == facturaDB.UsuarioId
            && !x.Pagada && x.FechaLimiteDePago < DateTime.Today);

        if (!facturasPendientesVencidas) {
            facturaDB.Usuario.MalaPaga = false;
            await context.SaveChangesAsync();
        }
        return NoContent();
    }
}