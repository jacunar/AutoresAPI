using AutoresAPI.Entidades;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutoresAPI;
public class ApplicationDbContext : IdentityDbContext {
    public ApplicationDbContext(DbContextOptions options) : base(options) {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AutorLibro>().HasKey(al => new { al.AutorId, al.LibroId });

        modelBuilder.Entity<Autor>().HasData(
            new Autor { Id = 1, Nombre = "Lewis Carroll" },
            new Autor { Id = 2, Nombre = "Fyodor Dostoevsky" },
            new Autor { Id = 3, Nombre = "William Shakespeare" },
            new Autor { Id = 4, Nombre = "Herman Melville" },
            new Autor { Id = 5, Nombre = "Dante Alighieri" },
            new Autor { Id = 6, Nombre = "Miguel De Cervantes" },
            new Autor { Id = 7, Nombre = "Gabriel García Márquez" }
            );
        modelBuilder.Entity<Libro>().HasData(
            new Libro { Id = 1, Titulo = "Alice’s Adventures In Wonderland" },
            new Libro { Id = 2, Titulo = "Crime And Punishment" },
            new Libro { Id = 3, Titulo = "Hamlet" },
            new Libro { Id = 4, Titulo = "Moby Dick" },
            new Libro { Id = 5, Titulo = "The Divine Comedy" },
            new Libro { Id = 6, Titulo = "Don Quixote" },
            new Libro { Id = 7, Titulo = "One Hundred Years Of Solitude" }
            );
    }
    public DbSet<Autor> Autores { get; set; }
    public DbSet<Libro> Libros { get; set; }
    public DbSet<Comentario> Comentarios { get; set; }
    public DbSet<AutorLibro> AutoresLibros { get; set; }
}