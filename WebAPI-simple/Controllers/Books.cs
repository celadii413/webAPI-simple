using Microsoft.AspNetCore.Mvc;
using WebAPI_simple.Data;
using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;

namespace WebAPI_simple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        public BooksController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //GET http://localhost:port/api/books/get-all-books
        [HttpGet("get-all-books")]
        public IActionResult GetAll()
        {
            //var allBooksDomain = _dbContext.Books.ToList();
            //return Ok(allBooksDomain);
            var allBooksDomain = _dbContext.Books;

            var allBooksDTO = allBooksDomain.Select(Books => new Models.DTO.BookWithAuthorAndPublisherDTO()
            {
                Id = Books.Id,
                Title = Books.Title,
                Description = Books.Description,
                IsRead = Books.IsRead,
                DateRead = Books.IsRead ? Books.DateRead.Value : null,
                Rate = Books.IsRead ? Books.Rate.Value : null,
                Genre = Books.Genre,
                CoverUrl = Books.CoverUrl,
                PublisherName = Books.Publisher.Name,
                AuthorNames = Books.Book_Authors.Select(n => n.Author.FullName).ToList()
            }).ToList();

            return Ok(allBooksDTO);
        }

        [HttpGet]
        [Route("get-book-by-id/{id}")]
        public IActionResult GetBookById([FromRoute] int id)
        {
            var bookWithDomain = _dbContext.Books.Where(n => n.Id == id);
            if (bookWithDomain == null)
            {
                return NotFound();
            }

            var bookWithDTO = bookWithDomain.Select(book => new Models.DTO.BookWithAuthorAndPublisherDTO()
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
            });
            return Ok(bookWithDTO);
        }

        [HttpGet("add-book")]
        public IActionResult AddBook([FromBody] AddBookRequestDTO addBookRequestDTO)
        {
            var bookDomainModel = new Book()
            {
                Title = addBookRequestDTO.Title,
                Description = addBookRequestDTO.Description,
                IsRead = addBookRequestDTO.IsRead,
                DateRead = addBookRequestDTO.IsRead ? addBookRequestDTO.DateRead.Value : null,
                Rate = addBookRequestDTO.IsRead ? addBookRequestDTO.Rate.Value : null,
                Genre = addBookRequestDTO.Genre,
                CoverUrl = addBookRequestDTO.CoverUrl,
                DateAdded = DateTime.Now,
                PublisherID = addBookRequestDTO.PublisherID
            };

            _dbContext.Books.Add(bookDomainModel);
            _dbContext.SaveChanges();

            foreach (var id in addBookRequestDTO.AuthorIds)
            {
                var _book_author = new Book_Author()
                {
                    BookId = bookDomainModel.Id,
                    AuthorId = id
                };

                _dbContext.Book_Authors.Add(_book_author);
                _dbContext.SaveChanges();
            }
            return Ok();
        }

        [HttpPut("update-book-by-id/{id}")]
        public IActionResult UpdateBookById(int id, [FromBody] AddBookRequestDTO bookDTO)
        {
            var bookDomain = _dbContext.Books.FirstOrDefault(n => n.Id == id);
            if (bookDomain != null)
            {
                bookDomain.Title = bookDTO.Title;
                bookDomain.Description = bookDTO.Description;
                bookDomain.IsRead = bookDTO.IsRead;
                bookDomain.DateRead = bookDTO.DateRead;
                bookDomain.Rate = bookDTO.Rate;
                bookDomain.Genre = bookDTO.Genre;
                bookDomain.CoverUrl = bookDTO.CoverUrl;
                bookDomain.DateAdded = bookDTO.DateAdded;
                bookDomain.PublisherID = bookDTO.PublisherID;
                _dbContext.SaveChanges();
            }
            var authorDomain = _dbContext.Book_Authors.Where(a => a.BookId == id).ToList();
            if (authorDomain != null)
            {
                _dbContext.Book_Authors.RemoveRange(authorDomain);
                _dbContext.SaveChanges();
            }
            foreach (var authorid in bookDTO.AuthorIds)
            {
                var _book_author = new Book_Author()
                {
                    BookId = id,
                    AuthorId = authorid
                };
                _dbContext.Book_Authors.Add(_book_author);
                _dbContext.SaveChanges();
            }
            return Ok(bookDTO);
        }

        [HttpDelete("delete-book-by-id/{id}")]
        public IActionResult DeleteBookById(int id)
        {
            var bookDomain = _dbContext.Books.FirstOrDefault(n => n.Id == id);
            if (bookDomain != null)
            {
                _dbContext.Books.Remove(bookDomain);
                _dbContext.SaveChanges();
            }
            return Ok();
        }
    }
}
