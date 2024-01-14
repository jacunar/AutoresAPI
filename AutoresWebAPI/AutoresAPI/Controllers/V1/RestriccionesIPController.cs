namespace AutoresAPI.Controllers.V1;

[Route("api/v1/restriccionesip")]
[ApiController]
public class RestriccionesIPController : CustomBaseController {
    private readonly ApplicationDbContext context;

    public RestriccionesIPController(ApplicationDbContext context) {
        this.context = context;
    }

    [HttpPost]
    public async Task<ActionResult> Post(CrearRestriccionIPDTO crearRestriccion) {
        var llaveDB = await context.LlavesAPI.FirstOrDefaultAsync(x => x.Id == crearRestriccion.LlaveId);

        if (llaveDB == null) {
            return NotFound();
        }

        var usuarioId = ObtenerUsuarioId();

        if (llaveDB.UsuarioId != usuarioId) {
            return Forbid();
        }

        var restriccionIP = new RestriccionIP {
            LlaveId = llaveDB.Id,
            IP = crearRestriccion.IP
        };

        context.Add(restriccionIP);

        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, ActualizarRestriccionDTO actualizarRestriccion) {
        var restriccionDB = await context.RestriccionesIP.Include(x => x.Llave)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (restriccionDB == null) return NotFound();

        var usuarioId = ObtenerUsuarioId();
        if (restriccionDB.Llave.UsuarioId != usuarioId) return Forbid();

        restriccionDB.IP = actualizarRestriccion.IP;
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id) {
        var restriccionDB = await context.RestriccionesIP.Include(x => x.Llave).FirstOrDefaultAsync(x => x.Id == id);
        if (restriccionDB == null) return NotFound();

        var usuarioId = ObtenerUsuarioId();
        if (restriccionDB.Llave.UsuarioId != usuarioId) return Forbid();

        context.Remove(restriccionDB);
        await context.SaveChangesAsync();
        return NoContent();
    }
}