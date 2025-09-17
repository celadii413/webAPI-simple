using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI_simple.Data;
using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;
using WebAPI_simple.Repositories;

namespace WebAPI_simple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpGet("get-all-books")]
        public async Task<IActionResult> GetAll()
        {
            var allBooks = await _bookRepository.GetAllBooksAsync();
            return Ok(allBooks);
        }

        [HttpGet("get-book-by-id/{id}")]
        public async Task<IActionResult> GetBookById([FromRoute] int id)
        {
            var book = await _bookRepository.GetBookByIdAsync(id);
            if (book == null) return NotFound();
            return Ok(book);
        }

        [HttpPost("add-book")]
        public async Task<IActionResult> AddBook([FromBody] AddBookRequestDTO addBookRequestDTO)
        {
            var book = await _bookRepository.AddBookAsync(addBookRequestDTO);
            return Ok(book);
        }

        [HttpPut("update-book-by-id/{id}")]
        public async Task<IActionResult> UpdateBookById(int id, [FromBody] AddBookRequestDTO bookDTO)
        {
            var updated = await _bookRepository.UpdateBookByIdAsync(id, bookDTO);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("delete-book-by-id/{id}")]
        public async Task<IActionResult> DeleteBookById(int id)
        {
            var deleted = await _bookRepository.DeleteBookByIdAsync(id);
            if (!deleted) return NotFound();
            return Ok();
        }
    }
}
