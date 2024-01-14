using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AutoresAPI.Controllers.V2;
[ApiController]
[Route("api/autores")]
[HeaderIsPresent("x-version", "2")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class AutoresController : ControllerBase {
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;

    public AutoresController(ApplicationDbContext context, IMapper mapper) {
        this.context = context;
        this.mapper = mapper;
    }

    [HttpGet(Name = "obtenerAutoresv2")]
    [AllowAnonymous]
    [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
    public async Task<ActionResult<List<AutorDTO>>> Get() {
        var autores = await context.Autores.ToListAsync();
        autores.ForEach(autor => autor.Nombre = autor.Nombre.ToUpper());
        return mapper.Map<List<AutorDTO>>(autores);
    }

    [HttpGet("{id:int}", Name = "obtenerAutorv2")]
    [AllowAnonymous]
    [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
    public async Task<ActionResult<AutorDTOConLibros>> Get(int id) {
        var autor = await context.Autores
            .Include(a => a.AutoresLibros).ThenInclude(l => l.Libro)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (autor is null)
            return NotFound();

        var dto = mapper.Map<AutorDTOConLibros>(autor);
        return dto;
    }
    
    [HttpGet("{nombre}", Name = "obtenerAutorPorNombrev2")]
    public async Task<ActionResult<List<AutorDTO>>> GetPorNombre([FromRoute] string nombre) {
        var autores = await context.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync();

        return mapper.Map<List<AutorDTO>>(autores);
    }

    [HttpPost(Name = "crearAutorv2")]
    public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO) {
        var existeAutorConElMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre);
        
        if (existeAutorConElMismoNombre)
            return BadRequest($"Ya existe un autor con el nombre {autorCreacionDTO.Nombre}");

        var autor = mapper.Map<Autor>(autorCreacionDTO);

        context.Add(autor);
        await context.SaveChangesAsync();

        var autorDTO = mapper.Map<AutorDTO>(autor);
        return CreatedAtRoute("obtenerAutorv2", new { id = autor.Id }, autorDTO);
    }

    [HttpPut("{id:int}", Name = "actualizarAutorv2")]
    public async Task<ActionResult> Put(AutorCreacionDTO autorCreacionDTO, int id) {
        var existe = await context.Autores.AnyAsync(x => x.Id == id);
        if (!existe)
            return NotFound();

        var autor = mapper.Map<Autor>(autorCreacionDTO);
        autor.Id = id;

        context.Update(autor);
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}", Name = "borrarAutorv2")]
    [Authorize(Policy = "EsAdmin")]
    public async Task<ActionResult> Delete(int id) {
        var existe = await context.Autores.AnyAsync(x => x.Id == id);
        if (!existe)
            return NotFound();

        context.Remove(new Autor() { Id = id });
        await context.SaveChangesAsync();
        return NoContent();
    }
}

//https://lecturasdepapelazulcom.wordpress.com/2020/05/10/libros-escritos-por-varios-escritores/