using Library.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.DAL.Repositories
{
    public interface IBorrowingRepository
    {
        Task<IEnumerable<Borrowing>> GetAllAsync();
        Task<IEnumerable<Borrowing>> GetByBookIdAsync(int bookId);
        Task<Borrowing?> GetByIdAsync(int id);
        Task<Borrowing> CreateAsync(Borrowing borrowing);
        Task<Borrowing> UpdateAsync(Borrowing borrowing);
        Task DeleteAsync(int id);
        Task<IEnumerable<Borrowing>> GetActiveBorrowingsAsync();
        Task<IEnumerable<Borrowing>> GetOverdueBorrowingsAsync();
    }
}
