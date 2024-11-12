using Bookstore.Data;
using Bookstore.Models;
using Bookstore.Services;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bookstore.Controllers
{
    public class GenresController : Controller
    {
        private readonly GenreService _service;

        public GenresController(GenreService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _service.FindAllAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Genre genre)
        {
            if (!ModelState.IsValid) 
            {
                return View();
            }
            await _service.InsertAsync(genre);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return RedirectToAction(nameof(Error), new { message = "id não fornecido" });
            }

            var obj = await _service.FindByIdAsync(id.Value);
            if (obj is null)
            {
                return RedirectToAction(nameof(Error), new { message = "id não encontrado" });
            }
            return View(obj);
        }
    }
}
