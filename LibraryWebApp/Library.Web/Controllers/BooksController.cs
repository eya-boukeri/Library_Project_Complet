using Library.BL.DTOs;
using Library.BL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;
        public BooksController(IBookService bookService) => _bookService = bookService;

        public async Task<IActionResult> Index(string? search)
        {
            var books = string.IsNullOrWhiteSpace(search)
                ? await _bookService.GetAllBooksAsync()
                : await _bookService.SearchBooksAsync(search);
            ViewBag.Search = search;
            return View(books);
        }

        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookService.GetBookWithBorrowingsAsync(id);
            if (book == null) return NotFound();
            return View(book);
        }

        public IActionResult Create() => View(new CreateBookDto());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBookDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            await _bookService.CreateBookAsync(dto);
            TempData["Success"] = "Livre ajouté avec succès !";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null) return NotFound();
            var dto = new UpdateBookDto
            {
                Id = book.Id, Title = book.Title, Author = book.Author,
                ISBN = book.ISBN, Genre = book.Genre, PublicationYear = book.PublicationYear,
                Publisher = book.Publisher, Description = book.Description,
                CoverImageUrl = book.CoverImageUrl, IsAvailable = book.IsAvailable
            };
            return View(dto);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateBookDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            await _bookService.UpdateBookAsync(dto);
            TempData["Success"] = "Livre modifié avec succès !";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null) return NotFound();
            return View(book);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _bookService.DeleteBookAsync(id);
            TempData["Success"] = "Livre supprimé.";
            return RedirectToAction(nameof(Index));
        }
    }
}
