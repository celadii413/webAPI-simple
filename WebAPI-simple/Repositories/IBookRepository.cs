using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;

namespace WebAPI_simple.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<BookWithAuthorAndPublisherDTO>> GetAllBooksAsync(string? filterOn = null, string? filterQuery = null, 
                                                                        string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000);
        Task<BookWithAuthorAndPublisherDTO?> GetBookByIdAsync(int id);
        Task<BookWithAuthorAndPublisherDTO> AddBookAsync(AddBookRequestDTO addBookRequestDTO);
        Task<BookWithAuthorAndPublisherDTO?> UpdateBookByIdAsync(int id, AddBookRequestDTO bookDTO);
        Task<bool> DeleteBookByIdAsync(int id);
    }
}

