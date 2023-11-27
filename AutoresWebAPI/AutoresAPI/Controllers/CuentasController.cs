using AutoresAPI.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
    private readonly SignInManager<IdentityUser> signInManager;

    public CuentasController(UserManager<IdentityUser> userManager, IConfiguration configuration,
                                SignInManager<IdentityUser> signInManager) {
        this.userManager = userManager;
        this.configuration = configuration;
        this.signInManager = signInManager;
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

    [HttpPost("login")]
    public async Task<ActionResult<RespuestaAutenticacion>> Login(CredencialUsuario credencialUsuario) {
        var resultado = await signInManager.PasswordSignInAsync(credencialUsuario.Email,
                    credencialUsuario.Password, isPersistent: false, lockoutOnFailure: false);
        if (resultado.Succeeded)
            return ConstruirToken(credencialUsuario);
        else
            return BadRequest("Login incorrecto");
    }

    [HttpGet("RenoverToken")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public ActionResult<RespuestaAutenticacion> Renovar() {
        var emailClaim = HttpContext.User.Claims.Where(c => c.Type == "email").FirstOrDefault();
        if (emailClaim is null)
            return BadRequest("El usuario no es válido");

        string email = emailClaim.Value;
        if (string.IsNullOrEmpty(email))
            return BadRequest("El usuario no es válido");

        var credencialUsuario = new CredencialUsuario() {
            Email = email
        };
        return ConstruirToken(credencialUsuario);
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