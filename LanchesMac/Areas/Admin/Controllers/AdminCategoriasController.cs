using LanchesMac.Context;
using LanchesMac.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LanchesMac.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminCategoriasController : Controller
    {
        private readonly AppDbContext _context;

        public AdminCategoriasController(AppDbContext context)
        {
            _context = context;
        }

        //GET: Admin/AdminCategorias
        //ARRUMAR
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categorias.ToListAsync());
        }

        //GET: Admin/AdminCategorias/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias.FirstOrDefaultAsync(a => a.CategoriaId == id);

            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

        //[HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(categoria);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoria);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (!CategoriaExists(categoria.CategoriaId))
                        return NotFound();
                    else
                        throw;
                }
            }

            return View(categoria);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Categoria categoria)
        {
            try
            {
                if (categoria == null)
                    return NotFound();

                if (ModelState.IsValid)
                {
                    _context.Remove(categoria);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                return View(categoria);
            }
            catch (Exception ex)
            {
                return View(categoria);
            }
        }

        private bool CategoriaExists(int id)
        {
            var categoria = _context.Categorias.Any(a => a.CategoriaId == id);

            return categoria;
        }
    }
}
