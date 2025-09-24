using Microsoft.EntityFrameworkCore;
using TheBrokeClub.API.Models;

namespace TheBrokeClub.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Transacao> Transacoes { get; set; }
    public DbSet<Metas> ObjetivosEconomia { get; set; }

    public DbSet<InvestimentoAtivo> InvestimentoAtivo { get; set; }
    public DbSet<InvestimentoPrecoCache> InvestimentoPrecoCache { get; set; }
    public DbSet<QuoteDailyUsage> QuoteDailyUsage { get; set; }

    // cache central por ticker (worker)
    public DbSet<TickerPriceCache> TickerPriceCache { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // usuario
        modelBuilder.Entity<Usuario>(e =>
        {
            e.ToTable("usuario");
            e.HasKey(u => u.IdUsuario);
        });

        // transacao
        modelBuilder.Entity<Transacao>(e =>
        {
            e.ToTable("transacao");
            e.HasKey(t => t.IdTransacao);

            // campos snake_case
            e.Property(t => t.InvestimentoAtivoId).HasColumnName("investimento_ativo_id");
            e.Property(t => t.PrecoUnit).HasColumnName("preco_unit");
            e.Property(t => t.Quantidade).HasColumnName("quantidade");

            // ⚠️ NADA de colocar aspas manualmente aqui
            // Se a coluna do banco foi criada como "UsuarioIdUsuario" (com aspas), passe o nome cru:
            e.Property(t => t.UsuarioIdUsuario).HasColumnName("UsuarioIdUsuario");
        });

        // metas (sua tabela real é "metas")
        modelBuilder.Entity<Metas>(e =>
        {
            e.ToTable("metas");
            e.HasKey(o => o.IdObjetivo);
        });

        // investimento_ativo
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

        // investimento_preco_cache
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

        // quote_daily_usage
        modelBuilder.Entity<QuoteDailyUsage>(e =>
        {
            e.ToTable("quote_daily_usage");
            e.HasKey(x => x.Day);
            e.Property(x => x.Day).HasColumnName("day");
            e.Property(x => x.Used).HasColumnName("used");
        });

        // ticker_price_cache
        modelBuilder.Entity<TickerPriceCache>(e =>
        {
            e.ToTable("ticker_price_cache");
            e.HasKey(x => x.Ticker);
            e.Property(x => x.Ticker).HasColumnName("ticker");
            e.Property(x => x.Price).HasColumnName("price");
            e.Property(x => x.AsOf).HasColumnName("asof");
            e.Property(x => x.Source).HasColumnName("source");
        });
    }
}