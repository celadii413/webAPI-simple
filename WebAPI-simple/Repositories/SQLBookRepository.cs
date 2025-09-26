using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
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
            // Kiểm tra Title không được để trống và không chứa ký tự đặc biệt
            if (string.IsNullOrWhiteSpace(addBookRequestDTO.Title) || !System.Text.RegularExpressions.Regex.IsMatch(addBookRequestDTO.Title, @"^[\p{L}\p{N}\p{P}\p{Zs}]+$"))
            {
                throw new Exception("Book Title cannot be empty or contain special characters");
            }

            // Nhà xuất bản phải tồn tại
            if (!await _dbContext.Publishers.AnyAsync(p => p.Id == addBookRequestDTO.PublisherID))
                throw new Exception("PublisherId does not exist");

            // NXB không được xuất bản 2 sách cùng tên
            if (await _dbContext.Books.AnyAsync(b => b.PublisherID == addBookRequestDTO.PublisherID && b.Title == addBookRequestDTO.Title))
                throw new Exception("This publisher already has a book with the same title");

            // Nhà xuất bản tối đa 100 sách 1 năm
            int year = DateTime.Now.Year;
            int publishedCount = await _dbContext.Books.CountAsync(b => b.PublisherID == addBookRequestDTO.PublisherID && b.DateAdded.Year == year);
            if (publishedCount >= 100)
                throw new Exception("Publisher cannot publish more than 100 books in a year");

            // Một sách có ít nhất 1 tác giả
            if (addBookRequestDTO.AuthorIds == null || !addBookRequestDTO.AuthorIds.Any())
                throw new Exception("A book must have at least 1 author");

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

            foreach (var authorId in addBookRequestDTO.AuthorIds)
            {
                // kiểm tra tác giả tồn tại
                if (!await _dbContext.Authors.AnyAsync(a => a.Id == authorId))
                    throw new Exception($"AuthorId {authorId} does not exist");

                // Kiểm tra trùng lặp tác giả
                if (await _dbContext.Book_Authors.AnyAsync(ba => ba.BookId == bookDomainModel.Id && ba.AuthorId == authorId))
                    throw new Exception("This Author is already assigned to the Book");

                // Tác giả không thể viết hơn 20 sách
                int bookCount = await _dbContext.Book_Authors.CountAsync(ba => ba.AuthorId == authorId);
                if (bookCount >= 20)
                    throw new Exception($"Author {authorId} cannot write more than 20 books");

                _dbContext.Book_Authors.Add(new Book_Author { BookId = bookDomainModel.Id, AuthorId = authorId });
            }

            await _dbContext.SaveChangesAsync();

            return await GetBookByIdAsync(bookDomainModel.Id) ?? throw new Exception("Book was not saved correctly");
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
