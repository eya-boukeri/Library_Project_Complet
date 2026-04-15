using Library.BL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.BL.Interfaces
{
    public interface IBorrowingService
    {
        Task<IEnumerable<BorrowingDto>> GetAllBorrowingsAsync();
        Task<IEnumerable<BorrowingDto>> GetBorrowingsByBookAsync(int bookId);
        Task<BorrowingDto?> GetBorrowingByIdAsync(int id);
        Task<BorrowingDto> CreateBorrowingAsync(CreateBorrowingDto dto);
        Task<BorrowingDto> ReturnBookAsync(UpdateBorrowingDto dto);
        Task DeleteBorrowingAsync(int id);
        Task<IEnumerable<BorrowingDto>> GetActiveBorrowingsAsync();
        Task<IEnumerable<BorrowingDto>> GetOverdueBorrowingsAsync();
    }
}
