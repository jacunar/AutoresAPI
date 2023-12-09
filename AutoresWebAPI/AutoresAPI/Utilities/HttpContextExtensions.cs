using Microsoft.EntityFrameworkCore;

namespace AutoresAPI.Utilities; 
public static class HttpContextExtensions {
    public async static Task InsertPaginationParameterInHeader<T>(this HttpContext httpContext,
        IQueryable<T> queryable) {
        if (httpContext == null) { throw new ArgumentNullException(nameof(httpContext)); }

        double cantidad = await queryable.CountAsync();
        httpContext.Response.Headers.Add("cantidadTotalRegistros", cantidad.ToString());
    }
}