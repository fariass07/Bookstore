﻿using Bookstore.Models;
using Bookstore.Models.ViewModels;
using Bookstore.Services;
using Bookstore.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Bookstore.Controllers
{
    public class SaleController : Controller
    {
        private readonly SaleService _service;
        private readonly BookService _bookService;
        private readonly SellerService _sellerService;

        public SaleController(SaleService service, BookService bookService, SellerService sellerService)
        {
            _service = service;
            _bookService = bookService;
            _sellerService = sellerService;
        }

        public async Task<IActionResult> Index(string sortOrder)
        {
            List<Sales> sales = await _service.FindAllAsync();

            ViewData["DateSortParam"] = sortOrder == "date_asc" ? "date_desc" : "date_asc";
            ViewData["AmountSortParam"] = sortOrder == "amount_asc" ? "amount_desc" : "amount_asc";
            ViewData["SellerSortParam"] = sortOrder == "seller_asc" ? "seller_desc" : "seller_asc";


            switch (sortOrder)
            {
                case "date_desc":
                    sales = sales.OrderByDescending(x => x.Date).ToList();
                    break;
                case "date_asc":
                    sales = sales.OrderBy(x => x.Date).ToList();
                    break;
                case "amount_desc":
                    sales = sales.OrderByDescending(x => x.Amount).ToList();
                    break;
                case "amount_asc":
                    sales = sales.OrderBy(x => x.Amount).ToList();
                    break;
                case "seller_desc":
                    sales = sales.OrderByDescending(x => x.Seller.Name).ToList();
                    break;
                case "seller_asc":
                    sales = sales.OrderBy(x => x.Seller.Name).ToList();
                    break;
                default:
                    sales = sales.OrderByDescending(x => x.Date).ToList();
                    break;
            }

            return View(sales);
        }
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não fornecido" });
            }
            var obj = await _service.FindByIdEagerAsync(id.Value);
            if (obj is null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não encontrado" });
            }

            return View(obj);
        }

        public async Task<IActionResult> Create()
        {
            List<Book> books = await _bookService.FindAllAsync();
            List<Seller> sellers = await _sellerService.FindAllAsync();


            SaleFormViewModel viewModel = new SaleFormViewModel { Books = books, Sellers = sellers };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaleFormViewModel viewModel)
        {
            if (!ModelState.IsValid && ModelState["Sale.Seller"].RawValue is not null)
            {
                List<Book> books = await _bookService.FindAllAsync();
                List<Seller> sellers = await _sellerService.FindAllAsync();
                viewModel.Books = books;
                viewModel.Sellers = sellers;
                return View(viewModel);
            }

            foreach (int id in viewModel.SelectedBooksIds)
            {
                Book book = await _bookService.FindByIdAsync(id);

                if (book is not null)
                {
                    viewModel.Sale.Books.Add(book);
                }
            }

            Seller seller = await _sellerService.FindByIdAsync(viewModel.SelectedSellerId);

            if (seller is not null)
            {
                viewModel.Sale.Seller = seller;
            }
            else
            {
                List<Book> books = await _bookService.FindAllAsync();
                List<Seller> sellers = await _sellerService.FindAllAsync();
                viewModel.Books = books;
                viewModel.Sellers = sellers;
                return View(viewModel);
            }

            await _service.InsertAsync(viewModel.Sale);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não encontrado" });
            }
            Sales obj = await _service.FindByIdAsync(id.Value);
            if (obj is null)
            {
                return RedirectToAction(nameof(Error), new { message = "Venda não encontrada" });
            }

            List<Book> books = await _bookService.FindAllAsync();
            List<Seller> sellers = await _sellerService.FindAllAsync();

            SaleFormViewModel viewModel = new SaleFormViewModel { Sale = obj, Books = books, Sellers = sellers };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SaleFormViewModel viewModel)
        {
            if (!ModelState.IsValid && ModelState["Sale.Seller"].RawValue is not null)
            {
                List<Book> books = await _bookService.FindAllAsync();
                List<Seller> sellers = await _sellerService.FindAllAsync();
                viewModel.Books = books;
                viewModel.Sellers = sellers;
                return View(viewModel);
            }

            if (id != viewModel.Sale.Id)
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

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não fornecido" });
            }

            var obj = await _service.FindByIdEagerAsync(id.Value);
            if (obj is null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não encontrado" });
            }

            return View(obj);
        }

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