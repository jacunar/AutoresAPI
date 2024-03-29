﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;

namespace AutoresAPI.Controllers.V1;
[Route("api/v1/libros/{libroId:int}/comentarios")]
[ApiController]
public class ComentariosController : CustomBaseController {
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;
    private readonly UserManager<Usuario> userManager;

    public ComentariosController(ApplicationDbContext context, IMapper mapper, 
                UserManager<Usuario> userManager) {
        this.context = context;
        this.mapper = mapper;
        this.userManager = userManager;
    }

    [HttpGet(Name = "obtenerComentariosLibro")]
    public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId,
            [FromQuery] PaginationDTO paginationDTO) {
        var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);
        if (!existeLibro)
            return NotFound();

        var queryable = context.Comentarios.Where(x => x.LibroId == libroId).AsQueryable();
        await HttpContext.InsertPaginationParameterInHeader(queryable);
        var comentarios = await queryable.OrderBy(x => x.Id).Page(paginationDTO).ToListAsync();

        return mapper.Map<List<ComentarioDTO>>(comentarios);
    }

    [HttpGet("{id:int}", Name = "obtenerComentario")]
    public async Task<ActionResult<ComentarioDTO>> GetById(int id) {
        var comentario = await context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);
        if(comentario is null) 
            return NotFound();

        return mapper.Map<ComentarioDTO>(comentario);
    }

    [HttpPost(Name = "crearComentario")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO) {
        var emailClaim = HttpContext.User.Claims.Where(c => c.Type == "email").FirstOrDefault();
        if (emailClaim is null)
            return BadRequest("El usuario no es válido");

        string email = emailClaim.Value;
        if (string.IsNullOrEmpty(email))
            return BadRequest("El usuario no es válido");

        var usuario = await userManager.FindByEmailAsync(email);
        if (usuario is null)
            return BadRequest("El usuario no es válido");

        var usuarioId = usuario.Id;

        var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);
        if (!existeLibro)
            return NotFound();

        var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
        comentario.LibroId = libroId;
        comentario.UsuarioId = usuarioId;
        context.Add(comentario);
        await context.SaveChangesAsync();

        var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
        return CreatedAtRoute("obtenerComentario", new { id = comentario.Id, libroId }, comentarioDTO);
    }

    [HttpPut("{id:int}", Name = "actualizarComentario")]
    public async Task<ActionResult> Put(int libroId, int id, ComentarioCreacionDTO comentarioCreacionDTO) {
        var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);
        if (!existeLibro)
            return NotFound();

        var existeComentario = await context.Comentarios.AnyAsync(c => c.Id == id);
        if (!existeComentario)
            return NotFound();

        var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
        comentario.Id = id;
        comentario.LibroId = libroId;
        context.Update(comentario);
        await context.SaveChangesAsync();
        return NoContent();
    }
}