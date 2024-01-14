using Microsoft.AspNetCore.Identity;

namespace AutoresAPI.Entidades; 
public class Usuario: IdentityUser {
    public bool MalaPaga { get; set; }
}