using Library.DAL.Context;
using Library.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.DAL.Repositories
{
    public class BorrowingRepository : IBorrowingRepository
    {
        private readonly LibraryDbContext _context;

        public BorrowingRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Borrowing>> GetAllAsync()
        {
            return await _context.Borrowings
                .Include(b => b.Book)
                .OrderByDescending(b => b.BorrowDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Borrowing>> GetByBookIdAsync(int bookId)
        {
            return await _context.Borrowings
                .Include(b => b.Book)
                .Where(b => b.BookId == bookId)
                .OrderByDescending(b => b.BorrowDate)
                .ToListAsync();
        }

        public async Task<Borrowing?> GetByIdAsync(int id)
        {
            return await _context.Borrowings
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Borrowing> CreateAsync(Borrowing borrowing)
        {
            _context.Borrowings.Add(borrowing);
            await _context.SaveChangesAsync();
            return borrowing;
        }

        public async Task<Borrowing> UpdateAsync(Borrowing borrowing)
        {
            _context.Borrowings.Update(borrowing);
            await _context.SaveChangesAsync();
            return borrowing;
        }

        public async Task DeleteAsync(int id)
        {
            var borrowing = await _context.Borrowings.FindAsync(id);
            if (borrowing != null)
            {
                _context.Borrowings.Remove(borrowing);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Borrowing>> GetActiveBorrowingsAsync()
        {
            return await _context.Borrowings
                .Include(b => b.Book)
                .Where(b => !b.IsReturned)
                .OrderBy(b => b.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Borrowing>> GetOverdueBorrowingsAsync()
        {
            var today = DateTime.UtcNow;
            return await _context.Borrowings
                .Include(b => b.Book)
                .Where(b => !b.IsReturned && b.DueDate < today)
                .OrderBy(b => b.DueDate)
                .ToListAsync();
        }
    }
}
