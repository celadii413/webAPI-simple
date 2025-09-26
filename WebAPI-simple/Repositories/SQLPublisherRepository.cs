using Microsoft.EntityFrameworkCore;
using WebAPI_simple.Data;
using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;

namespace WebAPI_simple.Repositories
{
    public class SQLPublisherRepository : IPublisherRepository
    {
        private readonly AppDbContext _dbContext;

        public SQLPublisherRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<PublisherDTO>> GetAllPublishersAsync()
        {
            return await _dbContext.Publishers
                .Select(p => new PublisherDTO
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToListAsync();
        }

        public async Task<PublisherNoIdDTO?> GetPublisherByIdAsync(int id)
        {
            return await _dbContext.Publishers
                .Where(p => p.Id == id)
                .Select(p => new PublisherNoIdDTO
                {
                    Name = p.Name
                })
                .FirstOrDefaultAsync();
        }

        public async Task<AddPublisherRequestDTO> AddPublisherAsync(AddPublisherRequestDTO addPublisherRequestDTO)
        {
            var exists = await _dbContext.Publishers.AnyAsync(p => p.Name == addPublisherRequestDTO.Name);
            if (exists)
                throw new Exception("Tên nhà xuất bản đã tồn tại");

            var publisher = new Publisher { Name = addPublisherRequestDTO.Name };
            _dbContext.Publishers.Add(publisher);
            await _dbContext.SaveChangesAsync();

            return addPublisherRequestDTO;
        }

        public async Task<PublisherNoIdDTO?> UpdatePublisherByIdAsync(int id, PublisherNoIdDTO publisherNoIdDTO)
        {
            var publisher = await _dbContext.Publishers.FirstOrDefaultAsync(p => p.Id == id);
            if (publisher == null) return null;

            publisher.Name = publisherNoIdDTO.Name;
            await _dbContext.SaveChangesAsync();

            return publisherNoIdDTO;
        }

        public async Task<Publisher?> DeletePublisherByIdAsync(int id)
        {
            if (await _dbContext.Books.AnyAsync(b => b.PublisherID == id))
                throw new Exception("Không thể xoá nhà xuất bản vì có sách liên kết");

            var publisher = await _dbContext.Publishers.FirstOrDefaultAsync(p => p.Id == id);
            if (publisher == null) return null;

            _dbContext.Publishers.Remove(publisher);
            await _dbContext.SaveChangesAsync();
            return publisher;
        }
    }
}
