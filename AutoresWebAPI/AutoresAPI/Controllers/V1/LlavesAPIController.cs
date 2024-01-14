using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AutoresAPI.Controllers.V1;
[Route("api/v1/llavesapi")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class LlavesAPIController : CustomBaseController {
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;
    private readonly ServicioLlaves servicioLlaves;

    public LlavesAPIController(ApplicationDbContext context, IMapper mapper, ServicioLlaves servicioLlaves) {
        this.context = context;
        this.mapper = mapper;
        this.servicioLlaves = servicioLlaves;
    }

    [HttpGet]
    public async Task<List<LlaveDTO>> MisLlaves() {
        var usuarioId = ObtenerUsuarioId();
        var llaves = await context.LlavesAPI.Include(x => x.RestriccionesDominio)
            .Include(x => x.RestriccionesIP)
            .Where(x => x.UsuarioId == usuarioId).ToListAsync();
        return mapper.Map<List<LlaveDTO>>(llaves);
    }

    [HttpPost]
    public async Task<ActionResult> CrearLlave(CrearLlaveDTO crearLlaveDTO) {
        var usuarioId = ObtenerUsuarioId();
        if (crearLlaveDTO.TipoLlave == TipoLlave.Gratuita) {
            var usuarioTieneLlaveGratuita = await context.LlavesAPI.AnyAsync(
                    x => x.UsuarioId == usuarioId && x.TipoLlave == TipoLlave.Gratuita);
            if (usuarioTieneLlaveGratuita)
                return BadRequest("El usuario ya tiene una llave gratuita");
        }

        await servicioLlaves.CrearLLave(usuarioId, crearLlaveDTO.TipoLlave);
        return NoContent();
    }

    [HttpPut]
    public async Task<ActionResult> ActualizarLlave(ActualizarLlaveDTO actualizarLlaveDTO) {
        var usuarioId = ObtenerUsuarioId();
        var llaveDb = await context.LlavesAPI.FirstOrDefaultAsync(x => x.Id == actualizarLlaveDTO.LlaveId);
        if (llaveDb == null)
            return NotFound();

        if (usuarioId != llaveDb.UsuarioId)
            return Forbid();

        if (actualizarLlaveDTO.ActualizarLlave)
            llaveDb.Llave = servicioLlaves.GenerarLlave();

        llaveDb.Activa = actualizarLlaveDTO.Activa;
        await context.SaveChangesAsync();
        return NoContent();
    }
}