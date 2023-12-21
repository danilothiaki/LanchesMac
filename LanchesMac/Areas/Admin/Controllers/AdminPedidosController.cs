using LanchesMac.Context;
using LanchesMac.Models;
using LanchesMac.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;

namespace LanchesMac.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminPedidosController : Controller
    {
        private readonly AppDbContext _context;

        public AdminPedidosController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult PedidoLanches(int? id)
        {
            var pedido = _context.Pedidos.Include(a => a.PedidoItens).ThenInclude(a => a.Lanche).FirstOrDefault(a => a.PedidoId == id);

            if (pedido == null)
            {
                Response.StatusCode = 404;
                return View("PedidoNotFound", id.Value);
            }

            PedidoLancheViewModel pedidoLanches = new PedidoLancheViewModel()
            {
                Pedido = pedido,
                PedidoDetalhes = pedido.PedidoItens
            };
            return View(pedidoLanches);
        }

        public async Task<IActionResult> Index(string filter, int pageindex = 1, string sort = "Nome")
        {
            //return View(await _context.Pedidos.ToListAsync());
            var resultado = _context.Pedidos.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
                resultado = resultado.Where(p => p.Nome == filter);

            var model = await PagingList.CreateAsync(resultado, 5, pageindex, sort, "Nome");
            model.RouteValue = new RouteValueDictionary { { "filter", filter } };

            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var pedido = await _context.Pedidos.FirstOrDefaultAsync(a => a.PedidoId == id);

            return View(pedido);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                _context.Pedidos.Add(pedido);
                _context.SaveChanges();
                return View(pedido);
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            Pedido pedido = _context.Pedidos.FirstOrDefault(a => a.PedidoId == id);

            return View(pedido);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Pedidos.Update(pedido);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (!PedidoExists(pedido.PedidoId))
                        return NotFound();
                    else
                        throw;
                }
            }

            return View(pedido);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            Pedido pedido = _context.Pedidos.FirstOrDefault(a => a.PedidoId == id);
            if (pedido == null)
                return NotFound();

            return View(pedido);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Pedidos.Remove(pedido);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (!PedidoExists(pedido.PedidoId))
                        return NotFound();
                    else
                        throw;
                }
            }
            return View();
        }

        private bool PedidoExists(int id)
        {
            return _context.Pedidos.Any(a => a.PedidoId == id);
        }
    }
}
