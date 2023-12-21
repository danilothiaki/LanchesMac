using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanchesMac.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]//com essa classe, caso o usuário não estar logado, será redirecionado para a areá de login
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
