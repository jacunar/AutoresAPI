using AutoMapper;
using AutoresAPI.DTOs;
using AutoresAPI.Entidades;

namespace AutoresAPI.Utilities; 
public class AutoMapperProfiles: Profile {
    public AutoMapperProfiles() {
        CreateMap<AutorCreacionDTO, Autor>();
        CreateMap<Autor, AutorDTO>();
        CreateMap<LibroCreacionDTO, Libro>();
        CreateMap<Libro, LibroDTO>();
        CreateMap<ComentarioCreacionDTO, Comentario>();
        CreateMap<Comentario, ComentarioDTO>();
    }
}