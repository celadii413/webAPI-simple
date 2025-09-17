using Microsoft.EntityFrameworkCore;
using WebAPI_simple.Data;
using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;

namespace WebAPI_simple.Repositories
{
    public class SQLBookRepository : IBookRepository
    {
        private readonly AppDbContext _dbContext;
        public SQLBookRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<BookWithAuthorAndPublisherDTO>> GetAllBooksAsync()
        {
            return await _dbContext.Books
                .Select(book => new BookWithAuthorAndPublisherDTO()
                {
                    Id = book.Id,
                    Title = book.Title,
                    Description = book.Description,
                    IsRead = book.IsRead,
                    DateRead = book.IsRead ? book.DateRead.Value : null,
                    Rate = book.IsRead ? book.Rate.Value : null,
                    Genre = book.Genre,
                    CoverUrl = book.CoverUrl,
                    PublisherName = book.Publisher.Name,
                    AuthorNames = book.Book_Authors.Select(n => n.Author.FullName).ToList()
                })
                .ToListAsync();
        }

        public async Task<BookWithAuthorAndPublisherDTO?> GetBookByIdAsync(int id)
        {
            return await _dbContext.Books
                .Where(b => b.Id == id)
                .Select(book => new BookWithAuthorAndPublisherDTO()
                {
                    Id = book.Id,
                    Title = book.Title,
                    Description = book.Description,
                    IsRead = book.IsRead,
                    DateRead = book.DateRead,
                    Rate = book.Rate,
                    Genre = book.Genre,
                    CoverUrl = book.CoverUrl,
                    PublisherName = book.Publisher.Name,
                    AuthorNames = book.Book_Authors.Select(n => n.Author.FullName).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<BookWithAuthorAndPublisherDTO> AddBookAsync(AddBookRequestDTO addBookRequestDTO)
        {
            var bookDomainModel = new Book
            {
                Title = addBookRequestDTO.Title,
                Description = addBookRequestDTO.Description,
                IsRead = addBookRequestDTO.IsRead,
                DateRead = addBookRequestDTO.DateRead,
                Rate = addBookRequestDTO.Rate,
                Genre = addBookRequestDTO.Genre,
                CoverUrl = addBookRequestDTO.CoverUrl,
                DateAdded = addBookRequestDTO.DateAdded,
                PublisherID = addBookRequestDTO.PublisherID
            };

            _dbContext.Books.Add(bookDomainModel);
            await _dbContext.SaveChangesAsync();

            foreach (var id in addBookRequestDTO.AuthorIds)
            {
                var bookAuthor = new Book_Author()
                {
                    BookId = bookDomainModel.Id,
                    AuthorId = id
                };
                _dbContext.Book_Authors.Add(bookAuthor);
            }

            await _dbContext.SaveChangesAsync();

            return new BookWithAuthorAndPublisherDTO
            {
                Id = bookDomainModel.Id,
                Title = bookDomainModel.Title,
                Description = bookDomainModel.Description,
                IsRead = bookDomainModel.IsRead,
                DateRead = bookDomainModel.DateRead,
                Rate = bookDomainModel.Rate,
                Genre = bookDomainModel.Genre,
                CoverUrl = bookDomainModel.CoverUrl,
                PublisherName = (await _dbContext.Publishers.FindAsync(bookDomainModel.PublisherID))?.Name ?? "",
                AuthorNames = await _dbContext.Book_Authors
                    .Where(b => b.BookId == bookDomainModel.Id)
                    .Select(b => b.Author.FullName)
                    .ToListAsync()
            };
        }

        public async Task<BookWithAuthorAndPublisherDTO?> UpdateBookByIdAsync(int id, AddBookRequestDTO bookDTO)
        {
            var bookDomain = await _dbContext.Books.FirstOrDefaultAsync(n => n.Id == id);
            if (bookDomain == null) return null;

            bookDomain.Title = bookDTO.Title;
            bookDomain.Description = bookDTO.Description;
            bookDomain.IsRead = bookDTO.IsRead;
            bookDomain.DateRead = bookDTO.DateRead;
            bookDomain.Rate = bookDTO.Rate;
            bookDomain.Genre = bookDTO.Genre;
            bookDomain.CoverUrl = bookDTO.CoverUrl;
            bookDomain.DateAdded = bookDTO.DateAdded;
            bookDomain.PublisherID = bookDTO.PublisherID;
            await _dbContext.SaveChangesAsync();

            var authorDomain = await _dbContext.Book_Authors.Where(a => a.BookId == id).ToListAsync();
            _dbContext.Book_Authors.RemoveRange(authorDomain);
            await _dbContext.SaveChangesAsync();

            foreach (var authorId in bookDTO.AuthorIds)
            {
                var bookAuthor = new Book_Author()
                {
                    BookId = id,
                    AuthorId = authorId
                };
                _dbContext.Book_Authors.Add(bookAuthor);
            }

            await _dbContext.SaveChangesAsync();

            return await GetBookByIdAsync(id);
        }

        public async Task<bool> DeleteBookByIdAsync(int id)
        {
            var bookDomain = await _dbContext.Books.FirstOrDefaultAsync(n => n.Id == id);
            if (bookDomain == null) return false;

            _dbContext.Books.Remove(bookDomain);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
