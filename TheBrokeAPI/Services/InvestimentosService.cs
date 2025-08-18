using Microsoft.EntityFrameworkCore;
using TheBrokeClub.API.Data;
using TheBrokeClub.API.Infrastructure.Quotes;

namespace TheBrokeClub.API.Application.Services;

public record InvestimentoResumoDto(
    int AtivoId, string Nome, string Ticker,
    decimal Quantidade, decimal PrecoMedio,
    decimal ValorAtual, decimal TotalInvestido,
    decimal GanhoNaoRealizado, decimal GanhoRealizado,
    decimal Percentual, bool IsQuoteStale
);

public interface IInvestimentosService
{
    Task<IReadOnlyList<InvestimentoResumoDto>> GetCardsAsync(int usuarioId, CancellationToken ct);
}

public class InvestimentosService : IInvestimentosService
{
    private readonly AppDbContext _db;
    private readonly IQuoteProvider _quotes;

    public InvestimentosService(AppDbContext db, IQuoteProvider quotes)
    {
        _db = db;
        _quotes = quotes;
    }

    public async Task<IReadOnlyList<InvestimentoResumoDto>> GetCardsAsync(int usuarioId, CancellationToken ct)
    {
        var ativos = await _db.InvestimentoAtivo
            .Where(a => a.UsuarioId == usuarioId)
            .AsNoTracking()
            .ToListAsync(ct);

        var list = new List<InvestimentoResumoDto>(ativos.Count);

        foreach (var a in ativos)
        {
            // ⚠️ use IdUsuario (seu model), não UsuarioIdUsuario
            var txs = await _db.Transacoes
                .Where(t => t.IdUsuario == usuarioId && t.InvestimentoAtivoId == a.Id)
                .AsNoTracking()
                .ToListAsync(ct);

            decimal qtd = 0m, somaPrecoQtd = 0m, somaQtdCompra = 0m;
            decimal totalCompras = 0m, totalVendas = 0m, ganhoRealizado = 0m;

            foreach (var t in txs)
            {
                // força string e normaliza
                var cat = (t.Categoria ?? string.Empty).ToLowerInvariant();

                // heurística simples (ajuste pra seus valores reais)
                var isCompra = cat.Contains("compra") || cat.Contains("buy");
                var isVenda = cat.Contains("venda") || cat.Contains("sell");
                var isDiv = cat.Contains("divid"); // dividendo

                // Se você adicionou Quantidade/PrecoUnit, usa; senão tenta deduzir
                var q = t.Quantidade > 0 ? t.Quantidade
                      : (t.PrecoUnit > 0 ? (t.Valor / t.PrecoUnit) : 0);

                var pu = t.PrecoUnit > 0 ? t.PrecoUnit
                     : (q > 0 ? (t.Valor / q) : 0);

                if (isCompra)
                {
                    qtd += q;
                    somaPrecoQtd += q * pu;
                    somaQtdCompra += q;
                    totalCompras += t.Valor;
                }
                else if (isVenda)
                {
                    var pm = somaQtdCompra > 0 ? (somaPrecoQtd / somaQtdCompra) : 0m;
                    qtd -= q;
                    totalVendas += t.Valor;
                    ganhoRealizado += (pu - pm) * q;
                }
                else if (isDiv)
                {
                    ganhoRealizado += t.Valor;
                }
            }

            var pmFinal = somaQtdCompra > 0 ? (somaPrecoQtd / somaQtdCompra) : 0m;
            var totalInvestido = totalCompras - totalVendas;

            var (precoAtual, _, _, isStale) = await _quotes.GetQuoteAsync(a.Id, a.Ticker, ct);

            var valorAtual = precoAtual * qtd;
            var ganhoNaoRealizado = (precoAtual - pmFinal) * qtd;
            var pct = (pmFinal > 0 && qtd > 0) ? ((precoAtual / pmFinal) - 1m) * 100m : 0m;

            list.Add(new InvestimentoResumoDto(
                a.Id, a.Nome, a.Ticker,
                Quantidade: Math.Round(qtd, 4),
                PrecoMedio: Math.Round(pmFinal, 4),
                ValorAtual: Math.Round(valorAtual, 2),
                TotalInvestido: Math.Round(totalInvestido, 2),
                GanhoNaoRealizado: Math.Round(ganhoNaoRealizado, 2),
                GanhoRealizado: Math.Round(ganhoRealizado, 2),
                Percentual: Math.Round(pct, 2),
                IsQuoteStale: isStale
            ));
        }

        return list;
    }
}