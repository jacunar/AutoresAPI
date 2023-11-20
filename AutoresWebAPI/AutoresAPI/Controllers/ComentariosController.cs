using AutoMapper;
using AutoresAPI.DTOs;
using AutoresAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace AutoresAPI.Controllers; 
[Route("api/libros/{libroId:int}/comentarios")]
[ApiController]
public class ComentariosController : ControllerBase {
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;

    public ComentariosController(ApplicationDbContext context, IMapper mapper) {
        this.context = context;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId) {
        var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);
        if (!existeLibro)
            return NotFound();

        var comentarios = await context.Comentarios
                .Where(x => x.LibroId == libroId).ToListAsync();

        return mapper.Map<List<ComentarioDTO>>(comentarios);
    }

    [HttpPost]
    public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO) {
        var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);
        if (!existeLibro)
            return NotFound();

        var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
        comentario.LibroId = libroId;
        context.Add(comentario);
        await context.SaveChangesAsync();
        return Ok();
    }
}