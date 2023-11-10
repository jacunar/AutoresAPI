using AutoresAPI.Entidades;
using Microsoft.AspNetCore.Mvc;

namespace AutoresAPI.Controllers;
[ApiController]
[Route("api/autores")]
public class AutoresController : ControllerBase {
    [HttpGet]
    public ActionResult<List<Autor>> Get() {
        return new List<Autor>() {
            new Autor() { Id=1, Nombre="Felipe" },
            new Autor() { Id=2, Nombre="Josue" }
        };
    }
}