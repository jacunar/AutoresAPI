﻿namespace AutoresAPI.DTOs; 
public class AutorDTO: Recurso {
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;    
}