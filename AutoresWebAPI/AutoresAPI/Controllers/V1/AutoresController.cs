using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace AutoresAPI.Controllers.V1;
[ApiController]
//[Route("api/v1/autores")]
[Route("api/autores")]
[HeaderIsPresent("x-version", "1")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class AutoresController : ControllerBase {
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;

    public AutoresController(ApplicationDbContext context, IMapper mapper) {
        this.context = context;
        this.mapper = mapper;
    }

    [HttpGet(Name = "obtenerAutoresv1")]
    [AllowAnonymous]
    [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
    public async Task<ActionResult<List<AutorDTO>>> Get([FromQuery] PaginationDTO paginationDTO) {
        var queryable = context.Autores.AsQueryable();
        await HttpContext.InsertPaginationParameterInHeader(queryable);

        var autores = await queryable.OrderBy(a => a.Nombre).Page(paginationDTO).ToListAsync();
        return mapper.Map<List<AutorDTO>>(autores);
    }

    [HttpGet("{id:int}", Name = "obtenerAutorv1")]
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
    
    [HttpGet("{nombre}", Name = "obtenerAutorPorNombrev1")]
    public async Task<ActionResult<List<AutorDTO>>> GetPorNombre([FromRoute] string nombre) {
        var autores = await context.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync();

        return mapper.Map<List<AutorDTO>>(autores);
    }

    [HttpPost(Name = "crearAutorv1")]
    public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO) {
        var existeAutorConElMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre);
        
        if (existeAutorConElMismoNombre)
            return BadRequest($"Ya existe un autor con el nombre {autorCreacionDTO.Nombre}");

        var autor = mapper.Map<Autor>(autorCreacionDTO);

        context.Add(autor);
        await context.SaveChangesAsync();

        var autorDTO = mapper.Map<AutorDTO>(autor);
        return CreatedAtRoute("obtenerAutorv1", new { id = autor.Id }, autorDTO);
    }

    [HttpPut("{id:int}", Name = "actualizarAutorv1")]
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

    /// <summary>
    /// Delete an author
    /// </summary>
    /// <param name="id">Author Id to delete</param>
    /// <returns></returns>
    [HttpDelete("{id:int}", Name = "borrarAutorv1")]
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