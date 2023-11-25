using AutoMapper;
using AutoresAPI.DTOs;
using AutoresAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoresAPI.Controllers;
[ApiController]
[Route("api/libros")]
public class LibrosController: ControllerBase {
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;

    public LibrosController(ApplicationDbContext context, IMapper mapper) {
        this.context = context;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<List<LibroDTO>> Get() {
        var libros = await context.Libros.ToListAsync();
        return mapper.Map<List<LibroDTO>>(libros);
    }

    [HttpGet("{id:int}", Name = "obtenerLibro")]
    public async Task<ActionResult<LibroDTOConAutores>> Get(int id) {
        var libro = await context.Libros
                    .Include(x => x.AutoresLibros).ThenInclude(a => a.Autor)
                    .FirstOrDefaultAsync(l => l.Id == id);
        return mapper.Map<LibroDTOConAutores>(libro);
    }

    [HttpPost]
    public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO) {
        //var existeAutor = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);

        //if (!existeAutor)
        //    return BadRequest($"No existe el autor con el Id: {libro.AutorId}");
        var libro = mapper.Map<Libro>(libroCreacionDTO);
        context.Add(libro);
        await context.SaveChangesAsync();

        var libroDTO = mapper.Map<LibroDTO>(libro);
        return CreatedAtRoute("obtenerLibro", new { id = libro.Id }, libroDTO);
    }
}