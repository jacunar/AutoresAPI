using AutoresAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AutoresAPI.Utilities;
public class HATEOASAutorFilterAttribute: HATEOASFilterAttribute {
    public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next) {
        var debeIncluir = DebeIncluirHATEOAS(context);
        if(!debeIncluir) {
            await next();
            return;
        }

        var resultado = context.Result as ObjectResult ?? null!;
        var modelo = resultado.Value as AutorDTO ?? throw new 
                        ArgumentException("Se esperaba una instancia de AutorDTO");

        await next();
    }
}