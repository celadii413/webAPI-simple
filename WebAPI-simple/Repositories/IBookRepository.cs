using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;

namespace WebAPI_simple.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<BookWithAuthorAndPublisherDTO>> GetAllBooksAsync();
        Task<BookWithAuthorAndPublisherDTO?> GetBookByIdAsync(int id);
        Task<BookWithAuthorAndPublisherDTO> AddBookAsync(AddBookRequestDTO addBookRequestDTO);
        Task<BookWithAuthorAndPublisherDTO?> UpdateBookByIdAsync(int id, AddBookRequestDTO bookDTO);
        Task<bool> DeleteBookByIdAsync(int id);
    }
}

