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
    public class BorrowingService : IBorrowingService
    {
        private readonly IBorrowingRepository _borrowingRepo;
        private readonly IBookRepository _bookRepo;

        public BorrowingService(IBorrowingRepository borrowingRepo, IBookRepository bookRepo)
        {
            _borrowingRepo = borrowingRepo;
            _bookRepo = bookRepo;
        }

        public async Task<IEnumerable<BorrowingDto>> GetAllBorrowingsAsync()
        {
            var list = await _borrowingRepo.GetAllAsync();
            return list.Select(MapToDto);
        }

        public async Task<IEnumerable<BorrowingDto>> GetBorrowingsByBookAsync(int bookId)
        {
            var list = await _borrowingRepo.GetByBookIdAsync(bookId);
            return list.Select(MapToDto);
        }

        public async Task<BorrowingDto?> GetBorrowingByIdAsync(int id)
        {
            var b = await _borrowingRepo.GetByIdAsync(id);
            return b == null ? null : MapToDto(b);
        }

        public async Task<BorrowingDto> CreateBorrowingAsync(CreateBorrowingDto dto)
        {
            var book = await _bookRepo.GetByIdAsync(dto.BookId)
                ?? throw new KeyNotFoundException($"Livre #{dto.BookId} introuvable");

            if (!book.IsAvailable)
                throw new InvalidOperationException("Ce livre n'est pas disponible pour l'emprunt.");

            var borrowing = new Borrowing
            {
                BookId = dto.BookId,
                BorrowerName = dto.BorrowerName,
                BorrowerEmail = dto.BorrowerEmail,
                BorrowDate = DateTime.UtcNow,
                DueDate = dto.DueDate,
                Notes = dto.Notes,
                IsReturned = false
            };

            var created = await _borrowingRepo.CreateAsync(borrowing);

            // Mark book as unavailable
            book.IsAvailable = false;
            await _bookRepo.UpdateAsync(book);

            // Reload with nav prop
            var full = await _borrowingRepo.GetByIdAsync(created.Id);
            return MapToDto(full!);
        }

        public async Task<BorrowingDto> ReturnBookAsync(UpdateBorrowingDto dto)
        {
            var borrowing = await _borrowingRepo.GetByIdAsync(dto.Id)
                ?? throw new KeyNotFoundException($"Emprunt #{dto.Id} introuvable");

            borrowing.IsReturned = dto.IsReturned;
            borrowing.ReturnDate = dto.IsReturned ? (dto.ReturnDate ?? DateTime.UtcNow) : null;
            borrowing.Notes = dto.Notes;

            var updated = await _borrowingRepo.UpdateAsync(borrowing);

            // Mark book as available again
            if (dto.IsReturned)
            {
                var book = await _bookRepo.GetByIdAsync(borrowing.BookId);
                if (book != null)
                {
                    book.IsAvailable = true;
                    await _bookRepo.UpdateAsync(book);
                }
            }

            return MapToDto(updated);
        }

        public async Task DeleteBorrowingAsync(int id)
        {
            await _borrowingRepo.DeleteAsync(id);
        }

        public async Task<IEnumerable<BorrowingDto>> GetActiveBorrowingsAsync()
        {
            var list = await _borrowingRepo.GetActiveBorrowingsAsync();
            return list.Select(MapToDto);
        }

        public async Task<IEnumerable<BorrowingDto>> GetOverdueBorrowingsAsync()
        {
            var list = await _borrowingRepo.GetOverdueBorrowingsAsync();
            return list.Select(MapToDto);
        }

        private static BorrowingDto MapToDto(Borrowing b) => new()
        {
            Id = b.Id,
            BookId = b.BookId,
            BookTitle = b.Book?.Title ?? "",
            BookAuthor = b.Book?.Author ?? "",
            BorrowerName = b.BorrowerName,
            BorrowerEmail = b.BorrowerEmail,
            BorrowDate = b.BorrowDate,
            ReturnDate = b.ReturnDate,
            DueDate = b.DueDate,
            IsReturned = b.IsReturned,
            Notes = b.Notes
        };
    }
}
