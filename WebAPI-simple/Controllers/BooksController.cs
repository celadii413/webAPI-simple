using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI_simple.CustomActionFilter;
using WebAPI_simple.Data;
using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;
using WebAPI_simple.Repositories;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery, 
            [FromQuery] string? sortBy, [FromQuery] bool isAscending,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100)
        {
            //sử dụng repository pattern
            var allBooks = await _bookRepository.GetAllBooksAsync(filterOn, filterQuery, sortBy, isAscending, pageNumber, pageSize);
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
        [ValidateModel]
        //[Authorize(Roles = "Write")]
        public async Task<IActionResult> AddBook([FromBody] AddBookRequestDTO addBookRequestDTO)
        {
            try
            {
                if (!ValidateAddBook(addBookRequestDTO))
                    return BadRequest(ModelState);

                var bookAdd = await _bookRepository.AddBookAsync(addBookRequestDTO);
                return Ok(bookAdd);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); 
            }
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

        #region Private methods
        private bool ValidateAddBook(AddBookRequestDTO addBookRequestDTO)
        {
            if (addBookRequestDTO == null)
            {
                ModelState.AddModelError(nameof(addBookRequestDTO), $"Please add book data");
                return false;
            }
            // kiem tra Description NotNull
            if (string.IsNullOrEmpty(addBookRequestDTO.Description))
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.Description),
                $"{nameof(addBookRequestDTO.Description)} cannot be null");
            }
            // kiem tra rating (0,5)
            if (addBookRequestDTO.Rate < 0 || addBookRequestDTO.Rate > 5)
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.Rate),
                $"{nameof(addBookRequestDTO.Rate)} cannot be less than 0 and more than 5");
            }
            if (ModelState.ErrorCount > 0)
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}
