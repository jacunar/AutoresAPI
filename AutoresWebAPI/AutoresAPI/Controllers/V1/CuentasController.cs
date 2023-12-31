﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AutoresAPI.Controllers.V1; 
[Route("api/v1/cuentas")]
[ApiController]
public class CuentasController : ControllerBase {
    private readonly UserManager<IdentityUser> userManager;
    private readonly IConfiguration configuration;
    private readonly SignInManager<IdentityUser> signInManager;
    private readonly HashService hashService;
    private readonly IDataProtector dataProtector;

    public CuentasController(UserManager<IdentityUser> userManager, IConfiguration configuration,
                                SignInManager<IdentityUser> signInManager,
                                IDataProtectionProvider dataProtectionProvider,
                                HashService hashService) {
        this.userManager = userManager;
        this.configuration = configuration;
        this.signInManager = signInManager;
        this.hashService = hashService;
        dataProtector = dataProtectionProvider.CreateProtector("clave_unica_y_secreta");
    }

    [HttpGet("encriptar")]
    private ActionResult Encriptar() {
        string textoPlano = "Josue Acuña";
        var textoCifrado = dataProtector.Protect(textoPlano);
        var textoDesencriptado = dataProtector.Unprotect(textoCifrado);

        return Ok(new {  
            textoPlano, textoCifrado, textoDesencriptado
        });
    }

    [HttpGet("encriptarPorTiempo")]
    private ActionResult EncriptarPorTiempo() {
        var protectorPorTiempoLimitado = dataProtector.ToTimeLimitedDataProtector();

        string textoPlano = "Josue Acuña";
        var textoCifrado = protectorPorTiempoLimitado.Protect(textoPlano, lifetime: TimeSpan.FromSeconds(6));
        Thread.Sleep(7000);
        var textoDesencriptado = protectorPorTiempoLimitado.Unprotect(textoCifrado);

        return Ok(new {
            textoPlano, textoCifrado, textoDesencriptado
        });
    }

    [HttpPost("registrar", Name = "registrarUsuario")]
    public async Task<ActionResult<RespuestaAutenticacion>> Registrar(CredencialUsuario credencialUsuario) {
        var usuario = new IdentityUser {
            UserName = credencialUsuario.Email,
            Email = credencialUsuario.Email
        };
        var resultado = await userManager.CreateAsync(usuario, credencialUsuario.Password);

        if (resultado.Succeeded)
            return await ConstruirToken(credencialUsuario);
        else
            return BadRequest(resultado.Errors);
    }

    [HttpPost("login", Name = "loginUsuario")]
    public async Task<ActionResult<RespuestaAutenticacion>> Login(CredencialUsuario credencialUsuario) {
        var resultado = await signInManager.PasswordSignInAsync(credencialUsuario.Email,
                    credencialUsuario.Password, isPersistent: false, lockoutOnFailure: false);
        if (resultado.Succeeded)
            return await ConstruirToken(credencialUsuario);
        else
            return BadRequest("Login incorrecto");
    }

    [HttpGet("RenovarToken", Name = "renovarToken")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<RespuestaAutenticacion>> Renovar() {
        var emailClaim = HttpContext.User.Claims.Where(c => c.Type == "email").FirstOrDefault();
        if (emailClaim is null)
            return BadRequest("El usuario no es válido");

        string email = emailClaim.Value;
        if (string.IsNullOrEmpty(email))
            return BadRequest("El usuario no es válido");

        var credencialUsuario = new CredencialUsuario() {
            Email = email
        };
        return await ConstruirToken(credencialUsuario);
    }

    private async Task<ActionResult<RespuestaAutenticacion>> ConstruirToken(CredencialUsuario credencialUsuario) {
        var claims = new List<Claim> {
            new("email", credencialUsuario.Email)
        };

        var usuario = await userManager.FindByEmailAsync(credencialUsuario.Email);
        if (usuario != null) {
            var claimsDB = await userManager.GetClaimsAsync(usuario);
            claims.AddRange(claimsDB);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llavejwt"] ?? ""));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
            var expiracion = DateTime.UtcNow.AddHours(12);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                                expires: expiracion, signingCredentials: creds);

            return new RespuestaAutenticacion() {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion
            };
        } else
            return BadRequest("El usuario no existe");
    }

    [HttpPost("HacerAdmin", Name = "hacerAdmin")]
    public async Task<ActionResult> HacerAdmin(EditarAdminDTO editarAdminDTO) {
        var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email);
        if (usuario != null) {
            await userManager.AddClaimAsync(usuario, new Claim("esAdmin", "1"));
            return NoContent();
        } else
            return BadRequest("El usuario no existe");
    }

    [HttpPost("RemoverAdmin", Name = "removerAdmin")]
    public async Task<ActionResult> RemoverAdmin(EditarAdminDTO editarAdminDTO) {
        var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email);
        if (usuario != null) {
            await userManager.RemoveClaimAsync(usuario, new Claim("esAdmin", "1"));
            return NoContent();
        } else
            return BadRequest("El usuario no existe");
    }

    [HttpGet("hash/{textoPlano}")]
    private ActionResult RealizarHash(string textoPlano) {
        var resultado1 = hashService.Hash(textoPlano);
        var resultado2 = hashService.Hash(textoPlano);

        return Ok(new {
            textoPlano, resultado1, resultado2
        });
    }
}