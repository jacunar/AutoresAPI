using AutoresAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoresAPI.Controllers;
[ApiController]
[Route("api/libros")]
public class LibrosController: ControllerBase {
    private readonly ApplicationDbContext context;

    public LibrosController(ApplicationDbContext context) {
        this.context = context;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Libro>> Get(int id) {
        return await context.Libros
                .Include(x => x.Autor)
                .FirstOrDefaultAsync(l => l.Id == id);
    }

    [HttpPost]
    public async Task<ActionResult> Post(Libro libro) {
        var existeAutor = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);

        if (!existeAutor)
            return BadRequest($"No existe el autor con el Id: { libro.AutorId}");

        context.Add(libro);
        await context.SaveChangesAsync();
        return Ok();
    }
}