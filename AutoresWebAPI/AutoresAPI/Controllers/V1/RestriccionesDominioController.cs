using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AutoresAPI.Controllers.V1;
[Route("api/v1/restriccionesdominio")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class RestriccionesDominioController : CustomBaseController {
    private readonly ApplicationDbContext context;

    public RestriccionesDominioController(ApplicationDbContext context) {
        this.context = context;
    }

    [HttpPost]
    public async Task<ActionResult> Post(CrearRestriccionesDominioDTO crearRestriccionesDominioDTO) {
        var llaveDB = await context.LlavesAPI.FirstOrDefaultAsync(x => x.Id == crearRestriccionesDominioDTO.LlaveId);

        if (llaveDB == null) return NotFound();

        var usuarioId = ObtenerUsuarioId();
        if (llaveDB.UsuarioId != usuarioId) return Forbid();

        var restriccionDominio = new RestriccionDominio() {
            LlaveId = crearRestriccionesDominioDTO.LlaveId,
            Dominio = crearRestriccionesDominioDTO.Dominio
        };
        context.Add(restriccionDominio);
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, ActualizarRestriccionDominioDTO actualizarRestriccionDominioDTO) {
        var restriccionDB = await context.RestriccionesDominio.Include(x => x.Llave)
                .FirstOrDefaultAsync(x => x.Id == id);
        if (restriccionDB == null) return NotFound();

        var usuarioId = ObtenerUsuarioId();
        if (restriccionDB.Llave.UsuarioId != usuarioId) return Forbid();

        restriccionDB.Dominio = actualizarRestriccionDominioDTO.Dominio;
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id) {
        var restriccionDB = await context.RestriccionesDominio.Include(x => x.Llave)
                .FirstOrDefaultAsync(x => x.Id == id);
        if (restriccionDB == null) return NotFound();

        var usuarioId = ObtenerUsuarioId();
        if (restriccionDB.Llave.UsuarioId != usuarioId) return Forbid();

        context.Remove(restriccionDB);
        await context.SaveChangesAsync();
        return NoContent();
    }
}