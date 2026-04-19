using Library.BL.DTOs;
using Library.BL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Library.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IWebHostEnvironment _environment;

        public BooksController(IBookService bookService, IWebHostEnvironment environment)
        {
            _bookService = bookService;
            _environment = environment;
        }

        public async Task<IActionResult> Index(string? search, string? letter)
        {
            var normalizedLetter = string.IsNullOrWhiteSpace(letter) ? null : letter.Trim().Substring(0, 1).ToUpperInvariant();

            var books = !string.IsNullOrWhiteSpace(normalizedLetter)
                ? await _bookService.GetBooksByInitialAsync(normalizedLetter)
                : string.IsNullOrWhiteSpace(search)
                ? await _bookService.GetAllBooksAsync()
                : await _bookService.SearchBooksAsync(search);

            ViewBag.Search = search;
            ViewBag.CurrentLetter = normalizedLetter;
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

            dto.CoverImageUrl = await SaveCoverImageAsync(dto.CoverImageFile);
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

            if (dto.CoverImageFile is { Length: > 0 })
            {
                var previousCover = (await _bookService.GetBookByIdAsync(dto.Id))?.CoverImageUrl;
                dto.CoverImageUrl = await SaveCoverImageAsync(dto.CoverImageFile);
                if (!ModelState.IsValid) return View(dto);
                DeleteCoverImage(previousCover);
            }

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

        private async Task<string?> SaveCoverImageAsync(IFormFile? coverImageFile)
        {
            if (coverImageFile is not { Length: > 0 })
            {
                return null;
            }

            var extension = Path.GetExtension(coverImageFile.FileName).ToLowerInvariant();
            var allowedExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".jfif", ".bmp", ".avif" };

            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError(nameof(CreateBookDto.CoverImageFile), $"Format non supporté ({extension}). Utilisez: JPG, JPEG, PNG, WEBP, GIF, JFIF, BMP ou AVIF.");
                return null;
            }

            var coversDirectory = Path.Combine(_environment.WebRootPath, "images", "covers");
            Directory.CreateDirectory(coversDirectory);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(coversDirectory, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await coverImageFile.CopyToAsync(stream);

            return $"~/images/covers/{fileName}";
        }

        private void DeleteCoverImage(string? coverImageUrl)
        {
            if (string.IsNullOrWhiteSpace(coverImageUrl))
            {
                return;
            }

            var relativePath = coverImageUrl
                .TrimStart('~')
                .TrimStart('/')
                .Replace('/', Path.DirectorySeparatorChar);

            if (!relativePath.StartsWith($"images{Path.DirectorySeparatorChar}covers{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var absolutePath = Path.Combine(_environment.WebRootPath, relativePath);

            if (System.IO.File.Exists(absolutePath))
            {
                System.IO.File.Delete(absolutePath);
            }
        }
    }
}
