using Microsoft.AspNetCore.Mvc;
using Bookstore.Models;
using Bookstore.Services.Exceptions;
using Bookstore.Models.ViewModels;
using System.Diagnostics;
using Bookstore.Services;

namespace Bookstore.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookService _service;
        private readonly GenreService _genreService;

        public BooksController(BookService service, GenreService genreService)
        {
            _service = service;
            _genreService = genreService;
        }

        public async Task<IActionResult> Index()
		{
			return View(await _service.FindAllAsync());
		}


		public async Task<IActionResult> Details(int? id)
		{
			if (id is null)
			{
				return RedirectToAction(nameof(Error), new { message = "Id não fornecido" });
			}

			var obj = await _service.FindByIdAsync(id.Value);
			if (obj is null)
			{
				return RedirectToAction(nameof(Error), new { message = "Id não encontrado" });
			}

			return View(obj);
		}

        public async Task<IActionResult> Create()
        {
            List<Genre> genres = await _genreService.FindAllAsync();

            BookFormViewModel viewModel = new BookFormViewModel { Genres = genres };
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Genres = await _genreService.FindAllAsync();
                return View(viewModel);
            }

            viewModel.Book.Genres = new List<Genre>();

            foreach (int genreId in viewModel.SelectedGenresIds)
            {
                Genre genre = await _genreService.FindByIdAsync(genreId);
                if (genre is not null)
                {
                    viewModel.Book.Genres.Add(genre);
                }
            }
            await _service.InsertAsync(viewModel.Book);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não fornecido" });
            }

            var obj = await _service.FindByIdAsync(id.Value);
            if (obj is null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não encontrado" });
            }

            List<Genre> genres = await _genreService.FindAllAsync();
            BookFormViewModel viewModel = new BookFormViewModel { Book = obj, Genres = genres };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (id != viewModel.Book.Id)
            {
                return RedirectToAction(nameof(Error), new { message = "Id's não condizentes" });
            }

            try
            {
                await _service.UpdateAsync(viewModel);
                return RedirectToAction(nameof(Index));
            }
            catch (ApplicationException ex)
            {
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
		{
			if (id is null)
			{
				return RedirectToAction(nameof(Error), new { message = "Id não fornecido" });
			}

			var obj = await _service.FindByIdAsync(id.Value);
			if (obj is null)
			{
				return RedirectToAction(nameof(Error), new { message = "Id não encontrado" });
			}

			return View(obj);
		}

		// POST: Books/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				await _service.RemoveAsync(id);
				return RedirectToAction(nameof(Index));
			}
			catch (IntegrityException ex)
			{
				return RedirectToAction(nameof(Error), new { message = ex.Message });
			}
		}

		public IActionResult Error(string message)
		{
			var viewModel = new ErrorViewModel
			{
				Message = message,
				RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
			};
			return View(viewModel);
		}
	}
}
