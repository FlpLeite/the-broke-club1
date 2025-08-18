using Microsoft.EntityFrameworkCore;
using TheBrokeClub.API.Models;

namespace TheBrokeClub.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Transacao> Transacoes { get; set; }
    public DbSet<Metas> ObjetivosEconomia { get; set; }

    public DbSet<InvestimentoPrecoCache> InvestimentoPrecoCache { get; set; }
    public DbSet<QuoteDailyUsage> QuoteDailyUsage { get; set; }
    public DbSet<InvestimentoAtivo> InvestimentoAtivo { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>().ToTable("usuario");
        modelBuilder.Entity<Transacao>().ToTable("transacao");
        modelBuilder.Entity<Metas>().ToTable("objetivo_economia");

        modelBuilder.Entity<Usuario>().HasKey(u => u.IdUsuario);
        modelBuilder.Entity<Transacao>().HasKey(t => t.IdTransacao);
        modelBuilder.Entity<Metas>().HasKey(o => o.IdObjetivo);

        modelBuilder.Entity<Metas>()
            .ToTable("metas")
            .HasKey(m => m.IdObjetivo);

        modelBuilder.Entity<InvestimentoPrecoCache>(e =>
        {
            e.ToTable("investimento_preco_cache");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.InvestimentoAtivoId).HasColumnName("investimento_ativo_id");
            e.Property(x => x.Price).HasColumnName("price");
            e.Property(x => x.Currency).HasColumnName("currency");
            e.Property(x => x.AsOf).HasColumnName("asof");
            e.Property(x => x.Source).HasColumnName("source");
        });

        modelBuilder.Entity<QuoteDailyUsage>(e =>
        {
            e.ToTable("quote_daily_usage");
            e.HasKey(x => x.Day);
            e.Property(x => x.Day).HasColumnName("day");
            e.Property(x => x.Used).HasColumnName("used");
        });

        modelBuilder.Entity<Transacao>()
            .Property(t => t.InvestimentoAtivoId)
            .HasColumnName("investimento_ativo_id");

        modelBuilder.Entity<InvestimentoAtivo>(e =>
        {
            e.ToTable("investimento_ativo");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.UsuarioId).HasColumnName("usuario_id");
            e.Property(x => x.Ticker).HasColumnName("ticker");
            e.Property(x => x.Nome).HasColumnName("nome");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<Transacao>().Property(t => t.Quantidade).HasColumnName("quantidade");
        modelBuilder.Entity<Transacao>().Property(t => t.PrecoUnit).HasColumnName("preco_unit");
    }
}
