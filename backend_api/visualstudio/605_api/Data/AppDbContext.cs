using Microsoft.EntityFrameworkCore;
using _605_api.Models;

namespace _605_api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Vinho> Vinhos { get; set; }
    public DbSet<Experiencia> Experiencias { get; set; }
    public DbSet<Reserva> Reservas { get; set; }
    public DbSet<Utilizador> Utilizadores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Vinho>()
            .Property(v => v.Preco)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Experiencia>()
            .Property(e => e.Preco)
            .HasPrecision(10, 2);

        // Seed de Vinhos
        modelBuilder.Entity<Vinho>().HasData(
            new Vinho { Id = 1, Nome = "Arinto Clássico", Tipo = "branco", Ano = 2022, Descricao = "Fresco e mineral, com notas cítricas típicas de Bucelas.", Preco = 12.50m, Imagem = "arinto_classico.png", Sku = "QA-001" },
            new Vinho { Id = 2, Nome = "Arinto Reserva", Tipo = "reserva", Ano = 2020, Descricao = "Maior complexidade e estágio em madeira. Elegante e persistente.", Preco = 18.00m, Imagem = "arinto_reserva.png", Sku = "QA-002" },
            new Vinho { Id = 3, Nome = "Arinto Colheita Tardia", Tipo = "colheita", Ano = 2021, Descricao = "Notas de mel e fruta madura. Acidez vibrante em equilíbrio.", Preco = 22.00m, Imagem = "arinto_colheita.png", Sku = "QA-003" }
        );

        // Seed de Experiências
        modelBuilder.Entity<Experiencia>().HasData(
            new Experiencia { Id = 1, Nome = "Prova de Arinto", Descricao = "Uma viagem pelos solos de Bucelas através dos nossos melhores Arintos.", Preco = 25.00m, DuracaoMinutos = 90, MaxPessoas = 20, Imagem = "prova_vinhos.png" },
            new Experiencia { Id = 2, Nome = "Visita às Vinhas", Descricao = "Passeio guiado pelas vinhas centenárias de Arinto.", Preco = 15.00m, DuracaoMinutos = 60, MaxPessoas = 15, Imagem = "QtaAzenha.png" },
            new Experiencia { Id = 3, Nome = "Workshop Vindima", Descricao = "Participe na colheita manual das uvas Arinto.", Preco = 40.00m, DuracaoMinutos = 180, MaxPessoas = 12, Imagem = "prova_vinhos.png" },
            new Experiencia { Id = 4, Nome = "Jantar na Adega", Descricao = "Menu tradicional português harmonizado com os nossos vinhos.", Preco = 60.00m, DuracaoMinutos = 150, MaxPessoas = 10, Imagem = "jantar.png" },
            new Experiencia { Id = 5, Nome = "Jacuzzi", Descricao = "Bolhas e Vinho", Preco = 80.00m, DuracaoMinutos = 60, MaxPessoas = 4, Imagem = "jacuzzi.jpg" }
        );
    }
}