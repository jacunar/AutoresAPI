using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace AutoresAPI.Controllers.V1;
[ApiController]
[Route("api/v1/libros")]
public class LibrosController: ControllerBase {
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;

    public LibrosController(ApplicationDbContext context, IMapper mapper) {
        this.context = context;
        this.mapper = mapper;
    }

    [HttpGet(Name = "obtenerLibros")]
    public async Task<List<LibroDTO>> Get() {
        var libros = await context.Libros.ToListAsync();
        return mapper.Map<List<LibroDTO>>(libros);
    }

    [HttpGet("{id:int}", Name = "obtenerLibro")]
    public async Task<ActionResult<LibroDTOConAutores>> Get(int id) {
        var libro = await context.Libros
                    .Include(x => x.AutoresLibros)
                    .ThenInclude(x => x.Autor)
                    .FirstOrDefaultAsync(l => l.Id == id);
        
        return mapper.Map<LibroDTOConAutores>(libro);
    }

    [HttpPost(Name = "crearLibro")]
    public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO) {
        if (libroCreacionDTO.AutoresIds is null)
            return BadRequest("No se puede crear un libro sin autores");

        var autoresIds = await context.Autores
                            .Where(x => libroCreacionDTO.AutoresIds.Contains(x.Id))
                            .Select(x => x.Id).ToListAsync();
        
        if (libroCreacionDTO.AutoresIds.Count != autoresIds.Count)
            return BadRequest("No existe uno de los autores enviados.");
        
        var libro = mapper.Map<Libro>(libroCreacionDTO);
        AsignarOrdenAutores(libro);

        context.Add(libro);
        await context.SaveChangesAsync();

        var libroDTO = mapper.Map<LibroDTO>(libro);
        return CreatedAtRoute("obtenerLibro", new { id = libro.Id }, libroDTO);
    }

    private void AsignarOrdenAutores(Libro libro) {
        if (libro.AutoresLibros != null) {
            for (int i = 0; i < libro.AutoresLibros.Count; i++) {
                libro.AutoresLibros[i].Orden = i;
            }
        }
    }

    [HttpPut("{id:int}", Name = "actualizarLibro")]
    public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO) {
        var libroDb = await context.Libros
                        .Include(x => x.AutoresLibros)
                        .FirstOrDefaultAsync(x => x.Id == id);

        if (libroDb is null) return NotFound();

        libroDb = mapper.Map(libroCreacionDTO, libroDb);
        AsignarOrdenAutores(libroDb);

        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id:int}", Name = "patchLibro")]
    public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPatchDTO> patchDocument) {
        if (patchDocument is null)
            return BadRequest();

        var libroDb = await context.Libros.FirstOrDefaultAsync(x => x.Id == id);

        if (libroDb is null)
            return NotFound();

        var libroDTO = mapper.Map<LibroPatchDTO>(libroDb);

        patchDocument.ApplyTo(libroDTO, ModelState);
        var esValido = TryValidateModel(libroDTO);
        if (!esValido)
            return BadRequest(ModelState);

        mapper.Map(libroDTO, libroDb);

        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}", Name = "borrarLibro")]
    [Authorize(Policy = "EsAdmin")]
    public async Task<ActionResult> Delete(int id) {
        var existe = await context.Libros.AnyAsync(x => x.Id == id);
        if (!existe)
            return NotFound();

        context.Remove(new Libro() { Id = id });
        await context.SaveChangesAsync();
        return NoContent();
    }
}