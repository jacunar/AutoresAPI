using AutoresAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

#nullable disable

namespace AutoresAPI.Services; 
public class GeneradorEnlaces {
    private readonly IAuthorizationService authorizationService;
    private readonly IHttpContextAccessor contextAccessor;
    private readonly IActionContextAccessor actionContextAccessor;

    public GeneradorEnlaces(IAuthorizationService authorizationService,
                IHttpContextAccessor contextAccessor, IActionContextAccessor actionContextAccessor) {
        this.authorizationService = authorizationService;
        this.contextAccessor = contextAccessor;
        this.actionContextAccessor = actionContextAccessor;
    }

    private IUrlHelper ConstruirUrlHelper() {
        var factoria = contextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
        return factoria.GetUrlHelper(actionContextAccessor.ActionContext);
    }

    private async Task<bool> EsAdmin() {
        var httpContext = contextAccessor.HttpContext;
        var resultado = await authorizationService.AuthorizeAsync(httpContext.User, "esAdmin");
        return resultado.Succeeded;
    }

    public async Task GenerarEnlaces(AutorDTO autorDTO) {
        var esAdmin = await EsAdmin();
        var Url = ConstruirUrlHelper();

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