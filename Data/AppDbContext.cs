using Microsoft.EntityFrameworkCore;
using TransVias.Models;

namespace TransVias.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Motorista> Motoristas { get; set; }
    public DbSet<Endereco> Enderecos { get; set; }
    public DbSet<Veiculo> Veiculos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // E-mail deve ser único
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Usuario 1 <-> 0..1 Motorista
        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Motorista)
            .WithOne(m => m.Usuario)
            .HasForeignKey<Motorista>(m => m.UsuarioId);

        // Motorista 1 <-> 1 Endereco
        modelBuilder.Entity<Motorista>()
            .HasOne(m => m.Endereco)
            .WithOne(e => e.Motorista)
            .HasForeignKey<Endereco>(e => e.MotoristaId);

        // Motorista 1 <-> 1 Veiculo
        modelBuilder.Entity<Motorista>()
            .HasOne(m => m.Veiculo)
            .WithOne(v => v.Motorista)
            .HasForeignKey<Veiculo>(v => v.MotoristaId);
    }
}