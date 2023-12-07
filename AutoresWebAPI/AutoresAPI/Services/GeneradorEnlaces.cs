using AutoresAPI.DTOs;
using System;

namespace AutoresAPI.Services; 
public class GeneradorEnlaces {
    public void GenerarEnlaces(AutorDTO autorDTO) {
        autorDTO.Enlaces.Add(new DatoHATEOAS(
                enlace: Url.Link("obtenerAutor", new { id = autorDTO.Id }) ?? "",
                descripcion: "self",
                metodo: "GET"));

        if (esAdmin) {
            autorDTO.Enlaces.Add(new DatoHATEOAS(
                    enlace: Url.Link("actualizarAutor", new { id = autorDTO.Id }) ?? "",
                    descripcion: "autor-actualizar",
                    metodo: "PUT"));
            autorDTO.Enlaces.Add(new DatoHATEOAS(
                    enlace: Url.Link("borrarAutor", new { id = autorDTO.Id }) ?? "",
                    descripcion: "autor-borrar",
                    metodo: "DELETE"));
        }
    }
}