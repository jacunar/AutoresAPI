using AutoMapper;
using AutoresAPI.DTOs;
using AutoresAPI.Entidades;

namespace AutoresAPI.Utilities;
public class AutoMapperProfiles : Profile {
    public AutoMapperProfiles() {
        CreateMap<AutorCreacionDTO, Autor>();
        CreateMap<Autor, AutorDTO>();
        CreateMap<Autor, AutorDTOConLibros>()
                .ForMember(autorDTO => autorDTO.Libros, opciones => opciones.MapFrom(MapAutorDTOLibros));

        CreateMap<LibroCreacionDTO, Libro>()
            .ForMember(x => x.AutoresLibros, opt => opt.MapFrom(MapAutoresLibros));
        CreateMap<Libro, LibroDTO>();
        CreateMap<Libro, LibroDTOConAutores>()
            .ForMember(x => x.Autores, opt => opt.MapFrom(MapLibroDTOAutores));
        CreateMap<LibroPatchDTO, Libro>().ReverseMap();

        CreateMap<ComentarioCreacionDTO, Comentario>();
        CreateMap<Comentario, ComentarioDTO>();
    }
    private List<AutorDTO> MapLibroDTOAutores(Libro libro, LibroDTOConAutores libroDTO) {
        var resultado = new List<AutorDTO>();

        if (libro.AutoresLibros is null) return resultado;

        foreach (var autorLibro in libro.AutoresLibros) {
            resultado.Add(new AutorDTO() {
                Id = autorLibro.LibroId,
                Nombre = autorLibro.Autor.Nombre
            });
        }

        return resultado;
    }

    private List<AutorLibro> MapAutoresLibros(LibroCreacionDTO libroCreacionDTO, Libro libro) {
        var resultado = new List<AutorLibro>();

        if (libroCreacionDTO.AutoresIds == null) return resultado;

        foreach (var autorId in libroCreacionDTO.AutoresIds)
            resultado.Add(new AutorLibro() { AutorId = autorId });

        return resultado;
    }

    private List<LibroDTO> MapAutorDTOLibros(Autor autor, AutorDTO autorDTO) {
        var resultado = new List<LibroDTO>();

        if (autor.AutoresLibros == null) { return resultado; }

        foreach (var autorLibro in autor.AutoresLibros) {
            resultado.Add(new LibroDTO() {
                Id = autorLibro.LibroId,
                Titulo = autorLibro.Libro.Titulo
            });
        }

        return resultado;
    }
}