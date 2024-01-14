namespace AutoresAPI.Middlewares;
public static class LimitarPeticionesMiddlewareExtensions {
    public static IApplicationBuilder UseLimitarPeticiones(this IApplicationBuilder app) {
        return app.UseMiddleware<LimitarPeticionesMiddleware>();
    }
}

public class LimitarPeticionesMiddleware {
    private readonly RequestDelegate siguiente;
    private readonly IConfiguration configuration;

    public LimitarPeticionesMiddleware(RequestDelegate siguiente, IConfiguration configuration) {
        this.siguiente = siguiente;
        this.configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext context) {
        var limitarPeticionesConfiguracion = new LimitarPeticionesConfiguracion();
        configuration.GetRequiredSection("limitarPeticiones").Bind(limitarPeticionesConfiguracion);

        var ruta = httpContext.Request.Path.ToString();
        var estaLaRutaEnListaBlanca = limitarPeticionesConfiguracion.ListaBlancaRutas.Any(x => ruta.Contains(x));

        if (estaLaRutaEnListaBlanca) {
            await siguiente(httpContext);
            return;
        }

        var llavesStringValues = httpContext.Request.Headers["X-Api-Key"];
        if (llavesStringValues.Count == 0) {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("Debe proveer la llave en la cabecera X-Api-Key");
            return;
        }

        if (llavesStringValues.Count > 1) {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("Solo una llave debe estar presente");
            return;
        }

        var llave = llavesStringValues[0];
        var llaveDB = await context.LlavesAPI
            .Include(x => x.RestriccionesDominio).Include(x => x.RestriccionesIP)
            .Include(x => x.Usuario)
            .FirstOrDefaultAsync(x => x.Llave == llave);

        if (llaveDB is null) {
            httpContext.Response.StatusCode = 404;
            await httpContext.Response.WriteAsync("La llave no existe");
            return;
        }
        if (!llaveDB.Activa) {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("La llave se encuentra inactiva");
            return;
        }

        if (llaveDB.TipoLlave == TipoLlave.Gratuita) {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var peticionesRealizadasHoy = await context.Peticiones.CountAsync(x =>
                x.LlaveAPIId == llaveDB.Id && x.FechaPeticion >= today && x.FechaPeticion < tomorrow);

            if (peticionesRealizadasHoy >= limitarPeticionesConfiguracion.PeticionesPorDiaGratuito) {
                httpContext.Response.StatusCode = 429; //Too many requests
                await httpContext.Response.WriteAsync("Ha excedido el límite de peticiones por día. Si desea" +
                    "realizar más peticiones, actualice su suscripción a una cuenta profesional");
                return;
            }
        } else if (llaveDB.Usuario.MalaPaga) {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("El usuario tiene una cuenta pendiente");
            return;
        }

        var superaRestricciones = PeticionSuperaAlgunaDeLasRestricciones(llaveDB, httpContext);
        if (!superaRestricciones) {
            httpContext.Response.StatusCode = 403;
            return;
        }

        var peticion = new Peticion() {
            LlaveAPIId = llaveDB.Id, FechaPeticion = DateTime.Now
        };
        context.Add(peticion);
        await context.SaveChangesAsync();

        await siguiente(httpContext);
    }

    private bool PeticionSuperaAlgunaDeLasRestricciones(LlaveAPI llaveAPI, HttpContext httpContext) {
        var hayRestricciones = llaveAPI.RestriccionesDominio.Any() || llaveAPI.RestriccionesIP.Any();
        if (!hayRestricciones)
            return true;

        var peticionSuperaRestriccionesDeDominio = 
            PeticionSuperaLasRestriccionesDeDominio(llaveAPI.RestriccionesDominio, httpContext);
        var peticionSuperaRestriccionDeIP =
            PeticionSuperaLasRestriccionesDeIP(llaveAPI.RestriccionesIP, httpContext);

        return peticionSuperaRestriccionesDeDominio || peticionSuperaRestriccionDeIP;
    }

    private bool PeticionSuperaLasRestriccionesDeIP(List<RestriccionIP> restriccions, HttpContext httpContext) {
        if (restriccions is null || restriccions.Count == 0)
            return false;

        var IP = httpContext.Connection.RemoteIpAddress.ToString();
        if (IP == string.Empty)
            return false;

        var superaRestriccion = restriccions.Any(x => x.IP == IP);
        return superaRestriccion;
    }

    private bool PeticionSuperaLasRestriccionesDeDominio(List<RestriccionDominio> restricciones, HttpContext httpContext) {
        if (restricciones is null || restricciones.Count == 0)
            return false;

        var referer = httpContext.Request.Headers["Referer"].ToString();
        if (referer == string.Empty)
            return false;

        Uri myUri = new Uri(referer);
        string host = myUri.Host;
        var superaRestriccion = restricciones.Any(x => x.Dominio == host);

        return superaRestriccion;
    }
}