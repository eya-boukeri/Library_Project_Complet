using Library.BL.DTOs;
using Library.BL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers
{
    public class BorrowingsController : Controller
    {
        private readonly IBorrowingService _borrowingService;
        private readonly IBookService _bookService;

        public BorrowingsController(IBorrowingService borrowingService, IBookService bookService)
        {
            _borrowingService = borrowingService;
            _bookService = bookService;
        }

        public async Task<IActionResult> Index()
        {
            var borrowings = await _borrowingService.GetAllBorrowingsAsync();
            return View(borrowings);
        }

        public async Task<IActionResult> Active()
        {
            var borrowings = await _borrowingService.GetActiveBorrowingsAsync();
            return View(borrowings);
        }

        public async Task<IActionResult> Overdue()
        {
            var borrowings = await _borrowingService.GetOverdueBorrowingsAsync();
            return View(borrowings);
        }

        public async Task<IActionResult> Create(int? bookId)
        {
            var books = await _bookService.GetAllBooksAsync();
            ViewBag.Books = books.Where(b => b.IsAvailable).ToList();
            var dto = new CreateBorrowingDto
            {
                BookId = bookId ?? 0,
                DueDate = DateTime.Today.AddDays(14)
            };
            return View(dto);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBorrowingDto dto)
        {
            if (!ModelState.IsValid)
            {
                var books = await _bookService.GetAllBooksAsync();
                ViewBag.Books = books.Where(b => b.IsAvailable).ToList();
                return View(dto);
            }
            try
            {
                await _borrowingService.CreateBorrowingAsync(dto);
                TempData["Success"] = "Emprunt enregistré avec succès !";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                var books = await _bookService.GetAllBooksAsync();
                ViewBag.Books = books.Where(b => b.IsAvailable).ToList();
                return View(dto);
            }
        }

        public async Task<IActionResult> Return(int id)
        {
            var borrowing = await _borrowingService.GetBorrowingByIdAsync(id);
            if (borrowing == null) return NotFound();
            return View(borrowing);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int id, string? notes)
        {
            await _borrowingService.ReturnBookAsync(new UpdateBorrowingDto
            {
                Id = id, IsReturned = true, ReturnDate = DateTime.UtcNow, Notes = notes
            });
            TempData["Success"] = "Retour enregistré avec succès !";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _borrowingService.DeleteBorrowingAsync(id);
            TempData["Success"] = "Emprunt supprimé.";
            return RedirectToAction(nameof(Index));
        }
    }
}
