using AutoresAPI.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AutoresAPI.Controllers; 
[Route("api/cuentas")]
[ApiController]
public class CuentasController : ControllerBase {
    private readonly UserManager<IdentityUser> userManager;
    private readonly IConfiguration configuration;

    public CuentasController(UserManager<IdentityUser> userManager, IConfiguration configuration) {
        this.userManager = userManager;
        this.configuration = configuration;
    }

    [HttpPost("registrar")]
    public async Task<ActionResult<RespuestaAutenticacion>> Registrar(CredencialUsuario credencialUsuario) {
        var usuario = new IdentityUser {
            UserName = credencialUsuario.Email,
            Email = credencialUsuario.Email
        };
        var resultado = await userManager.CreateAsync(usuario, credencialUsuario.Password);

        if (resultado.Succeeded)
            return ConstruirToken(credencialUsuario);
        else
            return BadRequest(resultado.Errors);
    }

    private RespuestaAutenticacion ConstruirToken(CredencialUsuario credencialUsuario) {
        var claims = new List<Claim> {
            new Claim("email", credencialUsuario.Email)
        };

        var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llavejwt"] ?? ""));
        var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
        var expiracion = DateTime.UtcNow.AddHours(12);

        var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                            expires: expiracion, signingCredentials: creds);

        return new RespuestaAutenticacion() {
            Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
            Expiracion = expiracion
        };
    }
}