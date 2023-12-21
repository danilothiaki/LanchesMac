using LanchesMac.Context;
using LanchesMac.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;

namespace LanchesMac.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminLanchesController : Controller
    {
        private readonly AppDbContext _context;

        public AdminLanchesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string filter, int pageindex = 1, string sort = "Nome")
        {
            var resultado = _context.Lanches.Include(a => a.Categoria).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
                resultado = resultado.Where(a => a.Nome.Contains(filter));

            var model = await PagingList.CreateAsync(resultado, 5, pageindex, sort, "Nome");
            model.RouteValue = new RouteValueDictionary { { "filter", filter } };
            return View(model);

            //return View(await _context.Lanches.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var lanche = await _context.Lanches.FirstOrDefaultAsync(a => a.LancheId == id);

            return View(lanche);
        }

        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "CategoriaNome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Lanche lanche) 
        {
            if (ModelState.IsValid)
            {
                _context.Lanches.Add(lanche);
                _context.SaveChanges();
                //return View(lanche);
                //VERIFICAR PRA QUAL PÁGINA IR
                return RedirectToAction(nameof(Index));
            }

            return View(lanche);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var lanche = await _context.Lanches.FindAsync(id);

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "CategoriaNome", lanche.CategoriaId);
            return View(lanche);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Lanche lanche)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lanche);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch(Exception ex)
                {
                    if (!LancheExists(lanche.LancheId))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "CategoriaNome", lanche.CategoriaId);
            return View(lanche);
        }

        //teste sem async
        public IActionResult Delete(int? id) 
        {
            if (id == null)
                return NotFound();

            Lanche lanche = _context.Lanches.FirstOrDefault(a => a.LancheId == id);

            return View(lanche);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Delete(int? id, Lanche lanche)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Remove(lanche);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));
        //        }
        //        catch (Exception ex)
        //        {
        //            throw;
        //        }
        //    }

        //    return View(lanche);
        //}

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var lanche = await _context.Lanches.FindAsync(id);
            _context.Lanches.Remove(lanche);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LancheExists (int id)
        {
            return _context.Lanches.Any(a => a.LancheId == id);
        }
    }
}
