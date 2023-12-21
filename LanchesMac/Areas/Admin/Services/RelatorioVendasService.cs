using LanchesMac.Context;
using LanchesMac.Models;
using Microsoft.EntityFrameworkCore;

namespace LanchesMac.Areas.Admin.Services
{
    public class RelatorioVendasService
    {
        private readonly AppDbContext _context;

        public RelatorioVendasService(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<List<Pedido>> FindByDateAsync(DateTime? minDate, DateTime? maxDate)
        {
            var resultado = _context.Pedidos.AsNoTracking().AsQueryable();

            if (minDate.HasValue)
                resultado = resultado.Where(a => a.PedidoEnviado >= minDate.Value);

            if (maxDate.HasValue)
                resultado = resultado.Where(a => a.PedidoEnviado <= maxDate.Value);

            var retorno =  await resultado.Include(a => a.PedidoItens).ThenInclude(a => a.Lanche).OrderByDescending(a => a.PedidoEnviado).ToListAsync();

            return retorno;
        }
    }
}
