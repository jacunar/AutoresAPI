﻿using AutoMapper;
using AutoresAPI.DTOs;
using AutoresAPI.Entidades;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace AutoresAPI.Controllers; 
[Route("api/libros/{libroId:int}/comentarios")]
[ApiController]
public class ComentariosController : ControllerBase {
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;
    private readonly UserManager<IdentityUser> userManager;

    public ComentariosController(ApplicationDbContext context, IMapper mapper, 
                UserManager<IdentityUser> userManager) {
        this.context = context;
        this.mapper = mapper;
        this.userManager = userManager;
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

    [HttpGet("{id:int}", Name = "obtenerComentario")]
    public async Task<ActionResult<ComentarioDTO>> GetById(int id) {
        var comentario = await context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);
        if(comentario is null) 
            return NotFound();

        return mapper.Map<ComentarioDTO>(comentario);
    }

    [HttpPost]
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
        return CreatedAtRoute("obtenerComentario", new { id = comentario.Id, libroId = libroId }, comentarioDTO);
    }

    [HttpPut("{id:int}")]
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