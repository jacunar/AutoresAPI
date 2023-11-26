﻿using AutoMapper;
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
                    .Include(x => x.AutoresLibros)
                    .ThenInclude(x => x.Autor)
                    .FirstOrDefaultAsync(l => l.Id == id);
        
        return mapper.Map<LibroDTOConAutores>(libro);
    }

    [HttpPost]
    public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO) {
        if (libroCreacionDTO.AutoresIds is null)
            return BadRequest("No se puede crear un libro sin autores");

        var autoresIds = await context.Autores
                            .Where(x => libroCreacionDTO.AutoresIds.Contains(x.Id))
                            .Select(x => x.Id).ToListAsync();
        
        if (libroCreacionDTO.AutoresIds.Count != autoresIds.Count)
            return BadRequest("No existe uno de los autores enviados.");
        
        var libro = mapper.Map<Libro>(libroCreacionDTO);

        if (libro.AutoresLibros != null) {
            for (int i = 0; i < libro.AutoresLibros.Count; i++) {
                libro.AutoresLibros[i].Orden = i;
            }
        }

        context.Add(libro);
        await context.SaveChangesAsync();

        var libroDTO = mapper.Map<LibroDTO>(libro);
        return CreatedAtRoute("obtenerLibro", new { id = libro.Id }, libroDTO);
    }
}