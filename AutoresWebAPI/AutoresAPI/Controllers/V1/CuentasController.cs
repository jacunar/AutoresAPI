using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AutoresAPI.Controllers.V1;
[Route("api/v1/cuentas")]
[ApiController]
public class CuentasController : CustomBaseController {
    private readonly UserManager<Usuario> userManager;
    private readonly IConfiguration configuration;
    private readonly SignInManager<Usuario> signInManager;
    private readonly ServicioLlaves servicioLlaves;

    public CuentasController(UserManager<Usuario> userManager, IConfiguration configuration,
                                SignInManager<Usuario> signInManager, ServicioLlaves servicioLlaves) {
        this.userManager = userManager;
        this.configuration = configuration;
        this.signInManager = signInManager;
        this.servicioLlaves = servicioLlaves;
    }
        
    [HttpPost("registrar", Name = "registrarUsuario")]
    public async Task<ActionResult<RespuestaAutenticacion>> Registrar(CredencialUsuario credencialUsuario) {
        var usuario = new Usuario {
            UserName = credencialUsuario.Email,
            Email = credencialUsuario.Email
        };
        var resultado = await userManager.CreateAsync(usuario, credencialUsuario.Password);

        if (resultado.Succeeded) {
            await servicioLlaves.CrearLLave(usuario.Id, TipoLlave.Gratuita);
            return await ConstruirToken(credencialUsuario, usuario.Id);
        } else
            return BadRequest(resultado.Errors);
    }

    [HttpPost("login", Name = "loginUsuario")]
    public async Task<ActionResult<RespuestaAutenticacion>> Login(CredencialUsuario credencialUsuario) {
        var resultado = await signInManager.PasswordSignInAsync(credencialUsuario.Email,
                    credencialUsuario.Password, isPersistent: false, lockoutOnFailure: false);
        if (resultado.Succeeded) {
            var usuario = await userManager.FindByEmailAsync(credencialUsuario.Email) ?? null!;
            return await ConstruirToken(credencialUsuario, usuario.Id);
        } else
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

        var idClaim = HttpContext.User.Claims.Where(c => c.Type == "id").FirstOrDefault();
        var usuarioId = idClaim?.Value;

        var credencialUsuario = new CredencialUsuario() {
            Email = email
        };
        return await ConstruirToken(credencialUsuario, usuarioId ?? "");
    }

    private async Task<ActionResult<RespuestaAutenticacion>> 
            ConstruirToken(CredencialUsuario credencialUsuario, string usuarioId) {
        var claims = new List<Claim> {
            new("email", credencialUsuario.Email),
            new("id", usuarioId)
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
}