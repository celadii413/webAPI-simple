using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI_simple.CustomActionFilter;
using WebAPI_simple.Data;
using WebAPI_simple.Models.Domain;
using WebAPI_simple.Models.DTO;
using WebAPI_simple.Repositories;

namespace WebAPI_simple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly AppDbContext _dbContext;

        public AuthorsController(IAuthorRepository authorRepository, AppDbContext dbContext)
        {
            _authorRepository = authorRepository;
            _dbContext = dbContext;
        }

        // GET: api/authors/get-all-authors
        [HttpGet("get-all-authors")]
        public async Task<IActionResult> GetAllAuthors()
        {
            var authors = await _authorRepository.GetAllAuthorsAsync();
            return Ok(authors);
        }

        // GET: api/authors/get-author-by-id/1
        [HttpGet("get-author-by-id/{id}")]
        public async Task<IActionResult> GetAuthorById([FromRoute] int id)
        {
            var author = await _authorRepository.GetAuthorByIdAsync(id);
            if (author == null)
                return NotFound($"Author with Id = {id} not found");

            return Ok(author);
        }

        // POST: api/authors/add-author
        [HttpPost("add-author")]
        [ValidateModel]
        public async Task<IActionResult> AddAuthor([FromBody] AddAuthorRequestDTO addAuthorRequestDTO)
        {
            if (!ValidateAddAuthor(addAuthorRequestDTO))
                return BadRequest(ModelState);

            var createdAuthor = await _authorRepository.AddAuthorAsync(addAuthorRequestDTO);
            return Ok(createdAuthor);
        }

        // PUT: api/authors/update-author-by-id/1
        [HttpPut("update-author-by-id/{id}")]
        public async Task<IActionResult> UpdateAuthorById([FromRoute] int id, [FromBody] AuthorNoIdDTO authorNoIdDTO)
        {
            var updatedAuthor = await _authorRepository.UpdateAuthorByIdAsync(id, authorNoIdDTO);
            if (updatedAuthor == null)
                return NotFound($"Author with Id = {id} not found");

            return Ok(updatedAuthor);
        }

        // DELETE: api/authors/delete-author-by-id/1
        [HttpDelete("delete-author-by-id/{id}")]
        public async Task<IActionResult> DeleteAuthorById([FromRoute] int id)
        {
            // kiểm tra nếu còn sách thì không cho xóa
            if (await _dbContext.Book_Authors.AnyAsync(ba => ba.AuthorId == id))
                return BadRequest("Author still has books. Please remove relations in Book_Author before deleting.");

            var deletedAuthor = await _authorRepository.DeleteAuthorByIdAsync(id);
            if (deletedAuthor == null)
                return NotFound($"Author with Id = {id} not found");

            return Ok(deletedAuthor);
        }

        #region Private methods
        private bool ValidateAddAuthor(AddAuthorRequestDTO addAuthorRequestDTO)
        {
            if (addAuthorRequestDTO == null)
            {
                ModelState.AddModelError(nameof(addAuthorRequestDTO), $"Please add author data");
                return false;
            }
            // kiểm tra Author.Name NotNull
            if (string.IsNullOrEmpty(addAuthorRequestDTO.FullName))
            {
                ModelState.AddModelError(nameof(addAuthorRequestDTO.FullName),
                $"{nameof(addAuthorRequestDTO.FullName)} cannot be null");
            }
            // kiểm tra độ dài tối thiểu tên tác giả
            if (addAuthorRequestDTO.FullName.Length < 3)
            {
                ModelState.AddModelError(nameof(addAuthorRequestDTO.FullName),
                    "Author name must be at least 3 characters");
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
