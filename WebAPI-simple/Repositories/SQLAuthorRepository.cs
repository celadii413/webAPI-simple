using Microsoft.EntityFrameworkCore;
using WebAPI_simple.Data;
using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;

namespace WebAPI_simple.Repositories
{
    public class SQLAuthorRepository : IAuthorRepository
    {
        private readonly AppDbContext _dbContext;

        public SQLAuthorRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Lấy toàn bộ Authors
        public async Task<List<AuthorDTO>> GetAllAuthorsAsync()
        {
            return await _dbContext.Authors
                .Select(a => new AuthorDTO
                {
                    Id = a.Id,
                    FullName = a.FullName
                })
                .ToListAsync();
        }

        // Lấy Author theo Id
        public async Task<AuthorNoIdDTO?> GetAuthorByIdAsync(int id)
        {
            var author = await _dbContext.Authors.FirstOrDefaultAsync(a => a.Id == id);

            if (author == null) return null;

            return new AuthorNoIdDTO
            {
                FullName = author.FullName
            };
        }

        // Thêm mới Author
        public async Task<AuthorNoIdDTO> AddAuthorAsync(AddAuthorRequestDTO addAuthorRequestDTO)
        {
            var author = new Author
            {
                FullName = addAuthorRequestDTO.FullName
            };

            await _dbContext.Authors.AddAsync(author);
            await _dbContext.SaveChangesAsync();

            return new AuthorNoIdDTO
            {
                FullName = author.FullName
            };
        }

        // Cập nhật Author
        public async Task<AuthorNoIdDTO?> UpdateAuthorByIdAsync(int id, AuthorNoIdDTO authorNoIdDTO)
        {
            var existingAuthor = await _dbContext.Authors.FirstOrDefaultAsync(a => a.Id == id);

            if (existingAuthor == null) return null;

            existingAuthor.FullName = authorNoIdDTO.FullName;

            await _dbContext.SaveChangesAsync();

            return new AuthorNoIdDTO
            {
                FullName = existingAuthor.FullName
            };
        }

        // Xóa Author
        public async Task<Author?> DeleteAuthorByIdAsync(int id)
        {
            var existingAuthor = await _dbContext.Authors.FirstOrDefaultAsync(a => a.Id == id);

            if (existingAuthor == null) return null;

            _dbContext.Authors.Remove(existingAuthor);
            await _dbContext.SaveChangesAsync();

            return existingAuthor;
        }
    }
}
