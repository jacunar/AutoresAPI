using AutoresAPI.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoresAPI.Controllers;
[Route("api")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class RootController : ControllerBase {
    [HttpGet(Name = "ObtenerRoot")]
    [AllowAnonymous]
    public ActionResult<IEnumerable<DatoHATEOAS>> Get() {
        var datosHateoas = new List<DatoHATEOAS>();

        datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("ObtenerRoot", new { }) ?? "",
                descripcion: "self", metodo: "GET"));

        datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("obtenerAutores", new { }) ?? "", descripcion: "autores",
                metodo: "GET"));

        datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("crearAutor", new { }) ?? "", descripcion: "autor-crear",
       metodo: "POST"));

        datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("crearLibro", new { }) ?? "", descripcion: "libro-crear",
            metodo: "POST"));

        return datosHateoas;
    }
}