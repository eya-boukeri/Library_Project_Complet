using Library.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.DAL.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync();
        Task<Book?> GetByIdAsync(int id);
        Task<Book?> GetByIdWithBorrowingsAsync(int id);
        Task<Book> CreateAsync(Book book);
        Task<Book> UpdateAsync(Book book);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<Book>> SearchAsync(string searchTerm);
    }
}
