using LanchesMac.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LanchesMac.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminImagensController : Controller
    {
        private readonly ConfigurationImagens _myConfig;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AdminImagensController(IOptions<ConfigurationImagens> configurationImagens, IWebHostEnvironment hostEnvironment)
        {
            _myConfig = configurationImagens.Value;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                ViewData["Erro"] = "Erro: Arquivos(s) não selecionado(s)";
                return View(ViewData);
            }

            if (files.Count > 10)
            {
                ViewData["Erro"] = "Erro: Quantidade de arquivos excedeu o limite";
                return View(ViewData);
            }

            var size = files.Sum(a => a.Length);

            var filePathsName = new List<string>();

            var filePath = Path.Combine(_hostEnvironment.WebRootPath, _myConfig.NomePastaImagensProdutos);

            foreach (var itemFormFile in files)
            {
                if (itemFormFile.FileName.Contains(".jpg") || itemFormFile.FileName.Contains(".gif") || itemFormFile.FileName.Contains(".png"))
                {
                    var fileNameWithPath = string.Concat(filePath, "\\", itemFormFile.FileName);

                    filePathsName.Add(fileNameWithPath);

                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        await itemFormFile.CopyToAsync(stream);
                    }
                }
            }

            ViewData["Resultado"] = $"{files.Count} arquivos foram enviados ao servidor, " + $"com tamanho total de : {size} bytes";

            ViewBag.Arquivos = filePathsName;

            return View(ViewData);
        }

        public IActionResult GetImagens()
        {
            FileManagerModel model = new FileManagerModel();

            var userImagesPath = Path.Combine(_hostEnvironment.WebRootPath, _myConfig.NomePastaImagensProdutos);

            DirectoryInfo dir = new DirectoryInfo(userImagesPath);

            FileInfo[] files = dir.GetFiles();

            model.PathImagesProduto = _myConfig.NomePastaImagensProdutos;

            if (files.Length == 0)
            {
                ViewData["Erro"] = $"Nenhum arquivo encontrado na pasta {userImagesPath}";
            }

            model.Files = files;

            return View(model);
        }

        public IActionResult Deletefile (string fname)
        {
            string _imagemDelete = Path.Combine(_hostEnvironment.WebRootPath, _myConfig.NomePastaImagensProdutos + "\\", fname);

            if (System.IO.File.Exists(_imagemDelete))
            {
                System.IO.File.Delete(_imagemDelete);

                ViewData["Deletado"] = $"Arquivo(s) {_imagemDelete} deletado com sucesso";
            }

            return View("Index");
        }
    }
}
