using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;

namespace WebAPI_simple.Repositories
{
    public interface IAuthorRepository
    {
        Task<List<AuthorDTO>> GetAllAuthorsAsync();
        Task<AuthorNoIdDTO?> GetAuthorByIdAsync(int id);
        Task<AuthorNoIdDTO> AddAuthorAsync(AddAuthorRequestDTO addAuthorRequestDTO);
        Task<AuthorNoIdDTO?> UpdateAuthorByIdAsync(int id, AuthorNoIdDTO authorNoIdDTO);
        Task<Author?> DeleteAuthorByIdAsync(int id);
    }
}
