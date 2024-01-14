﻿namespace AutoresAPI.Controllers.V1;

public class CustomBaseController : ControllerBase {
    protected string ObtenerUsuarioId() {
        var usuarioClaim = HttpContext.User.Claims.Where(x => x.Type == "id").FirstOrDefault();
        return usuarioClaim?.Value ?? "";
    }
}