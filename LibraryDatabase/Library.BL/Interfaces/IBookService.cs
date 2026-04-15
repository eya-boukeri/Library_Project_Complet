using Library.BL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.BL.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllBooksAsync();
        Task<BookDto?> GetBookByIdAsync(int id);
        Task<BookDto?> GetBookWithBorrowingsAsync(int id);
        Task<BookDto> CreateBookAsync(CreateBookDto dto);
        Task<BookDto> UpdateBookAsync(UpdateBookDto dto);
        Task DeleteBookAsync(int id);
        Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm);
        Task<DashboardDto> GetDashboardDataAsync();
    }
}
