using Library.BL.DTOs;
using Library.BL.Interfaces;
using Library.DAL.Models;
using Library.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.BL.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepo;
        private readonly IBorrowingRepository _borrowingRepo;

        public BookService(IBookRepository bookRepo, IBorrowingRepository borrowingRepo)
        {
            _bookRepo = bookRepo;
            _borrowingRepo = borrowingRepo;
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
        {
            var books = await _bookRepo.GetAllAsync();
            return books.Select(MapToDto);
        }

        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            var book = await _bookRepo.GetByIdAsync(id);
            return book == null ? null : MapToDto(book);
        }

        public async Task<BookDto?> GetBookWithBorrowingsAsync(int id)
        {
            var book = await _bookRepo.GetByIdWithBorrowingsAsync(id);
            if (book == null) return null;
            var dto = MapToDto(book);
            dto.BorrowingsCount = book.Borrowings.Count;
            return dto;
        }

        public async Task<BookDto> CreateBookAsync(CreateBookDto dto)
        {
            var book = new Book
            {
                Title = dto.Title,
                Author = dto.Author,
                ISBN = dto.ISBN,
                Genre = dto.Genre,
                PublicationYear = dto.PublicationYear,
                Publisher = dto.Publisher,
                Description = dto.Description,
                CoverImageUrl = dto.CoverImageUrl,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            };
            var created = await _bookRepo.CreateAsync(book);
            return MapToDto(created);
        }

        public async Task<BookDto> UpdateBookAsync(UpdateBookDto dto)
        {
            var book = await _bookRepo.GetByIdAsync(dto.Id)
                ?? throw new KeyNotFoundException($"Livre #{dto.Id} introuvable");

            book.Title = dto.Title;
            book.Author = dto.Author;
            book.ISBN = dto.ISBN;
            book.Genre = dto.Genre;
            book.PublicationYear = dto.PublicationYear;
            book.Publisher = dto.Publisher;
            book.Description = dto.Description;
            book.CoverImageUrl = dto.CoverImageUrl;
            book.IsAvailable = dto.IsAvailable;

            var updated = await _bookRepo.UpdateAsync(book);
            return MapToDto(updated);
        }

        public async Task DeleteBookAsync(int id)
        {
            if (!await _bookRepo.ExistsAsync(id))
                throw new KeyNotFoundException($"Livre #{id} introuvable");
            await _bookRepo.DeleteAsync(id);
        }

        public async Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm)
        {
            var books = await _bookRepo.SearchAsync(searchTerm);
            return books.Select(MapToDto);
        }

        public async Task<DashboardDto> GetDashboardDataAsync()
        {
            var allBooks = (await _bookRepo.GetAllAsync()).ToList();
            var activeBorrowings = (await _borrowingRepo.GetActiveBorrowingsAsync()).ToList();
            var overdue = (await _borrowingRepo.GetOverdueBorrowingsAsync()).ToList();
            var allBorrowings = (await _borrowingRepo.GetAllAsync()).ToList();

            return new DashboardDto
            {
                TotalBooks = allBooks.Count,
                AvailableBooks = allBooks.Count(b => b.IsAvailable),
                ActiveBorrowings = activeBorrowings.Count,
                OverdueBorrowings = overdue.Count,
                RecentBorrowings = allBorrowings.Take(5).Select(MapBorrowingToDto).ToList(),
                RecentBooks = allBooks.OrderByDescending(b => b.CreatedAt).Take(4).Select(MapToDto).ToList()
            };
        }

        private static BookDto MapToDto(Book b) => new()
        {
            Id = b.Id,
            Title = b.Title,
            Author = b.Author,
            ISBN = b.ISBN,
            Genre = b.Genre,
            PublicationYear = b.PublicationYear,
            Publisher = b.Publisher,
            Description = b.Description,
            IsAvailable = b.IsAvailable,
            CreatedAt = b.CreatedAt,
            CoverImageUrl = b.CoverImageUrl
        };

        private static BorrowingDto MapBorrowingToDto(DAL.Models.Borrowing br) => new()
        {
            Id = br.Id,
            BookId = br.BookId,
            BookTitle = br.Book?.Title ?? "",
            BookAuthor = br.Book?.Author ?? "",
            BorrowerName = br.BorrowerName,
            BorrowerEmail = br.BorrowerEmail,
            BorrowDate = br.BorrowDate,
            ReturnDate = br.ReturnDate,
            DueDate = br.DueDate,
            IsReturned = br.IsReturned,
            Notes = br.Notes
        };
    }
}
