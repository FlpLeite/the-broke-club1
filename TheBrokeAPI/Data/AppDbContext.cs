using Microsoft.EntityFrameworkCore;
using TheBrokeClub.API.Models;

namespace TheBrokeClub.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Transacao> Transacoes { get; set; }

    public DbSet<Metas> ObjetivosEconomia { get; set; }

    public DbSet<Investimento> Investimentos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>().ToTable("usuario");
        modelBuilder.Entity<Transacao>().ToTable("transacao");
        modelBuilder.Entity<Metas>().ToTable("objetivo_economia");
        modelBuilder.Entity<Investimento>().ToTable("investimentos");

        modelBuilder.Entity<Usuario>().HasKey(u => u.IdUsuario);
        modelBuilder.Entity<Transacao>().HasKey(t => t.IdTransacao);
        modelBuilder.Entity<Metas>().HasKey(o => o.IdObjetivo);
        modelBuilder.Entity<Investimento>().HasKey(i => i.IdInvestimento);

        modelBuilder.Entity<Metas>()
                .ToTable("metas")
                .HasKey(m => m.IdObjetivo);

        modelBuilder.Entity<Investimento>()
        .HasOne(i => i.Usuario)
        .WithMany()
        .HasForeignKey(i => i.IdUsuario);
    }

}
