using AutoMapper;
using AutoresAPI.DTOs;
using AutoresAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoresAPI.Controllers;
[ApiController]
[Route("api/autores")]
public class AutoresController : ControllerBase {
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;

    public AutoresController(ApplicationDbContext context, IMapper mapper) {
        this.context = context;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<List<AutorDTO>> Get() {
        var autores = await context.Autores.ToListAsync();
        return mapper.Map<List<AutorDTO>>(autores);

    }

    [HttpGet("{id:int}", Name = "obtenerAutor")]
    public async Task<ActionResult<AutorDTOConLibros>> Get(int id) {
        var autor = await context.Autores
            .Include(a => a.AutoresLibros).ThenInclude(l => l.Libro)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (autor is null)
            return NotFound();

        return mapper.Map<AutorDTOConLibros>(autor);
    }

    [HttpGet("{nombre}")]
    public async Task<ActionResult<List<AutorDTO>>> Get([FromRoute] string nombre) {
        var autores = await context.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync();

        return mapper.Map<List<AutorDTO>>(autores);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO) {
        var existeAutorConElMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre);
        
        if (existeAutorConElMismoNombre)
            return BadRequest($"Ya existe un autor con el nombre {autorCreacionDTO.Nombre}");

        var autor = mapper.Map<Autor>(autorCreacionDTO);

        context.Add(autor);
        await context.SaveChangesAsync();

        var autorDTO = mapper.Map<AutorDTO>(autor);
        return CreatedAtRoute("obtenerAutor", new { id = autor.Id }, autorDTO);
    }

    [HttpPut("{id:int}")]
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

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id) {
        var existe = await context.Autores.AnyAsync(x => x.Id == id);
        if (!existe)
            return NotFound();

        context.Remove(new Autor() { Id = id });
        await context.SaveChangesAsync();
        return Ok();
    }
}

//https://lecturasdepapelazulcom.wordpress.com/2020/05/10/libros-escritos-por-varios-escritores/